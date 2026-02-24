namespace _1Erronka_API.Modeloak
{
    public class ProduktuaOsagaia
    {
        public virtual Produktua Produktua { get; set; }
        public virtual Osagaia Osagaia { get; set; }

        public virtual int Kantitatea { get; set; } 

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (ProduktuaOsagaia)obj;
            return Produktua.Id == other.Produktua.Id && Osagaia.Id == other.Osagaia.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Produktua.Id, Osagaia.Id);
        }

    }
}
