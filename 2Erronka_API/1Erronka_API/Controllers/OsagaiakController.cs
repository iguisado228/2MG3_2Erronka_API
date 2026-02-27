﻿using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;

namespace _1Erronka_API.Controllers
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
    }
}
