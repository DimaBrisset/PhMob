namespace PhMob
{
    public class Contract
    {
        public int ContractNumber { get; }

        public Tariff Tariff { get; private set; }
        internal DateTime DateChangeTariff { get; private set; }
        internal DateTime LastDateDebtCounted { get; private set; }
        internal decimal Balance { get; private set; }

        internal decimal Debt
        {
            get => _debt;
            set
            {
                _debt = value;

                if (_debt >= 0)
                {
                    Balance += _debt;
                    _debt = 0;
                }
            }
        }

        private decimal _debt;

        internal Contract(int contractNumber, Tariff tariff)
        {
            Balance = 0.00M;
            Debt = 0.00M;
            ContractNumber = contractNumber;
            Tariff = tariff;
            DateChangeTariff = DateTime.Today;
            LastDateDebtCounted = DateTime.Today;
        }

        public void ChangeTariff(Tariff tariff)
        {
            if (DateTime.Today.AddMonths(-1) >= DateChangeTariff)
            {
                if (Tariff == tariff)
                {
                    Console.WriteLine($"You already have a tariff- {tariff.Name}");
                    return;
                }

                Tariff = tariff;
                Console.WriteLine($"Tariff was changed- {tariff.Name}");
                DateChangeTariff = DateTime.Now;
            }

            else
            {
                Console.WriteLine(
                    $"Refuce.You can change the tariff only once a month(next change date:  {DateChangeTariff.AddMonths(1).ToShortDateString()})");
            }
        }

        internal void PayBills(decimal sum)
        {
            Debt += sum;
            Console.WriteLine($"Add money {sum} . New Balance {Balance}");
            Console.WriteLine($"The debt is:{Debt}");
        }

        internal void SetDebt(decimal sum)
        {
            Debt -= sum;


            if (Balance <= 0)
                return;
            Balance += Debt;

            if (Balance <= 0)
            {
                Debt = Balance;
                Balance = 0;
            }

            else
            {
                Debt = 0;
            }

            LastDateDebtCounted = DateTime.Now;
        }
    }
}