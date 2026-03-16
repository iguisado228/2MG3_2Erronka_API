using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.DTOak;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LanpostuakController : ControllerBase
    {
        private readonly LanpostuaRepository _repo;

        public LanpostuakController(LanpostuaRepository repo)
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

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var l = _repo.Get(id);
            if (l == null) return NotFound();

            var dto = new LanpostuaDto
            {
                Id = l.Id,
                Lanpostu_izena = l.Lanpostu_izena
            };

            return Ok(dto);
        }
    }
}
