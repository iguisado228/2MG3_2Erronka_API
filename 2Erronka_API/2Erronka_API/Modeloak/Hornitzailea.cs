namespace _2Erronka_API.Modeloak
{
    public class Hornitzailea
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; } = string.Empty;
        public virtual string Kontaktua { get; set; } = string.Empty;
        public virtual string Helbidea { get; set; } = string.Empty;
    }
}
