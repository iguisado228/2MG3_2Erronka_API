namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Login egiteko behar diren datuak.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>Langilearen kodea.</summary>
        public int Langile_kodea { get; set; }
        /// <summary>Langilearen pasahitza (testu arrunta).</summary>
        public string Pasahitza { get; set; }
    }
}
