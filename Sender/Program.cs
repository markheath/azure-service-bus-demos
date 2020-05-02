using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = null;
            string queueName = "queue1";
            string topicName = null;
            string messageText = "Hello World";

            for(int n = 0; n < args.Length; n++)
            {
                switch(args[n])
                {
                    case "-c":
                        connectionString = args[++n];
                        break;
                    case "-q":
                        queueName = args[++n];
                        break;
                    case "-t":
                        topicName = args[++n];
                        break;
                    case "-m":
                        messageText = args[++n];
                        break;
                    default:
                        ShowHelp();
                        return;
                }
            }
            if (connectionString == null)
            {
                ShowHelp();
                return;
            }

            ISenderClient senderClient;
            if(topicName != null)
                senderClient = new TopicClient(connectionString, topicName);
            else 
                senderClient = new QueueClient(connectionString, queueName);
            
            var body = Encoding.UTF8.GetBytes(messageText);
            var message = new Message(body);
            message.CorrelationId = Guid.NewGuid().ToString();
            message.Label = "My message";
            message.UserProperties["Sender"] = "Mark";
            if (messageText.ToLower().Contains("delay"))
            {
                message.ScheduledEnqueueTimeUtc = DateTime.UtcNow.AddMinutes(5);
            }
            await senderClient.SendAsync(message);
            Console.WriteLine($"Sent message to {senderClient.Path}");
            await senderClient.CloseAsync();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage: dotnet run [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("-c connectionString [required]");
            Console.WriteLine("-q queueName [default is queue1]");
            Console.WriteLine("-t topicName [if provided sends to topic instead of queue]");
            Console.WriteLine("-m message [default is Hello World]");
        }
    }
}
