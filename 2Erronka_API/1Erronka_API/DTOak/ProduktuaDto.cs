namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Produktu baten datu-laburpena, API erantzunetan erabiltzeko.
    /// </summary>
    public class ProduktuaDto
    {
        /// <summary>Produktuaren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Produktuaren izena.</summary>
        public string Izena { get; set; } = string.Empty;
        /// <summary>Produktuaren prezioa.</summary>
        public double Prezioa { get; set; }
        /// <summary>Produktuaren mota edo kategoria.</summary>
        public string Mota { get; set; } = string.Empty;
        /// <summary>Produktuaren stock-kantitatea.</summary>
        public int Stock { get; set; }
    }

    /// <summary>
    /// Osagai baten datu-laburpena, API erantzunetan erabiltzeko.
    /// </summary>
    public class OsagaiaDto
    {
        /// <summary>Osagaiaren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Osagaiaren izena.</summary>
        public string Izena { get; set; } = string.Empty;
        /// <summary>Osagaiaren prezioa.</summary>
        public double Prezioa { get; set; }
        /// <summary>Osagaiaren stock-kantitatea.</summary>
        public int Stock { get; set; }
        /// <summary>Hornitzailearen identifikatzailea.</summary>
        public int HornitzaileakId { get; set; }
    }
}
