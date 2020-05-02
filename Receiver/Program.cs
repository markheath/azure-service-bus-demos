using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Core;

namespace Receiver
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connectionString = null;
            string queueName = "queue1";
            string subscriptionName = null;
            string topicName = null;
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
                    case "-s":
                        subscriptionName = args[++n];
                        break;
                    default:
                        ShowHelp();
                        return;
                }
            }
            if (connectionString  == null)
            {
                ShowHelp();
                return;
            }

            IReceiverClient listenerClient;
            if (topicName != null)
            {
                listenerClient = new SubscriptionClient(connectionString, topicName, subscriptionName);
            }
            else
            {
                listenerClient = new QueueClient(connectionString, queueName);
            }
            var messageHandlerOptions = new MessageHandlerOptions(OnException);
            messageHandlerOptions.MaxConcurrentCalls = 4;
            Console.WriteLine($"{messageHandlerOptions.AutoComplete},{messageHandlerOptions.MaxAutoRenewDuration},{messageHandlerOptions.MaxConcurrentCalls}");
            listenerClient.RegisterMessageHandler(OnMessage, messageHandlerOptions);
            Console.WriteLine($"Listening on {listenerClient.Path}, press any key");
            Console.ReadKey();
            await listenerClient.CloseAsync();
        }

        static async Task OnMessage(Message m, CancellationToken ct)
        {
            
            var messageText = Encoding.UTF8.GetString(m.Body);
            Console.WriteLine("Got a message:");
            Console.WriteLine(messageText);
            
            Console.WriteLine($"Enqueued at {m.SystemProperties.EnqueuedTimeUtc}");
            Console.WriteLine($"CorrelationId: {m.CorrelationId}"); // not filled in for you
            Console.WriteLine($"ContentType: {m.ContentType}"); // not filled in for you
            Console.WriteLine($"Label: {m.Label}"); // not filled in for you
            Console.WriteLine($"MessageId: {m.MessageId}"); // used for deduplication - is provided for you
            foreach(var prop in m.UserProperties)
            {
                Console.WriteLine($"{prop.Key}={prop.Value}");
            }

            if (messageText.ToLower().Contains("sleep"))
            {
                await Task.Delay(5000);
            }
            
            if (messageText.ToLower().Contains("error"))
            {
                throw new InvalidOperationException("something went wrong handling this message");
            }

            Console.WriteLine($"Finished processing: {messageText}");
            
        }

        static Task OnException(ExceptionReceivedEventArgs args)
        {
            Console.WriteLine("Got an exception:");
            Console.WriteLine(args.Exception.Message);
            Console.WriteLine(args.ExceptionReceivedContext.Action);
            Console.WriteLine(args.ExceptionReceivedContext.ClientId);
            Console.WriteLine(args.ExceptionReceivedContext.Endpoint);
            Console.WriteLine(args.ExceptionReceivedContext.EntityPath);
            return Task.CompletedTask;
        }

                private static void ShowHelp()
        {
            Console.WriteLine("Usage: dotnet run [options]");
            Console.WriteLine("Options:");
            Console.WriteLine("-c connectionString [required]");
            Console.WriteLine("-q queueName [default is queue1]");
            Console.WriteLine("-s subscriptionName [alternative to listening on a queue]");
            Console.WriteLine("-t topicName [required if listening to topic instead of queue]");
        }
    }
}
