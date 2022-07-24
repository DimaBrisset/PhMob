namespace PhMob
{
    public class UltraTariff : Tariff
    {
        public override string Name => @"""Ultra""";
        public override decimal Rate => 0.3M;
    }
}