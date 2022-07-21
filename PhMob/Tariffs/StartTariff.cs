namespace PhMob
{
    public class StartTariff : Tariff
    {
        public override string Name => @"""Start""";
        public override decimal Rate => 0.01M;
    }
}
