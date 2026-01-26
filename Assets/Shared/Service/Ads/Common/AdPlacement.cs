namespace Shared.Service.Ads.Common
{
    public class AdPlacement : IAdPlacement
    {
        private const string Tag = "AdPlacement";
        public string Name { get; }

        public AdPlacement(string name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return $"[{Tag} Name={Name}]";
        }
    }
}