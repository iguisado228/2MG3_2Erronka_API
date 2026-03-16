using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HornitzaileakController : ControllerBase
    {
        private readonly HornitzaileaRepository _repo;

        public HornitzaileakController(HornitzaileaRepository repo)
        {
            _repo = repo;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_repo.GetAll());
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var h = _repo.Get(id);
            if (h == null) return NotFound();
            return Ok(h);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Hornitzailea h)
        {
            _repo.Add(h);
            return CreatedAtAction(nameof(Get), new { id = h.Id }, h);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Hornitzailea h)
        {
            var existing = _repo.Get(id);
            if (existing == null) return NotFound();

            existing.Izena = h.Izena;
            existing.Kontaktua = h.Kontaktua;
            existing.Helbidea = h.Helbidea;

            _repo.Update(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var h = _repo.Get(id);
            if (h == null) return NotFound();

            _repo.Delete(h);
            return NoContent();
        }

        [HttpGet("search/{izena}")]
        public IActionResult Search(string izena)
        {
            var results = _repo.GetAll().Where(h => h.Izena.Contains(izena, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(results);
        }
    }
}
