using System.Globalization;

namespace PhMob
{
    public class UI
    {
          
       public static void Menu(Station ats)
        {
         
            User[] abonents = CreateDogovors(ats, 3);

            bool isWorking = true;

            while (isWorking)
            {
                Console.WriteLine();
                Console.WriteLine();
                Console.WriteLine("1. Call to ");
                Console.WriteLine("2. Show Balance  ");
                Console.WriteLine("3. Add Balance");
                Console.WriteLine("4. Call log");
                Console.WriteLine("5. Debt calculation");
                Console.WriteLine("6. Change Tariff");
                Console.WriteLine("7. Exit");

                try
                {
                    int command = Convert.ToInt32(Console.ReadLine());

                    switch (command)
                    {

                        case 1:
                            MakeDial(abonents);
                            break;
                        case 2:
                            GetBalance(abonents);
                            break;
                        case 3:
                            AddMoney(ats, abonents);
                            break;
                        case 4:
                            GetHistory(ats, abonents);
                            break;
                        case 5:
                            ats.CountDebts();
                            break;
                        case 6:
                            ChangeTariff(abonents);
                            break;
                        case 7:
                            isWorking = false;
                            continue;
                    }
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        private static void AddMoney(Station ats, User[] abonents)
        {
            Console.WriteLine("Port #:");
            int number = Convert.ToInt32(Console.ReadLine());

            User currentAbonent = abonents[number];

            Console.WriteLine("Add Amount:");
            decimal amount = Convert.ToDecimal(Console.ReadLine());

            amount = Math.Truncate(100 * amount) / 100;
            ats.AddMoney(currentAbonent.Contract, amount);
        }


        private static void ChangeTariff(User[] abonents)
        {
            Console.WriteLine("Contract #");
            int number = Convert.ToInt32(Console.ReadLine());

            User currentAbonent = abonents[number];

            Console.WriteLine($"Tariff himself :{currentAbonent.Contract.Tariff.Name}" + Environment.NewLine);
            Console.WriteLine("All Tariffs: " + Environment.NewLine);


            _ = typeof(Tariff);

            Tariff[] tariffs = new Tariff[3];
            tariffs[0] = new StartTariff();
            tariffs[1] = new StandardTariff();
            tariffs[2] = new UltraTariff();

            Console.WriteLine("insert # New Tariff: ");

            for (int i = 0; i < tariffs.Length; i++)
            {
                Console.WriteLine($"{i} {tariffs[i].Name}");
            }

            int current = Convert.ToInt32(Console.ReadLine());
            currentAbonent.Contract.ChangeTariff(tariffs[current]);
        }


        private static void GetHistory(Station ats, User[] abonents)
        {
            Console.WriteLine("Port #");
            int number = Convert.ToInt32(Console.ReadLine());
            User currentAbonent = abonents[number];

            List<Call> history = ats.GetHistory(currentAbonent.Port.AbonentNumber);

            if (history == null)
            {
                Console.WriteLine($"{currentAbonent.Port.AbonentNumber} - not faond");
                return;
            }

            Console.WriteLine("    Date   \t --\t \t Duration  \t Cost");

            foreach (var s in history)
            {
                Console.WriteLine($"{s.StartDate} \t {s.AbonentFrom}=>{s.AbonentTo} \t {s.Duration:hh\\:mm\\:ss} \t {s.Amount}");
            }

            bool isAlive = true;

            while (isAlive)
            {
                Console.WriteLine(string.Empty);
      
                Console.WriteLine("Filters By:");
                Console.WriteLine($"1.Date");
                Console.WriteLine($"2.Amount");
                Console.WriteLine($"3.Port# ");
                Console.WriteLine($"4.Filter reset");
                Console.WriteLine("5.Exit");
               
              
                try
                {
                    int command = Convert.ToInt32(Console.ReadLine());

                    switch (command)
                    {
                        case 1:
                            ApplyFilter(history, Filter.FilterByDate);
                            break;
                        case 2:
                            ApplyFilter(history, Filter.FilterByAmount);
                            break;
                        case 3:
                            ApplyFilter(history, Filter.FilterByAbonent);
                            break;
                        case 4:
                            ApplyFilter(history, Filter.FilterReset);
                            break;
                        case 5:
                            isAlive = false;
                            return;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }


        private static void ApplyFilter(List<Call> history, Filter filter)
        {
            List<Call> result = new();

            if (!Enum.IsDefined(typeof(Filter), filter))
            {
                return;
            }

            switch (filter)
            {
                case Filter.FilterByDate:
                    Console.WriteLine("Insert Date:(dd.mm.yy):");
                    var readString = Console.ReadLine();
                    if (readString != null)
                    {

                        DateTime date = DateTime.ParseExact(readString.ToString(), "dd.MM.yy", CultureInfo.InvariantCulture);
                        result = history.FindAll(x => x.StartDate.Date == date.Date);
                    }
                    break;

                case Filter.FilterByAmount:
                    Console.WriteLine("Insert Amount:");
                    decimal amount = Convert.ToDecimal(Console.ReadLine());
                    result = history.FindAll(x => x.Amount == amount);
                    break;

                case Filter.FilterByAbonent:
                    Console.WriteLine("Insert # User:");
                    decimal number = Convert.ToDecimal(Console.ReadLine());
                    result = history.FindAll(x => x.AbonentFrom == number || x.AbonentTo == number);
                    break;

                case Filter.FilterReset:
                    result = history;
                    break;
            }

            if (result.Count == 0)
            {
                Console.WriteLine("Not Found");
            }

            foreach (var s in result)
            {
                Console.WriteLine($"{s.StartDate} \t {s.AbonentFrom}=>{s.AbonentTo} \t {s.Duration:hh\\:mm\\:ss} \t {s.Amount}");
            }


        }

        private static void GetBalance(User[] abonents)
        {
            if (abonents is null)
            {
                throw new ArgumentNullException(nameof(abonents));
            }


            Console.WriteLine("Port #");
            int number = Convert.ToInt32(Console.ReadLine());

            User abonent = abonents[number];


            int dialNumber = 120;
            abonent.Terminal.Dial(dialNumber);
            Console.ReadKey();
            abonent.Terminal.FinishDial();
            Console.ReadKey();

        }




        private static void MakeDial(User[] abonents)
        {
            Console.WriteLine("Port #");
            int number = Convert.ToInt32(Console.ReadLine());

            User abonent = abonents[number];

            Console.WriteLine("Enter a number to call:");
            int dialNumber = Convert.ToInt32(Console.ReadLine());
            abonent.Terminal.Dial(dialNumber);
            Console.ReadKey();
            abonent.Terminal.FinishDial();
            Console.ReadKey();
        }


        private static void SetTerminalToPorts(User[] abonents, bool isConnect)
        {
            Console.WriteLine("Port #");

            int number = Convert.ToInt32(Console.ReadLine());

            User currentAbonent = abonents[number];

            if (isConnect)
            {
                currentAbonent.Terminal.ConnectPort(currentAbonent.Port);
            }

            else
            {
                currentAbonent.Terminal.DisconnectPort();
            }
        }


        private static User[] CreateDogovors(Station ats, int count)

        {
            User[] abonents = new User[count];

            for (int i = 0; i < count; i++)
            {
                Contract contract = ats.CreateDogovor();
                Port port = ats.GetPort(contract);
                Terminal phone = ats.GetPhone();
                phone.Ringing += Phone_Ringing;
                abonents[i] = new User(contract, port, phone);

            }

            Console.WriteLine();

            ConnectTerminalsToPorts(abonents);

            return abonents;
        }


        private static void ConnectTerminalsToPorts(User[] abonents)
        {
            for (int i = 0; i < abonents.Length; i++)
            {
                abonents[i].Terminal.ConnectPort(abonents[i].Port);
            }
        }


        private static void Phone_Ringing(ITerminal sender, TerminalEventArgs e)
        {
            Console.WriteLine($"{sender.Name}: {e.Message}");
            Console.WriteLine($"Replay- 1, Reset - key pressed");

            int command = Convert.ToInt32(Console.ReadLine());
            bool answer = false;

            if (command == 1)
            {
                answer = true;
            }

            sender.SendAcceptCall(answer);
        }

    }
}
