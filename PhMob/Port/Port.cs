namespace PhMob
{
    public class Port
    {
   
        public delegate void RingAcceptHandle(Port sender, int abonentNumber);

        public event RingAcceptHandle ?RingNotify;
        public int AbonentNumber { get; private set; }       
        public PortStatus Status { get; private set; }
        internal int PortNumber { get; private set; }
        internal int ContractNumber { get; private set; }
        internal CancellationTokenSource? CancelTokenSource { get; set; }

        internal delegate void СallAcceptHandle(Port sender, bool accept);
        internal event PortStateHandler ?PortConnected;
        internal event PortStateHandler ?PortDisconnected;
        internal event PortStateHandler ?OutcomeCall;
        internal event СallAcceptHandle? CallAccepted;

           

        internal Port()
        {
            Status = PortStatus.Free;
        }


        internal void ConnectTerminal(ITerminal terminal)
        {
            PortConnected?.Invoke(this, new PortEventArgs($"Port #{PortNumber}:  Terminal {terminal.Name} Connect with user Number: {AbonentNumber}"));
        }

        internal void DisconnectTerminal(ITerminal terminal)
        {
            PortDisconnected?.Invoke(this, new PortEventArgs($"Port #{PortNumber}:  Terminal {terminal.Name} DisConnect with user Number:  {AbonentNumber}"));
        }

        internal void IncomeCalling(Port port)
        {
       
            RingNotify?.Invoke(this, port.AbonentNumber);
        }

        internal void OutcomeCalling(int number)
        {
            if (AbonentNumber == number)
            {
                Console.WriteLine("Not Call to himself");
                return;
            }

      
            OutcomeCall?.Invoke(this, new PortEventArgs(number));
        }

        internal void SendAccept(bool accepted)
        {
            CallAccepted?.Invoke(this, accepted);
        }

        internal void FinishTalking()
        {
            CancelTokenSource?.Cancel();
        }
               
        internal void PortStatusChange(PortStatus status)
        {
            if (Enum.IsDefined(typeof(PortStatus), status))
            {
                Status = status;
            }
        }
           
        internal void SetAbonentNumber(int contractNumber, int portNumber, int abonentNumber)
        {
            ContractNumber = contractNumber;
            PortNumber = portNumber;
            AbonentNumber = abonentNumber;
        }
    }
}
