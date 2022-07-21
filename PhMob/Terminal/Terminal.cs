namespace PhMob
{
    public class Terminal : ITerminal
    {
        public string Name { get; }
        private Port? _port;
        public event TerminalStateHandler? Ringing;

        internal Terminal(string model)
        {
            Name = model.ToString();
        }
   
        public void ConnectPort(Port port)
        {
            if (_port != null)
            {
                Console.WriteLine($"Your device is already connected!");
                return;
            }

           
            _port = port;
            _port.ConnectTerminal(this);
            _port.RingNotify += Port_RingNotify;
        }

        public void DisconnectPort()
        {
            Console.WriteLine($"Disconnect request from {Name}");

            if (_port == null)
            {
                Console.WriteLine($"The device is not connected to the port!");
                return;
            }

            _port.DisconnectTerminal(this);
            _port.RingNotify -= Port_RingNotify;
            _port = null;
        }

        public void Dial(int number)
        {
            if (_port == null || _port.Status == PortStatus.Disconnected)
            {
                Console.WriteLine("The device is not connected to the port!");
                return;
            }
            _port.OutcomeCalling(number);
        }

        public void FinishDial()
        {
            if (_port != null)
            {
                _port.FinishTalking();

            }
        }

        public void SendAcceptCall(bool accepted)
        {
            if (_port != null)
            {
                _port.SendAccept(accepted);
            }
        }

        private void Port_RingNotify(Port sender, int number)
        {
            Ringing?.Invoke(this, new TerminalEventArgs($"Incoming call from subscriber {number}"));
        }
    }
}
