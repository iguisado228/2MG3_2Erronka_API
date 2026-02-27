namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Erreserba baten ordainketa erregistratzeko datuak.
    /// </summary>
    public class ErreserbaOrdainduDto
    {
        /// <summary>Erreserbaren identifikatzailea.</summary>
        public int ErreserbaId { get; set; }
        /// <summary>Ordaindu beharreko guztizko zenbatekoa.</summary>
        public double Guztira { get; set; }
        /// <summary>Bezeroak emandako zenbatekoa.</summary>
        public double Jasotakoa { get; set; }
        /// <summary>Bezeroari itzultzeko zenbatekoa.</summary>
        public double Itzulia { get; set; }
        /// <summary>Ordainketa kudeatzen duen langilearen identifikatzailea.</summary>
        public int LangileaId { get; set; }
        /// <summary>Ordainketa modua (adib. "Eskudirua", "Txartela").</summary>
        public string OrdainketaModua { get; set; } = string.Empty;
    }
}
 