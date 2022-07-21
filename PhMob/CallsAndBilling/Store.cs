namespace PhMob
{

    internal class Store
    {
        private readonly Random _random = new();

        private readonly string[] _phoneModels =
        {
            "Phone1", "Phone2", "Phone3","Phone4","Phone5",
        };
        internal Phone GetPhone()
        {

            int index = _random.Next(0, _phoneModels.Length);
            return new Phone(_phoneModels[index]);
        }
    }
}
