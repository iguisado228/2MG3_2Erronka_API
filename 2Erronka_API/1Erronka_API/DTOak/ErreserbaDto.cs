namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Erreserba baten datu-laburpena, API erantzunetan erabiltzeko.
    /// </summary>
    public class ErreserbaDto
    {
        /// <summary>Erreserbaren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Bezeroaren izena.</summary>
        public string BezeroIzena { get; set; }
        /// <summary>Bezeroaren telefonoa.</summary>
        public string Telefonoa { get; set; }
        /// <summary>Erreserban dagoen pertsona kopurua.</summary>
        public int PertsonaKopurua { get; set; }
        /// <summary>Erreserbaren eguna eta ordua.</summary>
        public DateTime EgunaOrdua { get; set; }
        /// <summary>Erreserbaren prezio totala.</summary>
        public double PrezioTotala { get; set; }
        /// <summary>Ordainduta dagoen adierazlea (0/1).</summary>
        public int Ordainduta { get; set; } = 0;
        /// <summary>Tiketa/fakturaren bidea (ruta), eskuragarri badago.</summary>
        public string FakturaRuta { get; set; }
        /// <summary>Langilearen identifikatzailea.</summary>
        public int LangileaId { get; set; }
        /// <summary>Langilearen izena.</summary>
        public string LangileaIzena { get; set; }
        /// <summary>Mahaiaren identifikatzailea.</summary>
        public int MahaiakId { get; set; }
    }
 
    /// <summary>
    /// Erreserba berria sortu edo eguneratzeko erabiltzen den sarrera-DTOa.
    /// </summary>
    public class ErreserbaSortuDto
    {
        /// <summary>Bezeroaren izena.</summary>
        public string BezeroIzena { get; set; } = string.Empty;
        /// <summary>Bezeroaren telefonoa.</summary>
        public string Telefonoa { get; set; } = string.Empty;
        /// <summary>Erreserban dagoen pertsona kopurua.</summary>
        public int PertsonaKopurua { get; set; }
        /// <summary>Erreserbaren eguna eta ordua.</summary>
        public DateTime EgunaOrdua { get; set; }
        /// <summary>Erreserbaren prezio totala.</summary>
        public double PrezioTotala { get; set; }
        /// <summary>Tiketa/fakturaren bidea (ruta).</summary>
        public string FakturaRuta { get; set; } = string.Empty;
        /// <summary>Erreserba kudeatzen duen langilearen identifikatzailea.</summary>
        public int LangileaId { get; set; }
        /// <summary>Erreserbarako aukeratutako mahaiaren identifikatzailea.</summary>
        public int MahaiakId { get; set; }
    }
}
 