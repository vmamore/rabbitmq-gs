using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLog
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

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName,
                    "logs",
                    "");
                System.Console.WriteLine(" [*] Waiting for logs");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) => {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    System.Console.WriteLine(" [x] {0}", message);
                };
                channel.BasicConsume(queueName, true, consumer);

                System.Console.WriteLine("Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
