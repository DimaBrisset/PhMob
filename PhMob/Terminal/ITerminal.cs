namespace PhMob
{
    public interface ITerminal
    {
        string Name { get; }
        void ConnectPort(Port port);
        void DisconnectPort();
        void Dial(int number);
        void FinishDial();
        void SendAcceptCall(bool accepted);
    }
}
