namespace PhMob
{
    public class StandardTariff : Tariff
    {
        public override string Name => @"""Standard""";
        public override decimal Rate => 0.05m;
    }
}