using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadingPractice
{
    // Event-Based ValueReturining
    delegate void ValueChanged(FibonacchiValueContainer container);
    class FibonacchiValueContainer
    {
        public event ValueChanged OnValueChanged;
        private int _result;
        public int Result
        {
            get { return _result; }
            set
            {
                _result = value;
                OnValueChanged(this);               
            }
        }
    }
    class MessageBus
    {
        public void PullMessageFromQueue<T>(string queueName)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            var consumer = new EventingBasicConsumer(channel);
            channel.QueueDeclare(queue: queueName,
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);
            channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);
            Console.WriteLine($" [*] Waiting for messages from {queueName} in thread {Thread.CurrentThread.ManagedThreadId}:");
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                var deserializeBody = JsonConvert.DeserializeObject<T>(message);
                Console.WriteLine(" [x] Received {0}", deserializeBody);
                channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };
            channel.BasicConsume(queue: queueName,
                                 autoAck: false,
                                 consumer: consumer);
        }
    }
    class Program
    {
        public static void Fibonachi(int a, FibonacchiValueContainer container)
        {
            int[] data = new int[a + 1];
            data[0] = 0;
            data[1] = 1;
            for(int i = 2; i <= a; i++)
            {
                data[i] = data[i - 1] + data[i - 2];
            }
            Thread.Sleep(5000);
            container.Result = data[a];
        }

        static void OnValueChanged1(FibonacchiValueContainer container)
        {
            Console.WriteLine(container.Result);
        }
        static void Main(string[] args)
        {
            // Task One
            /* 
            MessageBus bus = new MessageBus();
            List<Thread> queueConsumers = new List<Thread>();
            queueConsumers.Add(new Thread(() => bus.PullMessageFromQueue<string>("QueueOne")));
            queueConsumers.Add(new Thread(() => bus.PullMessageFromQueue<string>("QueueTwo")));
            queueConsumers.Add(new Thread(() => bus.PullMessageFromQueue<string>("QueueThree")));
            queueConsumers.Add(new Thread(() => bus.PullMessageFromQueue<string>("QueueFour")));
            foreach (var item in queueConsumers)
            {
                item.Start();
            }
            Console.WriteLine("Enter command: 1 - Add new queue");
            string command = Console.ReadLine();
            // ADD_QUEUE QUEUE_NAME
            string [] commandParsed = command.Split(' ');
            if(commandParsed[0] == "ADD_QUEUE")
            {
                Thread queue = new Thread(() => bus.PullMessageFromQueue<string>(commandParsed[1]));
                queue.Start();
                queueConsumers.Add(queue);
                Console.WriteLine("OK, queue added!");
            }
            */
            //Task2 

            /*
            int[] results = new int[2];

            Console.WriteLine("Enter Fib value");
            int fib = Int32.Parse(Console.ReadLine());
            Console.WriteLine("Wait");

            Thread anotherThread = new Thread(() => Fibonachi(fib, out results[0]));
            anotherThread.Start();

            Console.WriteLine("Enter another Fib again?");
            int nextRequest = Int32.Parse(Console.ReadLine());

            Thread anotherThread2 = new Thread(() => Fibonachi(nextRequest, out results[1]));
            anotherThread2.Start();

            anotherThread.Join();
            anotherThread2.Join();

            Console.WriteLine("RESULTS");
            Console.WriteLine(results[0]);
            Console.WriteLine(results[1]);
            */

            //FibonacchiValueContainer container = new FibonacchiValueContainer();
            //container.OnValueChanged += OnValueChanged1;

            //Thread anotherThread = new Thread(() => Fibonachi(45, container));
            //anotherThread.Start();


            /*class DatabaseConnection
   {
       private static object _locker = new object();
       private static Semaphore _semaphore = new Semaphore(2,2);
       public void WriteTransactionToDb()
       {
           //lock (_locker)
           //{
           _semaphore.WaitOne();
           using (SqlConnection sqlConnection = new SqlConnection())
           {
               Console.WriteLine("Opened SQL connection in thread # " +
                   Thread.CurrentThread.ManagedThreadId);

               Thread.Sleep(1000);


           }
           _semaphore.Release();
           Console.WriteLine("Closed SQL connection in thread # " +
                   Thread.CurrentThread.ManagedThreadId);
           //}

       }
   }*/


            DatabaseConnection db = new DatabaseConnection();

            while (true)
            {
                string name = Console.ReadLine();
                string password = Console.ReadLine();

                Thread thread = new Thread(() => db.ConnectToDb(name, password));
                thread.Start();
            }
            


            Console.ReadLine();
        }
    }
}
