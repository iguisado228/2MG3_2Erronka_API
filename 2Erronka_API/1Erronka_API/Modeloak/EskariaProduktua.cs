namespace _1Erronka_API.Modeloak
{
    public class EskariaProduktua
    {
        public virtual Eskaria Eskaria { get; set; }
        public virtual Produktua Produktua { get; set; }
        public virtual int Kantitatea { get; set; }
        public virtual double Prezioa { get; set; }

        public override bool Equals(object? obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;

            var other = (EskariaProduktua)obj;
            return Eskaria.Id == other.Eskaria.Id && Produktua.Id == other.Produktua.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Eskaria.Id, Produktua.Id);
        }
    }
}
