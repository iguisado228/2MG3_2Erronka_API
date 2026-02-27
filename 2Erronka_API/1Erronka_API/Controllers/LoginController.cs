using _1Erronka_API;
using _1Erronka_API.Domain;
using _1Erronka_API.DTOak;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

/// <summary>
/// Autentifikazioaren (login) API amaierako puntuak.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    /// <summary>
    /// Langile baten kredentzialak balioztatzen ditu eta loginaren emaitza itzultzen du.
    /// </summary>
    /// <param name="request">Langile-kodea eta pasahitza dituen eskaera.</param>
    /// <returns>200 OK erantzuna, LoginErantzuna egiturarekin.</returns>
    [HttpPost]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        using (var session = NHibernateHelper.OpenSession())
        {
            var langilea = session.Query<Langilea>()
                .FirstOrDefault(u => u.Langile_kodea == request.Langile_kodea);

            if (langilea == null)
            {
                return Ok(new LoginErantzuna
                {
                    Ok = false,
                    Code = "not_found",
                    Message = "Langilea ez da existitzen"
                });
            }

            string pasahitzaHash = HashPassword(request.Pasahitza);

            if (langilea.Pasahitza != pasahitzaHash)
            {
                return Ok(new LoginErantzuna
                {
                    Ok = false,
                    Code = "bad_password",
                    Message = "Pasahitza okerra da"
                });
            }

            if (!langilea.TpvSarrera)
            {
                return Ok(new LoginErantzuna
                {
                    Ok = false,
                    Code = "forbidden",
                    Message = "Ez daukazu TPV-ra sartzeko baimenik"
                });
            }

            return Ok(new LoginErantzuna
            {
                Ok = true,
                Code = "ok",
                Message = "Login zuzena",
                Data = new LangileaDto
                {
                    Id = langilea.Id,
                    Izena = langilea.Izena,
                    Erabiltzaile_izena = langilea.Erabiltzaile_izena,
                    Langile_kodea = langilea.Langile_kodea,
                    Gerentea = langilea.Gerentea,
                    TpvSarrera = langilea.TpvSarrera
                }
            });
        }
    }

    private string HashPassword(string input)
    {
        using (SHA256 sha = SHA256.Create())
        {
            byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}

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