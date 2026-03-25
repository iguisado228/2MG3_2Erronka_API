using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.DTOak;
using _2Erronka_API.Domain;
using System.Text;
using System.Security.Cryptography;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LangileakController : ControllerBase
    {
        private readonly LangileaRepository _repo;

        public LangileakController(LangileaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var langileak = _repo.GetAll();
            var dtoList = new List<LangileaDto>();
            foreach (var l in langileak)
            {
                dtoList.Add(new LangileaDto
                {
                    Id = l.Id,
                    Izena = l.Izena,
                    Abizena = l.Abizena,
                    NAN = l.NAN,
                    Erabiltzaile_izena = l.Erabiltzaile_izena,
                    Langile_kodea = l.Langile_kodea,
                    Helbidea = l.Helbidea,
                    Lanpostua = new LanpostuaDto
                    {
                        Id = l.Lanpostua?.Id ?? 0,
                        Lanpostu_izena = l.Lanpostua?.Lanpostu_izena ?? ""
                    }
                });
            }

            return Ok(dtoList);
        }

        [HttpGet("lanpostua/{lanpostuaId}")]
        public IActionResult GetByLanpostua(int lanpostuaId)
        {
            var langileak = _repo.GetByLanpostuaId(lanpostuaId);
            var dtoList = new List<LangileaDto>();
            foreach (var l in langileak)
            {
                dtoList.Add(new LangileaDto
                {
                    Id = l.Id,
                    Izena = l.Izena,
                    Abizena = l.Abizena,
                    NAN = l.NAN,
                    Erabiltzaile_izena = l.Erabiltzaile_izena,
                    Langile_kodea = l.Langile_kodea,
                    Helbidea = l.Helbidea,
                    Lanpostua = new LanpostuaDto
                    {
                        Id = l.Lanpostua?.Id ?? 0,
                        Lanpostu_izena = l.Lanpostua?.Lanpostu_izena ?? ""
                    }
                });
            }
            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var l = _repo.Get(id);
            if (l == null) return NotFound();

            var dto = new LangileaDto
            {
                Id = l.Id,
                Izena = l.Izena,
                Abizena = l.Abizena,
                NAN = l.NAN,
                Erabiltzaile_izena = l.Erabiltzaile_izena,
                Langile_kodea = l.Langile_kodea,
                Helbidea = l.Helbidea,
                Lanpostua = new LanpostuaDto
                {
                    Id = l.Lanpostua?.Id ?? 0,
                    Lanpostu_izena = l.Lanpostua?.Lanpostu_izena ?? ""
                }
            };

            return Ok(dto);
        }

        [HttpPost]
        public IActionResult Create([FromBody] LangileaDto dto)
        {
            if (dto == null) return BadRequest();
            if (dto.Lanpostua == null) return BadRequest("Lanpostua beharrezkoa da.");
            if (string.IsNullOrWhiteSpace(dto.Pasahitza)) return BadRequest("Pasahitza beharrezkoa da.");

            var langilea = new Langilea
            {
                Izena = dto.Izena,
                Abizena = dto.Abizena,
                NAN = dto.NAN,
                Erabiltzaile_izena = dto.Erabiltzaile_izena,
                Langile_kodea = dto.Langile_kodea,
                Pasahitza = HashPassword(dto.Pasahitza),
                Helbidea = dto.Helbidea,
                Lanpostua = new Lanpostua { Id = dto.Lanpostua.Id }
            };

            _repo.Add(langilea);
            dto.Id = langilea.Id;

            return CreatedAtAction(nameof(Get), new { id = langilea.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LangileaDto dto)
        {
            var langilea = _repo.Get(id);
            if (langilea == null) return NotFound();

            langilea.Izena = dto.Izena;
            langilea.Abizena = dto.Abizena;
            langilea.NAN = dto.NAN;
            langilea.Erabiltzaile_izena = dto.Erabiltzaile_izena;
            langilea.Langile_kodea = dto.Langile_kodea;
            if (!string.IsNullOrEmpty(dto.Pasahitza))
            {
                langilea.Pasahitza = HashPassword(dto.Pasahitza);
            }
            langilea.Helbidea = dto.Helbidea;
            langilea.Lanpostua = new Lanpostua { Id = dto.Lanpostua.Id };

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
