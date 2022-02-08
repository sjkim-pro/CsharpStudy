using System;
using System.Diagnostics;
using System.Threading;

namespace CsharpStudy
{
    public class ThreadTestForeground
    {
        private static Object obj = new Object();
        enum TestMode
        {
            FGTest = 1,
            BGTest,
            FGParameterizedTest,
            CurrentThreadPropertyTest
        }
        public static void Main()
        {
            TestMode mode = TestMode.CurrentThreadPropertyTest;
            Console.WriteLine($"TestMode:{mode.ToString()}");
            switch (mode)
            {
                case TestMode.FGTest:
                {
                    var th = new Thread(ExecuteInForeground);
                    th.Start();
                    Thread.Sleep(1000);
                    Console.WriteLine("Main thread ({0}) exiting...",
                        Thread.CurrentThread.ManagedThreadId);
                    break;
                }
                case TestMode.BGTest:
                {
                    var th = new Thread(ExecuteInForeground);
                    th.IsBackground = true; // FGTest 와 이 부분만 다름
                    th.Start();
                    Thread.Sleep(1000);
                    Console.WriteLine("Main thread ({0}) exiting...",
                        Thread.CurrentThread.ManagedThreadId);
                    break;
                }
                case TestMode.FGParameterizedTest:
                {
                    var th = new Thread(ExecuteInForegroundWithParam);
                    th.Start(3000);
                    Thread.Sleep(1000);
                    Console.WriteLine("Main thread ({0}) exiting...",
                        Thread.CurrentThread.ManagedThreadId);
                    break;
                }
                case TestMode.CurrentThreadPropertyTest:
                {
                    ThreadPool.QueueUserWorkItem(ShowThreadInformation);
                    var th1 = new Thread(ShowThreadInformation);
                    th1.Name = "FG, th1";
                    th1.Start();
                    var th2 = new Thread(ShowThreadInformation);
                    th2.Name = "BG, th2";
                    th2.IsBackground = true;
                    th2.Start();
                    Thread.Sleep(500);
                    ShowThreadInformation(null);
                    break;
                }
            }

            
            
        }
        
        private static void ExecuteInForeground()
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Thread {0}: {1}, Priority {2}",
                Thread.CurrentThread.ManagedThreadId,
                Thread.CurrentThread.ThreadState,
                Thread.CurrentThread.Priority);
            do {
                Console.WriteLine("Thread {0}: Elapsed {1:N2} seconds",
                    Thread.CurrentThread.ManagedThreadId,
                    sw.ElapsedMilliseconds / 1000.0);
                Thread.Sleep(500);
            } while (sw.ElapsedMilliseconds <= 5000);
            sw.Stop();
        }
        
        private static void ExecuteInForegroundWithParam(Object obj)
        {
            int interval;
            try {
                interval = (int) obj;
            }
            catch (InvalidCastException) {
                interval = 5000;
            }
            var sw = Stopwatch.StartNew();
            Console.WriteLine("Thread {0}: {1}, Priority {2}",
                Thread.CurrentThread.ManagedThreadId,
                Thread.CurrentThread.ThreadState,
                Thread.CurrentThread.Priority);
            do {
                Console.WriteLine("Thread {0}: Elapsed {1:N2} seconds",
                    Thread.CurrentThread.ManagedThreadId,
                    sw.ElapsedMilliseconds / 1000.0);
                Thread.Sleep(500);
            } while (sw.ElapsedMilliseconds <= interval);
            sw.Stop();
        }

        private static void ShowThreadInformation(Object state)
        {
            lock (obj)
            {
                var th = Thread.CurrentThread;
                Console.WriteLine($"Managed thread #{th.ManagedThreadId}");
                Console.WriteLine($"   Name: {th.Name}");
                Console.WriteLine($"   Background thread: {th.IsBackground}");
                Console.WriteLine($"   Thread pool thread: {th.IsThreadPoolThread}");
                Console.WriteLine($"   Priority: {th.Priority}");
                Console.WriteLine($"   Culture: {th.CurrentCulture.Name}");
                Console.WriteLine($"   UI Culture: {th.CurrentUICulture.Name}");
                Console.WriteLine();
                
            }
        }
    }
}