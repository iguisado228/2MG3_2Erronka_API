namespace _1Erronka_API.DTOak
{
    public class LangileaDto
    {
        public virtual int Id { get; set; }
        public virtual string Izena { get; set; }
        public virtual string Erabiltzaile_izena { get; set; }
        public int Langile_kodea { get; set; }
        public virtual string Pasahitza { get; set; }
        public virtual Boolean Gerentea { get; set; }
        public virtual Boolean TpvSarrera { get; set; }
    }
}
