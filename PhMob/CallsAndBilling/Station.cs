using System.Data;

namespace PhMob
{
    public class Station
    {
        public string Name { get; }

        private static readonly Random Random = new();
        private readonly Random _random = Random;
        private readonly Store _store = new();
        private (bool, Port) _acceptedCall;
        private readonly Billing _billing = new();
        private readonly Port[] _ports = new Port[30];
        private readonly List<Contract> _contracts = new();
        private readonly Dictionary<Contract, Port> _contractMap = new();
        private readonly Dictionary<int, Action<Contract>> _systemMethods = new();

        public Station(string name)
        {
            Name = name;
            Console.WriteLine($"Station name:{Name}");
            InitializePorts();
            InitializeSystemNumbers();
        }

        public Contract CreateDogovor()
        {
            int contractNumber = _contracts.Count;
            Contract contract = new(contractNumber, RandomTariff());
            Port port = _ports.First(x => x.Status == PortStatus.Free);

            if (port == null)
            {
                throw new Exception(message: "Did not free ports");
            }

            port.PortStatusChange(PortStatus.Disconnected);
            int portNumber = Array.IndexOf(_ports, port);
            port.SetAbonentNumber(contractNumber, portNumber, 37500 + portNumber);
            _contracts.Add(contract);
            _contractMap.Add(contract, port);
            return contract;
        }

        public Phone GetPhone()
        {
            return _store.GetPhone();
        }

        public void CountDebts()
        {
            _billing.CountDebtsForAbonents(_contracts);
        }

        public Port GetPort(Contract contract)
        {
            Port port = _contractMap[contract];
            return port;
        }

        public List<Call> GetHistory(int abonentNumber)
        {
            return _billing.GetHistory(abonentNumber);
        }

        public void AddMoney(Contract contract, decimal amount)
        {
            Port port = _contractMap[contract];
            Console.Write($"{port.AbonentNumber}: ");
            contract.PayBills(amount);
        }

        private void InitializePorts()
        {
            for (int i = 0; i < _ports.Length; i++)
            {
                _ports[i] = new Port();
                _ports[i].PortConnected += Station_PortConnected;
                _ports[i].PortDisconnected += Station_PortDisconnected;

                Thread.Sleep(50);
            }

            Console.WriteLine();
        }

        private void InitializeSystemNumbers()
        {
            _systemMethods.Add(120, ShowBalance);
        }

        private void ShowBalance(Contract contract)
        {
            Port port = _contractMap[contract];
            decimal balance = contract.Balance;
            Console.WriteLine($"Balance {port.AbonentNumber}: {balance} ");
            Console.WriteLine($"Debit is: {contract.Debt} ");
            Console.WriteLine($"Tariff: {contract.Tariff.Name} ( {contract.Tariff.Rate})");
            Console.WriteLine(
                $"The cost of services for the current month is calculated {Billing.LastPayDay} date of next month");
        }

        private void Station_PortConnected(Port sender, PortEventArgs e)
        {
            sender.OutcomeCall += Sender_OutcomeCall;
            sender.CallAccepted += Sender_CallAccepted;
            Console.WriteLine(e.Message);
            sender.PortStatusChange(PortStatus.Connected);
        }

        private void Station_PortDisconnected(Port sender, PortEventArgs e)
        {
            sender.OutcomeCall -= Sender_OutcomeCall;
            sender.CallAccepted -= Sender_CallAccepted;
            Console.WriteLine(e.Message);
            sender.PortStatusChange(PortStatus.Disconnected);
        }

        private void Sender_CallAccepted(Port sender, bool accept)
        {
            if (accept)
            {
                sender.PortStatusChange(PortStatus.Busy);
            }

            _acceptedCall = (accept, sender);
        }

        private void Sender_OutcomeCall(Port sender, PortEventArgs e)
        {
            Contract? dogovor = _contracts.FirstOrDefault(x => x.ContractNumber == sender.ContractNumber);

            if (dogovor != null)
            {
                if (_systemMethods.ContainsKey(e.AbonentNumber))
                {
                    _systemMethods[e.AbonentNumber](dogovor);
                    return;
                }

                bool isPaid = Billing.IsBillsPaid(dogovor.Debt);

                if (!isPaid)
                {
                    Console.WriteLine($"Pay off your debt {dogovor.Debt}");
                    return;
                }
            }

            Port? calledPort = _ports.FirstOrDefault(x => x.AbonentNumber == e.AbonentNumber);


            if (calledPort == null)
            {
                Console.WriteLine("Not correct phone number");
                return;
            }


            if (calledPort.Status == PortStatus.Busy)
            {
                Console.WriteLine("Subscriber you called is busy");
                return;
            }


            if (calledPort.Status == PortStatus.Disconnected)
            {
                Console.WriteLine("Subscriber you called is not available");
                return;
            }

            Console.WriteLine("Calling...");


            calledPort.IncomeCalling(sender);


            if (_acceptedCall.Item1 & _acceptedCall.Item2 == calledPort)
            {
                Console.WriteLine($"Call accepted by subscriber{calledPort.AbonentNumber} ");
                Connection(sender, calledPort);
            }

            else
            {
                Console.WriteLine($"Call rejected by subscriber {calledPort.AbonentNumber}");
            }
        }

        private async void Connection(Port callingPort, Port answerPort)
        {
            Console.WriteLine("Started chat");
            DateTime timeStart = DateTime.Now;
            await Task.Run(() => Talking(callingPort, answerPort));


            FinishDialog(callingPort, answerPort, timeStart);
        }

        private static void Talking(Port call, Port answer)
        {
            int i = 1;

            call.CancelTokenSource = new CancellationTokenSource();

            answer.CancelTokenSource = new CancellationTokenSource();

            while (true)
            {
                if (call.CancelTokenSource.Token.IsCancellationRequested || call.Status == PortStatus.Disconnected)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Call ended {call.AbonentNumber}");
                    return;
                }

                if (answer.CancelTokenSource.Token.IsCancellationRequested || answer.Status == PortStatus.Disconnected)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Call ended{answer.AbonentNumber}");
                    return;
                }

                TimeSpan time = new(0, 0, i);
                Console.Write(time);
                Thread.Sleep(100);
                i++;
                Console.CursorLeft = 0;
            }
        }

        private void FinishDialog(Port callingPort, Port answerPort, DateTime timeStart)
        {
            DateTime timeFinish = DateTime.Now;


            callingPort.CancelTokenSource = null;
            answerPort.CancelTokenSource = null;


            if (callingPort.Status == PortStatus.Busy)
            {
                callingPort.PortStatusChange(PortStatus.Connected);
            }

            if (answerPort.Status == PortStatus.Busy)
            {
                answerPort.PortStatusChange(PortStatus.Connected);
            }


            Contract? contract = _contracts.FirstOrDefault(x => x.ContractNumber == callingPort.ContractNumber);

            if (contract == null)
                return;
            Tariff tariff = contract.Tariff;

            decimal amount = Billing.GetCallPrice(tariff, timeFinish - timeStart);
            amount = Math.Round(amount, 2);
            Call call = new(contract.ContractNumber, tariff, timeStart, timeFinish, callingPort.AbonentNumber,
                answerPort.AbonentNumber, amount);
            _billing.AddCallToJournal(call);
            Console.WriteLine($"Call was finish. Cost {amount}");
        }

        private Tariff RandomTariff()
        {
            Type baseType;
            baseType = typeof(Tariff);
            Type[]? allDerivedTypes = baseType.Assembly.ExportedTypes.Where(t => baseType.IsAssignableFrom(t))
                .Where(t => t.IsAbstract == false).ToArray();


            Tariff? tariff =
                Activator.CreateInstance(
                    Type.GetType(allDerivedTypes[_random.Next(0, allDerivedTypes.Length)].FullName) ) as Tariff;

            
                return tariff;
            
            
        }
    }
}