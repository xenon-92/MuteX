using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MultipleMutexes
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var ba1 = new BankAccount();
            var ba2 = new BankAccount();

            Mutex m1 = new Mutex();
            Mutex m2 = new Mutex();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveTaken = m1.WaitOne();
                        try
                        {
                            ba1.Deposit(1);
                        }
                        finally
                        {
                            if (haveTaken) m1.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveTaken = m2.WaitOne();
                        try
                        {
                            ba2.Deposit(1);
                        }
                        finally
                        {
                            if (haveTaken) m2.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    for (int j = 0; j < 1000; j++)
                    {
                        bool haveTaken = WaitHandle.WaitAll(new[] { m1, m2 });
                        try
                        {
                            ba1.Transfer(ba2, 1);
                        }
                        finally
                        {
                            if (haveTaken)
                            {
                                m1.ReleaseMutex();
                                m2.ReleaseMutex();
                            }
                        }
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine($"balance in ba1 is {ba1.Balance}");
            Console.WriteLine($"balance in ba2 is {ba2.Balance}");
            Console.ReadKey();
        }
    }
    class BankAccount
    {
        public int Balance { get; set; }
        public void Deposit(int amt)
        {
            Balance += amt;
        }
        public void Withdraw(int amt)
        {
            Balance -= amt;
        }
        public void Transfer(BankAccount where, int amt)
        {
            Balance -= amt;
            where.Balance += amt;
        }
    }
}
