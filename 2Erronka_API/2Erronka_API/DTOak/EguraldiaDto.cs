namespace _2Erronka_API.DTOak
{
    public sealed class EguraldiaDto
    {
        public string Udalerria { get; set; } = string.Empty;
        public string Probintzia { get; set; } = string.Empty;
        public List<EguraldiEgunaDto> Egunak { get; set; } = new();
    }
}
