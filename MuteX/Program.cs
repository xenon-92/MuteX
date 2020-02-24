using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace MuteX
{
    class Program
    {
        static void Main(string[] args)
        {
            var tasks = new List<Task>();
            var ba = new BankAccount();
            Mutex mutex = new Mutex();
            for (int i = 0; i < 10; i++)
            {
                tasks.Add(Task.Factory.StartNew(()=> {
                    for (int j = 0; j < 100; j++)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Deposit(100);
                        }
                        finally
                        {
                            if (haveLock)
                                mutex.ReleaseMutex();
                        }
                    }
                }));
                tasks.Add(Task.Factory.StartNew(() => {
                    for (int j = 0; j < 100; j++)
                    {
                        bool haveLock = mutex.WaitOne();
                        try
                        {
                            ba.Withdraw(100);
                        }
                        finally
                        {
                            if (haveLock)
                            {
                                mutex.ReleaseMutex();
                            }
                        }
                    }
                }));
            }
            Task.WaitAll(tasks.ToArray());
            Console.WriteLine(ba.Balance);
            Console.ReadKey();
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
        }
    }
}
