using System;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace ReceiveLogDirect
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "admin", Password = "admin2017" };
            using(var connection = factory.CreateConnection())
            using(var channel = connection.CreateModel()) {
                channel.ExchangeDeclare(exchange: "direct_logs", ExchangeType.Direct);

                var queueName = channel.QueueDeclare().QueueName;

                if(args.Length < 1) {
                    System.Console.Error.WriteLine("Usage: {0} [info] [warning] [error]",
                        Environment.GetCommandLineArgs()[0]);
                    System.Console.WriteLine("Press [enter] to exit.");
                    Console.ReadLine();
                    Environment.ExitCode = 1;
                    return;
                }

                foreach(var severity in args) {
                    channel.QueueBind(queueName, "direct_logs", severity);
                }

                System.Console.WriteLine(" [*] Waiting for messages...");

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += (model, ea) => {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    var routignKey = ea.RoutingKey;
                    System.Console.WriteLine(" [x] Received '{0}':'{1}'", routignKey, message);
                };
                channel.BasicConsume(queueName, true, consumer);
                System.Console.WriteLine("Press [enter] to exit");
                Console.ReadLine();
            }
        }
    }
}
