using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.Modeloak;

namespace _2Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KategoriakController : ControllerBase
    {
        private readonly MotaRepository _repo;

        public KategoriakController(MotaRepository repo)
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
            var m = _repo.Get(id);
            if (m == null) return NotFound();
            return Ok(m);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Mota m)
        {
            _repo.Add(m);
            return CreatedAtAction(nameof(Get), new { id = m.Id }, m);
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] Mota m)
        {
            var existing = _repo.Get(id);
            if (existing == null) return NotFound();

            existing.Izena = m.Izena;

            _repo.Update(existing);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var m = _repo.Get(id);
            if (m == null) return NotFound();

            _repo.Delete(m);
            return NoContent();
        }
    }
}
