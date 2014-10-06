using System.Security.Cryptography.X509Certificates;

namespace AutoCatalogLib.Exchange
{
    public interface IRule
    {
        string Identificator { get; set; }
        string Name { get; set; }
        string Description { get; set; }
    }
}
