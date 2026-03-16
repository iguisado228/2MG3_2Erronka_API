using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.DTOak;

namespace _2Erronka_API.Controllers
{
    /// <summary>
    /// Produktuen kontsultarako API amaierako puntuak.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProduktuakController : ControllerBase
    {
        private readonly ProduktuaRepository _repo;

        public ProduktuakController(ProduktuaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Produktu guztiak eskuratzen ditu.
        /// </summary>
        /// <returns>200 OK erantzuna, ProduktuaDto zerrendarekin.</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var produktuak = _repo.GetAll();

            var dtoList = produktuak.Select(p => new ProduktuaDto
            {
                Id = p.Id,
                Izena = p.Izena,
                Prezioa = p.Prezioa,
                MotaId = p.MotaId,
                Stock = p.Stock
            }).ToList();

            return Ok(dtoList);
        }

        /// <summary>
        /// IDaren arabera produktu bat eskuratzen du.
        /// </summary>
        /// <param name="id">Produktuaren identifikatzailea.</param>
        /// <returns>200 OK (ProduktuaDto) edo 404 Not Found.</returns>
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var produktua = _repo.Get(id);
            if (produktua == null) return NotFound();

            var dto = new ProduktuaDto
            {
                Id = produktua.Id,
                Izena = produktua.Izena,
                Prezioa = produktua.Prezioa,
                MotaId = produktua.MotaId,
                Stock = produktua.Stock
            };

            return Ok(dto);
        }

        /// <summary>
        /// Produktu berri bat sortzen du.
        /// </summary>
        /// <param name="dto">Produktuaren datuak.</param>
        /// <returns>201 Created.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] ProduktuaDto dto)
        {
            var produktua = new _2Erronka_API.Modeloak.Produktua
            {
                Izena = dto.Izena,
                Prezioa = dto.Prezioa,
                MotaId = dto.MotaId,
                Stock = dto.Stock
            };

            _repo.Add(produktua);
            dto.Id = produktua.Id;

            return CreatedAtAction(nameof(Get), new { id = produktua.Id }, dto);
        }

        /// <summary>
        /// Existitzen den produktu bat eguneratzen du.
        /// </summary>
        /// <param name="id">Produktuaren identifikatzailea.</param>
        /// <param name="dto">Produktuaren datu berriak.</param>
        /// <returns>204 No Content edo 404 Not Found.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ProduktuaDto dto)
        {
            var produktua = _repo.Get(id);
            if (produktua == null) return NotFound();

            produktua.Izena = dto.Izena;
            produktua.Prezioa = dto.Prezioa;
            produktua.MotaId = dto.MotaId;
            produktua.Stock = dto.Stock;

            _repo.Update(produktua);
            return NoContent();
        }

        /// <summary>
        /// Produktu bat ezabatzen du.
        /// </summary>
        /// <param name="id">Produktuaren identifikatzailea.</param>
        /// <returns>204 No Content edo 404 Not Found.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var produktua = _repo.Get(id);
            if (produktua == null) return NotFound();

            _repo.Delete(produktua);
            return NoContent();
        }
    }
}
