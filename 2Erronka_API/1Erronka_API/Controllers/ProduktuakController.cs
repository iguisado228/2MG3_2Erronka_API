using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;

namespace _1Erronka_API.Controllers
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
                Mota = p.Mota,
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
                Mota = produktua.Mota,
                Stock = produktua.Stock
            };

            return Ok(dto);
        }
    }
}
