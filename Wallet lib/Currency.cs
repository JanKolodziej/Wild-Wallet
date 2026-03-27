using SQLitePCL;

namespace Wallet_lib
{
    [Preserve]
    public class Currency
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Iso { get; set; }
        public string Flag { get; set; }
        public Currency() { }
        public Currency(string code, string name, string iso, string flag)
        {
            Code = code;
            Name = name;
            Iso = iso;
            Flag = flag;
        }
    }
}
