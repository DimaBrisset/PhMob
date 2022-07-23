namespace PhMob
{
    public delegate void TerminalStateHandler(ITerminal sender, TerminalEventArgs e);

    public class TerminalEventArgs : EventArgs
    {
        public string Message { get; }

        public TerminalEventArgs(string message)
        {
            Message = message;
        }
    }
}