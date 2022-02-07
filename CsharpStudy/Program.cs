using System;
using System.Threading;

namespace CsharpStudy
{
    public class ThreadExample
    {
        public static void ThreadProc()
        {
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"ThreadProc: {i}, TID:{Thread.CurrentThread.ManagedThreadId}");
                // Yield the rest of the time slice.
                Thread.Sleep(0);
            }
        }
        
        static void Main(string[] args)
        {
            Console.WriteLine("Main thread: Start a second thread.");
            Thread t = new Thread(new ThreadStart(ThreadProc));
            
            t.Start();
            //Thread.Sleep(0);

            for (int i = 0; i < 4; i++) {
                Console.WriteLine($"Main thread: Do some work.{Thread.CurrentThread.ManagedThreadId}");
                Thread.Sleep(0);
            }

            Console.WriteLine("Main thread: Call Join(), to wait until ThreadProc ends.");
            t.Join();
            Console.WriteLine("Main thread: ThreadProc.Join has returned.  Press Enter to end program.");
            Console.ReadLine();
        }
    }

    class Program
    {
       
    }
}