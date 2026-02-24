namespace _1Erronka_API.DTOak
{
    public class EskariaDto
    {
        public int Id { get; set; }
        public double Prezioa { get; set; }
        public string Egoera { get; set; } = string.Empty;
        public int ErreserbaId { get; set; }
        public int MahaiaZenbakia { get; set; }
        public List<EskariaProduktuaDto> Produktuak { get; set; } = new();
    }

    public class EskariaSortuDto
    {
        public int ErreserbaId { get; set; }
        public double Prezioa { get; set; }
        public string Egoera { get; set; } = string.Empty;
        public List<EskariaProduktuaSortuDto> Produktuak { get; set; } = new();
    }

    public class EskariaProduktuaDto
    {
        public int ProduktuaId { get; set; }
        public string ProduktuaIzena { get; set; } = string.Empty;
        public int Kantitatea { get; set; }
        public double Prezioa { get; set; }
    }

    public class EskariaProduktuaSortuDto
    {
        public int ProduktuaId { get; set; }
        public int Kantitatea { get; set; }
        public double Prezioa { get; set; }
    }
}
