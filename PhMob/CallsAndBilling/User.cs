namespace PhMob
{
    public class User
    {
        public Contract Contract { get; }
        public Port Port { get; }
        public Terminal Terminal { get; }
        public User(Contract contract, Port port, Terminal terminal)
        {
            Contract = contract;
            Port = port;
            Terminal = terminal;
        }
    }
}
