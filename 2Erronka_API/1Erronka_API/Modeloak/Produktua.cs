namespace _1Erronka_API.Modeloak
{
    public class Produktua
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; } = string.Empty;
        public virtual double Prezioa { get; set; }
        public virtual string Mota { get; set; } = string.Empty;
        public virtual int Stock { get; set; }

        public virtual IList<ProduktuaOsagaia> Osagaiak { get; set; } = new List<ProduktuaOsagaia>();
    }
}
