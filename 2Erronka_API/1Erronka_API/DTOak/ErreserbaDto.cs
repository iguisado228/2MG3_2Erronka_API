namespace _1Erronka_API.DTOak
{
    public class ErreserbaDto
    {
        public int Id { get; set; }
        public string BezeroIzena { get; set; }
        public string Telefonoa { get; set; }
        public int PertsonaKopurua { get; set; }
        public DateTime EgunaOrdua { get; set; }
        public double PrezioTotala { get; set; }
        public int Ordainduta { get; set; } = 0;
        public string FakturaRuta { get; set; }
        public int LangileaId { get; set; }
        public string LangileaIzena { get; set; }
        public int MahaiakId { get; set; }
    }

    public class ErreserbaSortuDto
    {
        public string BezeroIzena { get; set; } = string.Empty;
        public string Telefonoa { get; set; } = string.Empty;
        public int PertsonaKopurua { get; set; }
        public DateTime EgunaOrdua { get; set; } 
        public double PrezioTotala { get; set; }
        public string FakturaRuta { get; set; } = string.Empty;
        public int LangileaId { get; set; }
        public int MahaiakId { get; set; }
    }
}
