using System;
using System.Collections.Generic;
using System.Text;

namespace MessageWorker.Kafka
{
    public class SaslConfig
    {
        public SaslConfig()
        {
        }
        public void Init(Dictionary<string, string> dictParams)
        {
            if (!dictParams.TryGetValue("saslmechanism", out var _mechanism))
                throw new ArgumentException($"Не указан saslmechanism для подключения к Kafka", "saslmechanism");

            if (!dictParams.TryGetValue("saslusername", out var _username))
                throw new ArgumentException($"Не указан saslusername для подключения к Kafka", "saslusername");

            if (!dictParams.TryGetValue("saslpassword", out var _password))
                throw new ArgumentException($"Не указан saslpassword для подключения к Kafka", "saslpassword");
        }

        public string Mechanism { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public Dictionary<string, object> GetDictionaryConfig()
        {
            var конфиг = new Dictionary<string, object> {
                { "security.protocol", "SASL_PLAINTEXT"},
                { "sasl.mechanism", Mechanism},
                { "sasl.username", Username },
                { "sasl.password", Password },
            };
            return конфиг;
        }
    }
}
