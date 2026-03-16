﻿﻿﻿using Microsoft.AspNetCore.Mvc;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.DTOak;

namespace _2Erronka_API.Controllers
{
    /// <summary>
    /// Osagaien kontsultarako API amaierako puntuak.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OsagaiakController : ControllerBase
    {
        private readonly OsagaiaRepository _repo;

        public OsagaiakController(OsagaiaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Osagai guztiak eskuratzen ditu.
        /// </summary>
        /// <returns>200 OK erantzuna, OsagaiaDto zerrendarekin.</returns>
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

        /// <summary>
        /// IDaren arabera osagai bat eskuratzen du.
        /// </summary>
        /// <param name="id">Osagaiaren identifikatzailea.</param>
        /// <returns>200 OK (OsagaiaDto) edo 404 Not Found.</returns>
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

        /// <summary>
        /// Osagai berri bat sortzen du.
        /// </summary>
        /// <param name="dto">Osagaia sortzeko datuak.</param>
        /// <returns>201 Created erantzuna.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] OsagaiaDto dto)
        {
            var osagaia = new _2Erronka_API.Modeloak.Osagaia
            {
                Izena = dto.Izena,
                Prezioa = dto.Prezioa,
                Stock = dto.Stock,
                HornitzaileakId = dto.HornitzaileakId
            };

            _repo.Add(osagaia);
            dto.Id = osagaia.Id;

            return CreatedAtAction(nameof(Get), new { id = osagaia.Id }, dto);
        }

        /// <summary>
        /// Existitzen den osagai bat eguneratzen du.
        /// </summary>
        /// <param name="id">Osagaiaren identifikatzailea.</param>
        /// <param name="dto">Eguneratzeko osagaiaren datuak.</param>
        /// <returns>204 No Content edo 404 Not Found.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] OsagaiaDto dto)
        {
            var osagaia = _repo.Get(id);
            if (osagaia == null) return NotFound();

            osagaia.Izena = dto.Izena;
            osagaia.Prezioa = dto.Prezioa;
            osagaia.Stock = dto.Stock;
            osagaia.HornitzaileakId = dto.HornitzaileakId;

            _repo.Update(osagaia);
            return NoContent();
        }

        /// <summary>
        /// Osagai bat ezabatzen du.
        /// </summary>
        /// <param name="id">Osagaiaren identifikatzailea.</param>
        /// <returns>204 No Content edo 404 Not Found.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var osagaia = _repo.Get(id);
            if (osagaia == null) return NotFound();

            _repo.Delete(osagaia);
            return NoContent();
        }

        [HttpGet("stock-gutxi")]
        public IActionResult GetStockGutxi()
        {
            var results = _repo.GetAll().Where(o => o.Stock < 10).ToList(); // Adibidez 10 baino gutxiago
            return Ok(results);
        }

        [HttpGet("search/{searchTerm}")]
        public IActionResult Search(string searchTerm)
        {
            var results = _repo.GetAll().Where(o => o.Izena.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)).ToList();
            return Ok(results);
        }

        [HttpPatch("{id}/stock")]
        public IActionResult UpdateStock(int id, [FromBody] StockUpdateDto dto)
        {
            var osagaia = _repo.Get(id);
            if (osagaia == null) return NotFound();

            osagaia.Stock += dto.Kopurua;
            _repo.Update(osagaia);
            return NoContent();
        }

        [HttpPatch("{id}/eskatu")]
        public IActionResult ToggleEskatu(int id)
        {
            // DB-n ez dago 'eskatu' zutaberik db.sql-en arabera, baina Java-k deitzen du.
            // Momentuz OK itzuliko dugu.
            return Ok();
        }
    }

    public class StockUpdateDto
    {
        public int Kopurua { get; set; }
    }
}
