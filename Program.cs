using System;
using System.Threading;

namespace ConcurrencyTest
{
    class Program
    {
        static void Main(string[] args)
        {
           EventWaitHandleTest2();
        }
        static void ProducerConsumerQueueTest() {
            using(ProducerConsumerQueue queue = new ProducerConsumerQueue())
            {
                queue.EnqueueTask("Hellow!");

                for(int i = 0; i < 10; i++)
                    queue.EnqueueTask("Message " + i);
                
                queue.EnqueueTask("Goodbay!");
            }
            // Выход из using приводит к вызвоу Dispose, который ставит
            // в очередь Null-задачу и ожидает, пока Потребитель не завершится
        }

        static EventWaitHandle wh = new AutoResetEvent(false);
        static EventWaitHandle ready = new AutoResetEvent(false);
        static EventWaitHandle go = new AutoResetEvent(false);
        static volatile string task;
        static void EventWaitHandleTest2()
        {
            new Thread(Work).Start();

            // Сигнализируем рабочему потоку 5 раз
            for(int i = 1; i <= 5; i++)
            {
                ready.WaitOne(); //Сначала ждем, когда рабочий поток будет готов
                task = "a".PadRight(i, 'h');
                go.Set(); //Говорим разбочему потоку что можно начинать
            }
            // Сообщаем о необходимости завершения рабочего потока,
            // используя null-строку
            ready.WaitOne();
            task = null;
            go.Set();
        }
        static void Work()
        {
            while(true)
            {
                ready.Set();
                go.WaitOne();
                if (task == null)
                    return;
                Console.WriteLine(task);
            }
        }

        static void EventWaitHandleTest(){
            new Thread(Waiter).Start();
            Thread.Sleep(1000);
            wh.Set();
        }

        static void Waiter()
        {
            Console.WriteLine("Waiting...");
            wh.WaitOne();
            Console.WriteLine("Got the signal");
        }
        static void InterruptTest()
        {
            Thread t = new Thread(delegate()
            {
                try{
                    Thread.Sleep(Timeout.Infinite);
                    Console.WriteLine("Code after block with Sleep");
                }
                catch(ThreadInterruptedException)
                {
                    Console.WriteLine("Forcibly");
                }

                Console.WriteLine("Woken!");
            });
            
            t.Start();
            t.Interrupt();
        }
    }
}
