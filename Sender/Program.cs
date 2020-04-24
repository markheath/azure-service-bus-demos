using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;

namespace Sender
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var connectionString = args[0];
            var queueName = "queue1";
            var queueClient = new QueueClient(connectionString, queueName);
            var messageText = args[1];
            var body = Encoding.UTF8.GetBytes(messageText);
            var message= new Message(body);
            await queueClient.SendAsync(message);
            Console.WriteLine("Sent message");
        }
    }
}
