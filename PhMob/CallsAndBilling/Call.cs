﻿namespace PhMob
{
    public class Call
    {
        public int DogovorNumber { get; }
        public DateTime StartDate { get; }
        public DateTime FinishDate { get; }
        public TimeSpan Duration { get; }
        public Tariff Tariff { get; }
        public int AbonentFrom { get; }
        public int AbonentTo { get; }
        public decimal Amount { get; }

        public Call(int dogovorNumber, Tariff tariff, DateTime startDate, DateTime finishDate, int fromAbonentNumber, int toAbonentNumber, decimal amount)
        {
            DogovorNumber = dogovorNumber;
            StartDate = startDate;
            FinishDate = finishDate;
            Duration = finishDate - startDate;
            AbonentFrom = fromAbonentNumber;
            AbonentTo = toAbonentNumber;
            Tariff = tariff;
            Amount = amount;
        }


    }
}
