namespace PhMob
{
    public delegate void PortStateHandler(Port sender, PortEventArgs e);

    public class PortEventArgs : EventArgs
    {
        public string? Message { get; private set; }
        public int AbonentNumber { get; private set; }

        public PortEventArgs(string message)
        {
            Message = message;
        }

        public PortEventArgs(int abonentNumber)
        {
            AbonentNumber = abonentNumber;
        }
    }
}