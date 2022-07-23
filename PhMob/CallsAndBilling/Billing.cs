namespace PhMob
{
    internal class Billing
    {
        private readonly List<Call> _journal = new();
        internal static int LastPayDay => 30;

        internal void AddCallToJournal(Call call)
        {
            _journal.Add(call);
            Console.WriteLine("Call entry added to log");
        }

        internal List<Call> GetHistory(int number)
        {
            List<Call> calls = _journal.Where(x => x.AbonentFrom == number || x.AbonentTo == number).ToList();
            return calls;
        }

        internal static bool IsBillsPaid(decimal debt)
        {
            return debt >= 0;
        }

        internal static decimal GetCallPrice(Tariff tariff, TimeSpan duration) =>
            Convert.ToDecimal(duration.TotalSeconds) * tariff.Rate;

        internal decimal GetBillLastMonth(int dogovorNumber, DateTime firstDate, DateTime lastDate)
        {
            IEnumerable<Call> dateFilter = _journal.Where(x => x.StartDate >= firstDate & x.StartDate <= lastDate);
            IEnumerable<Call> abonentFilter = dateFilter.Where(x => x.DogovorNumber == dogovorNumber);


            return abonentFilter.Sum(i => i.Amount);
        }

        internal void CountDebtsForAbonents(IEnumerable<Contract> dogovors)
        {
            DateTime date = DateTime.Now.AddMonths(-1);
            DateTime firstDate = new(date.Year, date.Month, 1);
            DateTime lastDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddDays(-1);

            foreach (Contract dogovor in dogovors)
            {
                if (dogovor.LastDateDebtCounted.Month == DateTime.Now.Month)
                {
                    Console.WriteLine("debt has already been calculated");
                    continue;
                }

                decimal summ = GetBillLastMonth(dogovor.ContractNumber, firstDate, lastDate);
                dogovor.SetDebt(summ);
                Console.WriteLine($"Contract # {dogovor.ContractNumber} Balance: {summ} ");
            }
        }
    }
}