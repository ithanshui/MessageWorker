using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace MessageWorker.AMQP
{
    [Serializable]
    public class AMQPUri : ISerializable
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Host { get; set; }
        public string Port { get; set; }
        
        public AMQPUri()
        {
        }

        public AMQPUri(string userName, string password, string host, string port)
        {
            UserName = userName;
            Password = password;
            Host = host;
            Port = port;
        }
        //"amqp://guest:guest@localhost:5672"
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("AMQP_Uri", this.ToString());
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder("amqp://");
            if (!string.IsNullOrEmpty(UserName) || !string.IsNullOrEmpty(Password))
                result.Append($"{UserName}:{Password}@");
            result.Append($"{Host}:{Port}@");

            return ToString();
        }
    }
}
