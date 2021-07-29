﻿using System;
using System.Linq;
using System.Text;
using RabbitMQ.Client;

namespace EmitLogDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin2017" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: "direct_logs", ExchangeType.Direct);

                var severity = (args.Length > 0) ? args[0] : "info";
                var message = (args.Length > 1)
                    ? string.Join(" ", args.Skip(1).ToArray())
                    : "Hello World";
                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "direct_logs",
                    routingKey: severity,
                    basicProperties: null,
                    body);
                Console.WriteLine(" [x] Sent '{0}':'{1}'", severity, message);
            }

            System.Console.WriteLine(" Press [enter] to exit");
            Console.ReadLine();
        }
    }
}
