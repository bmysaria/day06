using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using d06.Models.d06.Models;

namespace d06.Models
{
    public class Register
    {
        public TimeSpan TimePerItem { get; }
        public TimeSpan TimePerCustomer { get; }
        public TimeSpan FromStart { get; set; }
        public int No { get; }
        private Store _store;
        public Queue<Customer> QueuedCustomers { get; }
        public Thread Thread { get; }

        public Register(int number, TimeSpan timePerItem, TimeSpan timePerCustomer, Store store)
        {
            FromStart = new TimeSpan(0, 0, 0);
            Thread = new Thread(Process) {Name = $"Register {No}"};
            TimePerCustomer = timePerCustomer;
            TimePerItem = timePerItem;
            No = number;
            QueuedCustomers = new Queue<Customer>();
            _store = store;
        }

        public override string ToString()
            => $"Register#{No} ({QueuedCustomers.Count} customers in line)";

        private void Process()
        {
            while (_store.IsOpen)
            {
                Stopwatch stopWatch = new Stopwatch();
                stopWatch.Start();
                if (!QueuedCustomers.TryDequeue(out Customer customer))
                    continue;
                _store.Storage.TakeWares(customer);
                Thread.Sleep(TimePerCustomer + TimePerItem * customer.ItemsInCart);
                FromStart += stopWatch.Elapsed;
                Console.WriteLine($"{FromStart.Seconds}   {customer} served by {this}");
                stopWatch.Stop();
            }
        }
    }
}