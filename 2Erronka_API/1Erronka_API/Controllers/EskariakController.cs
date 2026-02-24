using Microsoft.AspNetCore.Mvc;
using _1Erronka_API.Repositorioak;
using _1Erronka_API.DTOak;
using _1Erronka_API.Modeloak;

namespace _1Erronka_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EskariakController : ControllerBase
    {
        private readonly EskariaRepository _repo;
        private readonly ProduktuaRepository _produktuaRepo;
        private readonly ErreserbaRepository _erreserbaRepo;
        private readonly ProduktuaOsagaiaRepository _produktuaOsagaiaRepo;
        private readonly OsagaiaRepository _osagaiaRepo;


        public EskariakController(EskariaRepository repo, ProduktuaRepository produktuaRepo, ErreserbaRepository erreserbaRepo, ProduktuaOsagaiaRepository produktuOsagaiaRepo, OsagaiaRepository osagaiaRepo)
        {
            _repo = repo;
            _produktuaRepo = produktuaRepo;
            _erreserbaRepo = erreserbaRepo;
            _produktuaOsagaiaRepo = produktuOsagaiaRepo;
            _osagaiaRepo = osagaiaRepo;
        }

        [HttpPost]
        public IActionResult Sortu([FromBody] EskariaSortuDto dto)
        {
            object erantzuna = null;

            try
            {
                _repo.ExecuteSerializableTransaction(() =>
                {
                    var erreserba = _erreserbaRepo.Get(dto.ErreserbaId);
                    if (erreserba == null) throw new Exception("Erreserba ez da aurkitu");

                    if (erreserba.Langilea == null) throw new Exception("Erreserbak ez du langilerik asignatuta");

                    var eskaria = new Eskaria
                    {
                        Erreserba = erreserba,
                        Prezioa = dto.Prezioa,
                        Egoera = dto.Egoera,
                        Langilea = erreserba.Langilea,
                        Mahaia = erreserba.Mahaia,
                        Produktuak = new List<EskariaProduktua>()
                    };

                    foreach (var p in dto.Produktuak)
                    {
                        var produktua = _produktuaRepo.Get(p.ProduktuaId);
                        if (produktua == null) continue;

                        if (produktua.Stock <= 0)
                            throw new Exception($"Ez dago stock-ik '{produktua.Izena}' produktuan.");

                        if (produktua.Stock < p.Kantitatea)
                        {
                            p.Kantitatea = produktua.Stock;
                        }

                        var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);

                        foreach (var po in osagaiak)
                        {
                            var osagaia = po.Osagaia;
                            var kantitateaTotala = po.Kantitatea * p.Kantitatea;

                            if (osagaia.Stock < kantitateaTotala)
                                throw new Exception($"Ez dago nahikoa stock '{osagaia.Izena}' osagaian");
                        }

                        foreach (var po in osagaiak)
                        {
                            var osagaia = po.Osagaia;
                            var kantitateaTotala = po.Kantitatea * p.Kantitatea;

                            osagaia.Stock -= kantitateaTotala;
                            _osagaiaRepo.Update(osagaia);
                        }

                        produktua.Stock -= p.Kantitatea;
                        _produktuaRepo.Update(produktua);

                        eskaria.Produktuak.Add(new EskariaProduktua
                        {
                            Eskaria = eskaria,
                            Produktua = produktua,
                            Kantitatea = p.Kantitatea,
                            Prezioa = produktua.Prezioa
                        });
                    }

                    eskaria.Prezioa = eskaria.Produktuak.Sum(p => p.Prezioa * p.Kantitatea);
                    _repo.Add(eskaria);

                    erantzuna = new
                    {
                        EskariaId = eskaria.Id,
                        PrezioaTotala = eskaria.Produktuak.Sum(p => p.Prezioa * p.Kantitatea),
                        Produktuak = eskaria.Produktuak.Select(p => new
                        {
                            ProduktuaIzena = p.Produktua.Izena,
                            Kantitatea = p.Kantitatea,
                            ProduktuakPrezioaBakarka = p.Prezioa,
                            ProduktuakPrezioaGuztira = p.Prezioa * p.Kantitatea
                        }).ToList()
                    };
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(erantzuna);
        }


        [HttpPut("{id}")]
        public IActionResult Eguneratu(int id, [FromBody] EskariaSortuDto dto)
        {
            try
            {
                _repo.ExecuteSerializableTransaction(() =>
                {
                    var eskaria = _repo.Get(id);
                    if (eskaria == null) throw new Exception("Eskaria ez da aurkitu");

                    var existingProductsMap = eskaria.Produktuak.ToDictionary(p => p.Produktua.Id);
                    var newProductIds = dto.Produktuak.Select(p => p.ProduktuaId).ToHashSet();

                    var toRemove = existingProductsMap.Values.Where(p => !newProductIds.Contains(p.Produktua.Id)).ToList();
                    foreach (var item in toRemove)
                    {
                        var produktua = item.Produktua;
                        produktua.Stock += item.Kantitatea;
                        _produktuaRepo.Update(produktua);

                        var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);
                        foreach (var po in osagaiak)
                        {
                            po.Osagaia.Stock += po.Kantitatea * item.Kantitatea;
                            _osagaiaRepo.Update(po.Osagaia);
                        }

                        eskaria.Produktuak.Remove(item);
                    }

                    foreach (var newItem in dto.Produktuak)
                    {
                        if (existingProductsMap.TryGetValue(newItem.ProduktuaId, out var existingItem))
                        {
                            int diff = newItem.Kantitatea - existingItem.Kantitatea;

                            if (diff != 0)
                            {
                                var produktua = existingItem.Produktua;

                                if (diff > 0 && produktua.Stock < diff)
                                    throw new Exception($"Ez dago stock-ik '{produktua.Izena}' produktuan.");

                                produktua.Stock -= diff;
                                _produktuaRepo.Update(produktua);

                                var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);
                                foreach (var po in osagaiak)
                                {
                                    if (diff > 0 && po.Osagaia.Stock < po.Kantitatea * diff)
                                        throw new Exception($"Ez dago nahikoa stock '{po.Osagaia.Izena}' osagaian");

                                    po.Osagaia.Stock -= po.Kantitatea * diff;
                                    _osagaiaRepo.Update(po.Osagaia);
                                }

                                existingItem.Kantitatea = newItem.Kantitatea;
                            }
                            
                            existingItem.Prezioa = newItem.Prezioa;
                        }
                        else
                        {
                            var produktua = _produktuaRepo.Get(newItem.ProduktuaId);
                            if (produktua == null) continue;

                            if (produktua.Stock < newItem.Kantitatea)
                                throw new Exception($"Ez dago stock-ik '{produktua.Izena}' produktuan.");

                            produktua.Stock -= newItem.Kantitatea;
                            _produktuaRepo.Update(produktua);

                            var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);
                            foreach (var po in osagaiak)
                            {
                                if (po.Osagaia.Stock < po.Kantitatea * newItem.Kantitatea)
                                    throw new Exception($"Ez dago nahikoa stock '{po.Osagaia.Izena}' osagaian");

                                po.Osagaia.Stock -= po.Kantitatea * newItem.Kantitatea;
                                _osagaiaRepo.Update(po.Osagaia);
                            }

                            eskaria.Produktuak.Add(new EskariaProduktua
                            {
                                Eskaria = eskaria,
                                Produktua = produktua,
                                Kantitatea = newItem.Kantitatea,
                                Prezioa = produktua.Prezioa
                            });
                        }
                    }

                    eskaria.Prezioa = dto.Prezioa;
                    eskaria.Egoera = dto.Egoera;

                    _repo.Update(eskaria);
                });

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Ezabatu(int id)
        {
            try
            {
                _repo.ExecuteSerializableTransaction(() =>
                {
                    var eskaria = _repo.Get(id);
                    if (eskaria == null) throw new Exception("Eskaria ez da aurkitu");

                    foreach (var p in eskaria.Produktuak)
                    {
                        var produktua = _produktuaRepo.Get(p.Produktua.Id);
                        if (produktua != null)
                        {
                            produktua.Stock += p.Kantitatea;
                            _produktuaRepo.Update(produktua);

                            var osagaiak = _produktuaOsagaiaRepo.GetByProduktuaId(produktua.Id);
                            foreach (var po in osagaiak)
                            {
                                po.Osagaia.Stock += po.Kantitatea * p.Kantitatea;
                                _osagaiaRepo.Update(po.Osagaia);
                            }
                        }
                    }

                    _repo.Delete(eskaria);
                });

                return Ok(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetEskaria(int id)
        {
            var eskaria = _repo.Get(id);
            if (eskaria == null) return NotFound();

            var dto = new EskariaDto
            {
                Id = eskaria.Id,
                Prezioa = eskaria.Prezioa,
                Egoera = eskaria.Egoera,
                ErreserbaId = eskaria.Erreserba.Id,
                Produktuak = eskaria.Produktuak.Select(p => new EskariaProduktuaDto
                {
                    ProduktuaId = p.Produktua.Id,
                    ProduktuaIzena = p.Produktua.Izena,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                }).ToList()
            };

            return Ok(dto);
        }

        [HttpGet("erreserba/{erreserbaId}")]
        public IActionResult GetEskariakByErreserba(int erreserbaId)
        {
            var eskariak = _repo.GetAll().Where(e => e.Erreserba.Id == erreserbaId).ToList();

            var dtoList = eskariak.Select(e => new EskariaDto
            {
                Id = e.Id,
                Prezioa = e.Prezioa,
                Egoera = e.Egoera,
                ErreserbaId = e.Erreserba.Id,
                Produktuak = e.Produktuak.Select(p => new EskariaProduktuaDto
                {
                    ProduktuaId = p.Produktua.Id,
                    ProduktuaIzena = p.Produktua.Izena,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }


        [HttpGet]
        public IActionResult GetEskariak()
        {
            var eskariak = _repo.GetAll().ToList();

            var dtoList = eskariak.Select(e => new EskariaDto
            {
                Id = e.Id,
                Prezioa = e.Prezioa,
                Egoera = e.Egoera,
                ErreserbaId = e.Erreserba.Id,

                MahaiaZenbakia = e.Erreserba.Mahaia.Zenbakia,

                Produktuak = e.Produktuak.Select(p => new EskariaProduktuaDto
                {
                    ProduktuaId = p.Produktua.Id,
                    ProduktuaIzena = p.Produktua.Izena,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }



        [HttpGet("egoera/{egoera}")]
        public IActionResult GetEskariakByEgoera(string egoera)
        {
            var eskariak = _repo.GetAll()
                .Where(e => e.Egoera.Equals(egoera, StringComparison.OrdinalIgnoreCase))
                .ToList();

            var dtoList = eskariak.Select(e => new EskariaDto
            {
                Id = e.Id,
                Prezioa = e.Prezioa,
                Egoera = e.Egoera,
                ErreserbaId = e.Erreserba.Id,
                Produktuak = e.Produktuak.Select(p => new EskariaProduktuaDto
                {
                    ProduktuaId = p.Produktua.Id,
                    ProduktuaIzena = p.Produktua.Izena,
                    Kantitatea = p.Kantitatea,
                    Prezioa = p.Prezioa
                }).ToList()
            }).ToList();

            return Ok(dtoList);
        }

    }
}
