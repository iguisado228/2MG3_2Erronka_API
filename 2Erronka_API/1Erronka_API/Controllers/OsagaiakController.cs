using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;

namespace _1Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OsagaiakController : ControllerBase
    {
        private readonly OsagaiaRepository _repo;

        public OsagaiakController(OsagaiaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var osagaiak = _repo.GetAll();

            var dtoList = osagaiak.Select(o => new OsagaiaDto
            {
                Id = o.Id,
                Izena = o.Izena,
                Prezioa = o.Prezioa,
                Stock = o.Stock,
                HornitzaileakId = o.HornitzaileakId
            }).ToList();

            return Ok(dtoList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var osagaia = _repo.Get(id);
            if (osagaia == null) return NotFound();

            var dto = new OsagaiaDto
            {
                Id = osagaia.Id,
                Izena = osagaia.Izena,
                Prezioa = osagaia.Prezioa,
                Stock = osagaia.Stock,
                HornitzaileakId = osagaia.HornitzaileakId
            };

            return Ok(dto);
        }
    }
}
