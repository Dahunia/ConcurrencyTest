using System;
using System.Collections.Generic;
using System.Threading;

namespace ConcurrencyTest
{
    public class ProducerConsumerQueue : IDisposable
    {
        EventWaitHandle wh = new AutoResetEvent(false);
        Thread worker;
        object locker = new object();
        Queue<string> tasks = new Queue<string>();

        public ProducerConsumerQueue()
        {
            worker = new Thread(Work);
            worker.Start();
        }
        public void EnqueueTask(string task)
        {
            lock(locker)
                tasks.Enqueue(task);

            wh.Set();
        }
        public void Work()
        {
            while(true)
            {
                string task = null;
                lock(locker)
                {
                    if (tasks.Count > 0)
                    {
                        task = tasks.Dequeue();
                        if(task == null)
                            return;
                    }
                }

                if (task != null)
                {
                    Console.WriteLine("Executing task: " + task);
                    Thread.Sleep(1000);
                }
                else
                    wh.WaitOne();
            }
        }
        public void Dispose()
        {
            EnqueueTask(null);
            worker.Join();
            wh.Close();
        }
    }
}