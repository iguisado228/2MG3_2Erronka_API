using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.DTOak;
using _2Erronka_API.Domain;
using _2Erronka_API.Repositorioak;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/odoo/lanpostuak")]
    public class OdooLanpostuakController : ControllerBase
    {
        private readonly LanpostuaRepository _repo;

        public OdooLanpostuakController(LanpostuaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var lanpostuak = _repo.GetAll();
            var dtoList = lanpostuak.Select(l => new LanpostuaDto
            {
                Id = l.Id,
                Lanpostu_izena = l.Lanpostu_izena
            }).ToList();

            return Ok(dtoList);
        }

        [HttpPost]
        public IActionResult Create([FromBody] LanpostuaDto dto)
        {
            if (dto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.Lanpostu_izena)) return BadRequest("Lanpostu_izena beharrezkoa da.");

            var lanpostua = new Lanpostua
            {
                Lanpostu_izena = dto.Lanpostu_izena
            };

            _repo.Add(lanpostua);
            dto.Id = lanpostua.Id;

            return CreatedAtAction(nameof(GetAll), new { id = lanpostua.Id }, dto);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] LanpostuaDto dto)
        {
            if (dto == null) return BadRequest();
            if (string.IsNullOrWhiteSpace(dto.Lanpostu_izena)) return BadRequest("Lanpostu_izena beharrezkoa da.");

            var lanpostua = _repo.Get(id);
            if (lanpostua == null) return NotFound();

            lanpostua.Lanpostu_izena = dto.Lanpostu_izena;
            _repo.Update(lanpostua);

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var lanpostua = _repo.Get(id);
            if (lanpostua == null) return NotFound();

            _repo.Delete(lanpostua);
            return NoContent();
        }
    }
}
