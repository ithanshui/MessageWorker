using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MessageWorker.Kafka
{
    public class KafkaSettings
    {
        public bool IsAutocommit { get; set; }
        public string GroupId { get; set; }
        public string ClientId { get; set; }
        public IEnumerable<string> Servers { get; set; }
        public string SecurityProtocol { get; set; }
        public SaslConfig SaslConfig { get; set; }

        public KafkaSettings() { }

        public void Init(string notificationSettingsString)
        {
            var dict = ParseValue(notificationSettingsString);

            if (!dict.TryGetValue("groupid", out var _groupId))
                throw new ArgumentException($"Не указан groupid для подключения к Kafka", "groupid");
            GroupId = _groupId;

            dict.TryGetValue("clientid", out var _clientId);
            ClientId = _clientId;

            string server = string.Empty;
            if (dict.TryGetValue("servers", out server))
            {
                Servers = server.Split(',').Select(n => n.Trim());
            }
            else
                throw new ArgumentException($"Не указан servers для подключения к Kafka", "servers");

            if (!Servers.Any())
                throw new ArgumentException($"Не удалось распарсить servers для подключения к Kafka (формат поля: 'servername1:port1, servername2:port2, servername3:port3.....')");

            dict.TryGetValue("securityprotocol", out var _securityProtocol);
            SecurityProtocol = _securityProtocol;

            switch (SecurityProtocol?.ToUpper())
            {
                case "SASL_PLAINTEXT":
                    SaslConfig = new SaslConfig();
                    SaslConfig.Init(dict);
                    break;
            }
        }

        internal Dictionary<string, string> ParseValue(string notificationSettingsString)
        {
            ///Преобразуем строку вида: Property1=value1;property2=value2
            var result = notificationSettingsString
                .Split(';').Where(n => !string.IsNullOrEmpty(n))
                .Select(n => n.Split('='))
                .ToDictionary(k => k[0].Trim().ToLower(), v => v[1].Trim());

            return result;
        }

        public Dictionary<string, object> GetDictionaryConfig()
        {
            var configDictionary
                = SaslConfig?.GetDictionaryConfig()
                ?? new Dictionary<string, object>();

            if (!string.IsNullOrEmpty(GroupId))
                configDictionary.Add("group.id", GroupId);
            if (!string.IsNullOrEmpty(ClientId))
                configDictionary.Add("client.id", ClientId);
            configDictionary.Add("bootstrap.servers", Servers.Aggregate((n, j) => $"{n},{j}"));
            configDictionary.Add("enable.auto.commit", IsAutocommit);

            return configDictionary;
        }
    }
}
