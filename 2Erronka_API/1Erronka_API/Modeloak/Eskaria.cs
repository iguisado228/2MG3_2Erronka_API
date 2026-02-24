using _1Erronka_API.Domain;

namespace _1Erronka_API.Modeloak
{
    public class Eskaria
    {
        public virtual int Id { get; set; }
        public virtual double Prezioa { get; set; }
        public virtual string Egoera { get; set; } = string.Empty;
        public virtual Erreserba Erreserba { get; set; }
        public virtual Langilea Langilea { get; set; }
        public virtual Mahaia Mahaia { get; set; }
        public virtual IList<EskariaProduktua> Produktuak { get; set; } = new List<EskariaProduktua>();
    }
}
