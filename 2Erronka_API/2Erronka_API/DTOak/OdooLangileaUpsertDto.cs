namespace _2Erronka_API.DTOak
{
    public class OdooLangileaUpsertDto
    {
        public int? Id { get; set; }
        public string Izena { get; set; }
        public string Abizena { get; set; }
        public string NAN { get; set; }
        public string Erabiltzaile_izena { get; set; }
        public int Langile_kodea { get; set; }
        public string? Pasahitza { get; set; }
        public string? PasahitzaHash { get; set; }
        public string Helbidea { get; set; }
        public int LanpostuaId { get; set; }
    }
}
