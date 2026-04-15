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
        private readonly ProduktuaOsagaiaRepository _produktuaOsagaiaRepo;
        private readonly OsagaiaRepository _osagaiaRepo;

        public ProduktuakController(
            ProduktuaRepository repo,
            ProduktuaOsagaiaRepository produktuaOsagaiaRepo,
            OsagaiaRepository osagaiaRepo
        )
        {
            _repo = repo;
            _produktuaOsagaiaRepo = produktuaOsagaiaRepo;
            _osagaiaRepo = osagaiaRepo;
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

        [HttpGet("{id}/osagaiak")]
        public IActionResult GetOsagaiak(int id)
        {
            var produktua = _repo.Get(id);
            if (produktua == null) return NotFound();

            var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(id);
            var dtoList =
                osagaiak.Select(po => new ProduktuaOsagaiaDto
                {
                    OsagaiaId = po.Osagaia.Id,
                    Kantitatea = po.Kantitatea,
                    Izena = po.Osagaia.Izena,
                    Stock = po.Osagaia.Stock
                }).ToList();

            return Ok(dtoList);
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
            if (dto.Stock < 0) return BadRequest("Stock ezin da negatiboa izan.");

            int? appliedStock = null;
            int requestedStock = dto.Stock;
            var notFound = false;

            _repo.ExecuteSerializableTransaction(() =>
            {
                var produktua = _repo.Get(id);
                if (produktua == null)
                {
                    notFound = true;
                    return;
                }

                produktua.Izena = dto.Izena;
                produktua.Prezioa = dto.Prezioa;
                produktua.MotaId = dto.MotaId;

                var currentStock = produktua.Stock;
                var requestedDelta = requestedStock - currentStock;
                if (requestedDelta == 0)
                {
                    _repo.Update(produktua);
                    appliedStock = requestedStock;
                    return;
                }

                var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);

                int appliedDelta = requestedDelta;
                if (requestedDelta > 0)
                {
                    var maxDelta = int.MaxValue;
                    foreach (var po in osagaiak)
                    {
                        if (po.Kantitatea <= 0) continue;
                        var possible = po.Osagaia.Stock / po.Kantitatea;
                        if (possible < maxDelta) maxDelta = possible;
                    }
                    if (maxDelta < 0) maxDelta = 0;
                    if (appliedDelta > maxDelta) appliedDelta = maxDelta;
                }

                if (appliedDelta != 0)
                {
                    foreach (var po in osagaiak)
                    {
                        if (po.Kantitatea <= 0) continue;
                        var deltaIng = -appliedDelta * po.Kantitatea;
                        po.Osagaia.Stock += deltaIng;
                        if (po.Osagaia.Stock < 0)
                        {
                            throw new Exception($"Ez dago nahikoa stock '{po.Osagaia.Izena}' osagaian");
                        }
                        _osagaiaRepo.Update(po.Osagaia);
                    }
                }

                produktua.Stock = currentStock + appliedDelta;
                _repo.Update(produktua);
                appliedStock = produktua.Stock;
            });

            if (notFound) return NotFound();
            if (appliedStock == null) return StatusCode(500);
            if (appliedStock.Value != requestedStock)
            {
                return Ok(new { requestedStock, appliedStock = appliedStock.Value });
            }
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
