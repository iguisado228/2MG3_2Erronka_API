using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Controllers
{
    /// <summary>
    /// Mahaien kudeaketarako API amaierako puntuak.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MahaiakController : ControllerBase
    {
        private readonly MahaiaRepository _repo;

        public MahaiakController(MahaiaRepository repo)
        {
            _repo = repo;
        }

        /// <summary>
        /// Mahai guztiak eskuratzen ditu.
        /// </summary>
        /// <returns>200 OK erantzuna, MahaiaDto zerrendarekin.</returns>
        [HttpGet]
        public IActionResult GetAll()
        {
            var mahaiak = _repo.GetAll();

            var dtoList = mahaiak.Select(m => new MahaiaDto
            {
                Id = m.Id,
                Zenbakia = m.Zenbakia,
                PertsonaKopurua = m.PertsonaKopurua,
                Kokapena = m.Kokapena
            }).ToList();

            return Ok(dtoList);
        }

        /// <summary>
        /// Mahai berria sortzen du.
        /// </summary>
        /// <param name="dto">Mahaiaren datuak.</param>
        /// <returns>200 OK erantzuna.</returns>
        [HttpPost]
        public IActionResult Create([FromBody] MahaiaDto dto)
        {
            var mahaia = new Mahaia
            {
                Zenbakia = dto.Zenbakia,
                PertsonaKopurua = dto.PertsonaKopurua,
                Kokapena = dto.Kokapena ?? ""
            };

            _repo.Add(mahaia);

            return Ok();
            Console.WriteLine($"Zenbakia: {dto.Zenbakia}, PertsonaKopurua: {dto.PertsonaKopurua}, Kokapena: {dto.Kokapena}");
        }

        /// <summary>
        /// Existitzen den mahai bat eguneratzen du.
        /// </summary>
        /// <param name="id">Mahaiaren identifikatzailea.</param>
        /// <param name="dto">Eguneratzeko mahaiaren datuak.</param>
        /// <returns>200 OK edo 404 Not Found.</returns>
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] MahaiaDto dto)
        {
            var mahaia = _repo.Get(id);
            if (mahaia == null)
                return NotFound();

            mahaia.Zenbakia = dto.Zenbakia;
            mahaia.PertsonaKopurua = dto.PertsonaKopurua;
            mahaia.Kokapena = dto.Kokapena;

            _repo.Update(mahaia);

            return Ok();
        }

        /// <summary>
        /// Mahai bat ezabatzen du.
        /// </summary>
        /// <param name="id">Mahaiaren identifikatzailea.</param>
        /// <returns>200 OK edo 404 Not Found.</returns>
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var mahaia = _repo.Get(id);
            if (mahaia == null)
                return NotFound();

            _repo.Delete(mahaia);

            return Ok();
        }

    }
}