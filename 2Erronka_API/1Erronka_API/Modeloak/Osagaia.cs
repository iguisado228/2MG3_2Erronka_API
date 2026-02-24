namespace _1Erronka_API.Modeloak
{
    public class Osagaia
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; } = string.Empty;
        public virtual double Prezioa { get; set; }
        public virtual int Stock { get; set; }
        public virtual int HornitzaileakId { get; set; }
    }
}
