namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Mahai baten datu-laburpena, API erantzunetan eta sarreretan erabiltzeko.
    /// </summary>
    public class MahaiaDto
    {
        /// <summary>Mahaiaren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Mahaiaren zenbakia.</summary>
        public int Zenbakia { get; set; }
        /// <summary>Mahaiak onartzen duen pertsona kopurua.</summary>
        public int PertsonaKopurua { get; set; }
        /// <summary>Mahaiaren kokapena (adib. barruan, terrazan...).</summary>
        public string Kokapena { get; set; } = string.Empty;
    }
}
