using System;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Threading;
using ThreadState = System.Threading.ThreadState;

namespace CsharpStudy
{
    public class ThreadTestDocs
    {
        private static Object obj = new Object();
        private static Thread thread1, thread2, thread3;
        private static TimeSpan waitTime = new TimeSpan(0, 0, 1);
        enum TestMode
        {
            FGTest = 1,
            BGTest,
            FGParameterizedTest,
            CurrentThreadPropertyTest,
            JoinTest,
            JoinWithTimeoutTest,
            JoinWithTimeSpanTest,
            JoinWithTimeSpanTest2,
            YieldTest,
        }
        public static void Main()
        {
            TestMode mode = TestMode.YieldTest;
            Console.WriteLine($"TestMode:{mode.ToString()}");
            switch (mode)
            {
                case TestMode.FGTest:
                {
                    var th = new Thread(ExecuteInForeground);
                    th.Start();
                    Thread.Sleep(1000);
                    break;
                }
                case TestMode.BGTest:
                {
                    var th = new Thread(ExecuteInForeground);
                    th.IsBackground = true; // FGTest 와 이 부분만 다름
                    th.Start();
                    Thread.Sleep(1000);
                    break;
                }
                case TestMode.FGParameterizedTest:
                {
                    var th = new Thread(ExecuteInForegroundWithParam);
                    th.Start(3000);
                    Thread.Sleep(1000);
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
                case TestMode.JoinTest:
                {
                    thread1 = new Thread(JoinTest);
                    thread1.Name = "Thread1";
                    thread1.Start();
                    thread2 = new Thread(JoinTest);
                    thread2.Name = "Thread2";
                    thread2.Start();
                    break;
                }
                case TestMode.JoinWithTimeoutTest:
                {
                    thread1 = new Thread(JoinWithTimeoutTest);
                    thread1.Name = "Thread1";
                    thread1.Start();
                    thread2 = new Thread(JoinWithTimeoutTest);
                    thread2.Name = "Thread2";
                    thread2.Start();
                    break;
                }
                case TestMode.JoinWithTimeSpanTest:
                {
                    Thread newThread = new Thread(TimeSleep);
                    newThread.Start();

                    if (newThread.Join(waitTime + waitTime))
                        Console.WriteLine("newThread terminated.");
                    else 
                        Console.WriteLine("Join time out.");
                    break;
                }
                case TestMode.JoinWithTimeSpanTest2:
                {
                    thread1 = new Thread(JoinWithTimeSpanTest);
                    thread1.Name = "Thread1";
                    thread1.Start();
                    thread2 = new Thread(JoinWithTimeSpanTest);
                    thread2.Name = "Thread2";
                    thread2.Start();
                    
                    break;
                }
                case TestMode.YieldTest:
                {
                    thread1 = new Thread(ThreadPriority);
                    thread1.Priority = System.Threading.ThreadPriority.Highest;
                    thread1.Name = "Thread1";
                    thread1.Start();
                    thread2 = new Thread(ThreadPriority);
                    thread2.Priority = System.Threading.ThreadPriority.Normal;
                    thread2.Name = "Thread2";
                    thread2.Start();
                    thread3 = new Thread(ThreadPriority);
                    thread3.Priority = System.Threading.ThreadPriority.Lowest;
                    thread3.Name = "Thread3";
                    thread3.Start();
                    break;
                }
            }

            Console.WriteLine("Main thread ({0}) exiting...",
                Thread.CurrentThread.ManagedThreadId);
            
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
        private static void JoinTest()
        {
            Console.WriteLine($"\nCurrent thread: {Thread.CurrentThread.Name}");
            if (Thread.CurrentThread.Name == "Thread1" && thread2.ThreadState != ThreadState.Unstarted)
            {
                thread2.Join();
            }
            
            Thread.Sleep(4000);
            Console.WriteLine($"\nCurrent thread: {Thread.CurrentThread.Name}");
            Console.WriteLine($"Thread1: {thread1.ThreadState}");
            Console.WriteLine($"Thread2: {thread2.ThreadState}");
        }
        
        private static void JoinWithTimeoutTest()
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"\nCurrent thread: {Thread.CurrentThread.Name}");
            if (Thread.CurrentThread.Name == "Thread1" && thread2.ThreadState != ThreadState.Unstarted)
            {
                if (thread2.Join(2000))
                    Console.WriteLine("Thread2 has terminated.");
                else
                    Console.WriteLine($"The timeout has elapsed and Thread1 will resume.{Thread.CurrentThread.Name}, {thread1.ThreadState}");
            }
            Console.WriteLine($"Current thread: {Thread.CurrentThread.Name}, timeElapsed:{sw.ElapsedMilliseconds}"); ;
            Thread.Sleep(4000);
            Console.WriteLine($"\nCurrent thread: {Thread.CurrentThread.Name}");
            Console.WriteLine($"Thread1: {thread1.ThreadState}");
            Console.WriteLine($"Thread2: {thread2.ThreadState}");
        }

        private static void TimeSleep()
        {
            Thread.Sleep(waitTime);
            Console.WriteLine($"TimeSleep");
        }
        
        private static void JoinWithTimeSpanTest()
        {
            var sw = Stopwatch.StartNew();
            Console.WriteLine($"\nCurrent thread: {Thread.CurrentThread.Name}");
            if (Thread.CurrentThread.Name == "Thread1" && thread2.ThreadState != ThreadState.Unstarted)
            {
                if (thread2.Join(TimeSpan.FromSeconds(2)))
                    Console.WriteLine("Thread2 has terminated.");
                else
                    Console.WriteLine($"The timeout has elapsed and Thread1 will resume.{Thread.CurrentThread.Name}, {thread1.ThreadState}");
            }
            Console.WriteLine($"Current thread: {Thread.CurrentThread.Name}, timeElapsed:{sw.ElapsedMilliseconds}"); ;
            Thread.Sleep(4000);
            Console.WriteLine($"\nCurrent thread: {Thread.CurrentThread.Name}");
            Console.WriteLine($"Thread1: {thread1.ThreadState}");
            Console.WriteLine($"Thread2: {thread2.ThreadState}");
        }

        private static void ThreadPriority()
        {
            int count = 0;
            while (count < 50)
            {
                count++;
                Console.WriteLine($"Count:{count}, Current Thread: {Thread.CurrentThread.ManagedThreadId}/{Thread.CurrentThread.Name}, Priority: {Thread.CurrentThread.Priority}");
                Thread.Yield();
            }
            
            Console.WriteLine($"Thread  {Thread.CurrentThread.ManagedThreadId}/ {Thread.CurrentThread.Name} is terminated");
        }
    }
    
    
}