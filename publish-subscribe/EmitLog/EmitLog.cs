using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace EmitLog
{
    class Program
    {
        static void Main(string[] args)
        {
             var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin2017" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("logs", type: ExchangeType.Fanout);

                var message =  GetMessage(args);
                var body = Encoding.UTF8.GetBytes(message);

                channel.BasicPublish("logs",
                "",
                basicProperties: null,
                body);
                System.Console.WriteLine(" [x] sent {0}", message);
            }

            System.Console.WriteLine(" Press [enter] to exit");
            Console.ReadLine();
        }

        private static string GetMessage(string[] args) {
            return ((args.Length > 0) ? string.Join(" ", args) : "info: Hello World");
        }
    }
}
