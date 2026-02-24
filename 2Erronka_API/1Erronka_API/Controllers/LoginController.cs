using _1Erronka_API;
using _1Erronka_API.Domain;
using _1Erronka_API.DTOak;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
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

public class LoginRequest
{
    public int Langile_kodea { get; set; }
    public string Pasahitza { get; set; }
}

public class LoginErantzuna
{
    public bool Ok { get; set; }
    public string Code { get; set; } = "";
    public string Message { get; set; } = "";
    public LangileaDto? Data { get; set; }
}
