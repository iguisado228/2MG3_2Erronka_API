namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Eskari baten datu-laburpena, API erantzunetan erabiltzeko.
    /// </summary>
    public class EskariaDto
    {
        /// <summary>Eskariaren identifikatzailea.</summary>
        public int Id { get; set; }
        /// <summary>Eskariaren prezio totala.</summary>
        public double Prezioa { get; set; }
        /// <summary>Eskariaren egoera (adib. prestatzen, zerbitzatuta...).</summary>
        public string Egoera { get; set; } = string.Empty;
        /// <summary>Lotutako erreserbaren identifikatzailea.</summary>
        public int ErreserbaId { get; set; }
        /// <summary>Lotutako mahaiaren zenbakia.</summary>
        public int MahaiaZenbakia { get; set; }
        /// <summary>Eskariaren produktu-zerrenda.</summary>
        public List<EskariaProduktuaDto> Produktuak { get; set; } = new();
    }

    /// <summary>
    /// Eskari berria sortu edo eguneratzeko erabiltzen den sarrera-DTOa.
    /// </summary>
    public class EskariaSortuDto
    {
        /// <summary>Lotutako erreserbaren identifikatzailea.</summary>
        public int ErreserbaId { get; set; }
        /// <summary>Eskariaren prezioa (zerbitzuak kalkulatu dezake).</summary>
        public double Prezioa { get; set; }
        /// <summary>Eskariaren egoera.</summary>
        public string Egoera { get; set; } = string.Empty;
        /// <summary>Eskarian sartutako produktuen zerrenda.</summary>
        public List<EskariaProduktuaSortuDto> Produktuak { get; set; } = new();
    }

    /// <summary>
    /// Eskari bateko produktu baten erantzun-eredua.
    /// </summary>
    public class EskariaProduktuaDto
    {
        /// <summary>Produktuaren identifikatzailea.</summary>
        public int ProduktuaId { get; set; }
        /// <summary>Produktuaren izena.</summary>
        public string ProduktuaIzena { get; set; } = string.Empty;
        /// <summary>Eskarian eskatutako kantitatea.</summary>
        public int Kantitatea { get; set; }
        /// <summary>Produktuaren unitateko prezioa.</summary>
        public double Prezioa { get; set; }
    }

    /// <summary>
    /// Eskari batean produktu bat gehitzeko erabiltzen den sarrera-eredua.
    /// </summary>
    public class EskariaProduktuaSortuDto
    {
        /// <summary>Produktuaren identifikatzailea.</summary>
        public int ProduktuaId { get; set; }
        /// <summary>Eskarian eskatutako kantitatea.</summary>
        public int Kantitatea { get; set; }
        /// <summary>Produktuaren prezioa (zerbitzuak gainidatzi dezake).</summary>
        public double Prezioa { get; set; }
    }
}