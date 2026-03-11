using _2Erronka_API.Domain;
using _2Erronka_API.DTOak;
using _2Erronka_API.Repositorioak;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Security.Cryptography;

namespace _2Erronka_API.Controllers
{

    /// <summary>
    /// Autentifikazioaren (login) API amaierako puntuak.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class LoginController : ControllerBase
    {
        private readonly LangileaRepository _langileaRepository;

        public LoginController(LangileaRepository langileaRepository)
        {
            _langileaRepository = langileaRepository;
        }

        /// <summary>
        /// Langile baten kredentzialak balioztatzen ditu eta loginaren emaitza itzultzen du.
        /// </summary>
        /// <param name="request">Langile-kodea eta pasahitza dituen eskaera.</param>
        /// <returns>200 OK erantzuna, LoginErantzuna egiturarekin.</returns>
        [HttpPost]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var langilea = _langileaRepository.GetByKodea(request.Langile_kodea);

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

            // Lanpostuaren izenaren arabera baimenak egiaztatu
            // Adibidez: "Administratzailea" edo "Zerbitzaria" baimena dute
            var baimendutakoLanpostuak = new[] { "Administratzailea", "Zerbitzaria", "Gerentea" };

            if (!baimendutakoLanpostuak.Contains(langilea.Lanpostua.Lanpostu_izena))
            {
                return Ok(new LoginErantzuna
                {
                    Ok = false,
                    Code = "forbidden",
                    Message = "Zure lanpostuak ez dauka TPV-ra sartzeko baimenik"
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
                    Abizena = langilea.Abizena,
                    NAN = langilea.NAN,
                    Erabiltzaile_izena = langilea.Erabiltzaile_izena,
                    Langile_kodea = langilea.Langile_kodea,
                    Helbidea = langilea.Helbidea,
                    Lanpostua = new LanpostuaDto
                    {
                        Id = langilea.Lanpostua.Id,
                        Lanpostu_izena = langilea.Lanpostua.Lanpostu_izena
                    }
                }
            });
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
}