using _1Erronka_API.DTOak;

namespace _1Erronka_API.DTOak
{
    /// <summary>
    /// Loginaren erantzun estandarra.
    /// </summary>
    public class LoginErantzuna
    {
        /// <summary>Operazioa ongi joan den adierazlea.</summary>
        public bool Ok { get; set; }
        /// <summary>Errore- edo egoera-kode laburra.</summary>
        public string Code { get; set; } = "";
        /// <summary>Erabiltzaileari erakusteko mezua.</summary>
        public string Message { get; set; } = "";
        /// <summary>Login zuzena denean, langilearen datu laburrak.</summary>
        public LangileaDto? Data { get; set; }
    }
}
