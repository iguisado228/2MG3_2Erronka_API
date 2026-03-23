using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.DTOak;
using _2Erronka_API.Domain;
using _2Erronka_API.Repositorioak;
using System.Text;
using System.Security.Cryptography;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/odoo/langileak")]
    public class OdooLangileakController : ControllerBase
    {
        private readonly LangileaRepository _repo;

        public OdooLangileakController(LangileaRepository repo)
        {
            _repo = repo;
        }

        [HttpPost]
        public IActionResult Create([FromBody] OdooLangileaUpsertDto dto)
        {
            if (dto == null) return BadRequest();
            if (dto.LanpostuaId <= 0) return BadRequest("LanpostuaId beharrezkoa da.");

            string passwordHash = "";
            if (!string.IsNullOrWhiteSpace(dto.Pasahitza))
            {
                passwordHash = HashPassword(dto.Pasahitza);
            }
            else if (!string.IsNullOrWhiteSpace(dto.PasahitzaHash))
            {
                passwordHash = dto.PasahitzaHash;
            }

            var langilea = new Langilea
            {
                Izena = dto.Izena,
                Abizena = dto.Abizena,
                NAN = dto.NAN,
                Erabiltzaile_izena = dto.Erabiltzaile_izena,
                Langile_kodea = dto.Langile_kodea,
                Pasahitza = passwordHash,
                Helbidea = dto.Helbidea,
                Lanpostua = new Lanpostua { Id = dto.LanpostuaId }
            };

            _repo.Add(langilea);

            var response = new LangileaDto
            {
                Id = langilea.Id,
                Izena = langilea.Izena,
                Abizena = langilea.Abizena,
                NAN = langilea.NAN,
                Erabiltzaile_izena = langilea.Erabiltzaile_izena,
                Langile_kodea = langilea.Langile_kodea,
                Pasahitza = langilea.Pasahitza,
                Helbidea = langilea.Helbidea,
                Lanpostua = new LanpostuaDto { Id = dto.LanpostuaId, Lanpostu_izena = "" }
            };

            return Ok(response);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] OdooLangileaUpsertDto dto)
        {
            if (dto == null) return BadRequest();
            if (dto.LanpostuaId <= 0) return BadRequest("LanpostuaId beharrezkoa da.");

            var langilea = _repo.Get(id);
            if (langilea == null) return NotFound();

            langilea.Izena = dto.Izena;
            langilea.Abizena = dto.Abizena;
            langilea.NAN = dto.NAN;
            langilea.Erabiltzaile_izena = dto.Erabiltzaile_izena;
            langilea.Langile_kodea = dto.Langile_kodea;
            if (!string.IsNullOrWhiteSpace(dto.Pasahitza))
            {
                langilea.Pasahitza = HashPassword(dto.Pasahitza);
            }
            else if (!string.IsNullOrWhiteSpace(dto.PasahitzaHash))
            {
                langilea.Pasahitza = dto.PasahitzaHash;
            }
            langilea.Helbidea = dto.Helbidea;
            langilea.Lanpostua = new Lanpostua { Id = dto.LanpostuaId };

            _repo.Update(langilea);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var langilea = _repo.Get(id);
            if (langilea == null) return NotFound();

            _repo.Delete(langilea);
            return NoContent();
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
