using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using _2Erronka_API.Controllers;
using _2Erronka_API.Domain;
using _2Erronka_API.DTOak;
using _2Erronka_API.Modeloak;
using _2Erronka_API.Repositorioak;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _2Erronka_API.Testak
{
    public class EskariakControllerTest
    {
        private static EskariakController CreateController(
            out Mock<EskariaRepository> eskariaRepo,
            out Mock<ProduktuaRepository> produktuaRepo,
            out Mock<ErreserbaRepository> erreserbaRepo,
            out Mock<ProduktuaOsagaiaRepository> produktuOsagaiaRepo,
            out Mock<OsagaiaRepository> osagaiaRepo)
        {
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);

            eskariaRepo = new Mock<EskariaRepository>(mockSessionFactory.Object);
            produktuaRepo = new Mock<ProduktuaRepository>(mockSessionFactory.Object);
            erreserbaRepo = new Mock<ErreserbaRepository>(mockSessionFactory.Object);
            produktuOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSessionFactory.Object);
            osagaiaRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            eskariaRepo
                .Setup(r => r.ExecuteSerializableTransaction(It.IsAny<Action>()))
                .Callback<Action>(action => action());

            return new EskariakController(
                eskariaRepo.Object,
                produktuaRepo.Object,
                erreserbaRepo.Object,
                produktuOsagaiaRepo.Object,
                osagaiaRepo.Object
            );
        }

        private static Erreserba CreateErreserba(int id, Langilea? langilea, Mahaia mahaia)
        {
            return new Erreserba
            {
                Id = id,
                BezeroIzena = "Bezeroa",
                Telefonoa = "000",
                PertsonaKopurua = 2,
                EgunaOrdua = DateTime.UtcNow,
                PrezioTotala = 0,
                Ordainduta = 0,
                FakturaRuta = string.Empty,
                Langilea = langilea!,
                Mahaia = mahaia
            };
        }

        private static object GetAnonymousProp(object obj, string propName)
        {
            var prop = obj.GetType().GetProperty(propName);
            Assert.NotNull(prop);
            return prop.GetValue(obj)!;
        }

        [Fact]
        public void Sortu_BadRequestItzultzenDu_ErreserbaEzDeneanAurkitu()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var dto = new EskariaSortuDto
            {
                ErreserbaId = 123,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>()
            };

            erreserbaRepo.Setup(r => r.Get(dto.ErreserbaId)).Returns((Erreserba?)null);

            var result = controller.Sortu(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erreserba ez da aurkitu", badRequest.Value);
        }

        [Fact]
        public void Sortu_BadRequestItzultzenDu_ErreserbakLangilerikEzDuenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "Egongela" };
            var erreserba = CreateErreserba(1, null, mahaia);

            var dto = new EskariaSortuDto
            {
                ErreserbaId = 1,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>()
            };

            erreserbaRepo.Setup(r => r.Get(dto.ErreserbaId)).Returns(erreserba);

            var result = controller.Sortu(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Erreserbak ez du langilerik asignatuta", badRequest.Value);
        }

        [Fact]
        public void Sortu_OkItzultzenDu_ProduktuaEzDeneanExistitzenEtaSaltatzenDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var mahaia = new Mahaia { Id = 1, Zenbakia = 3, PertsonaKopurua = 4, Kokapena = "Terraza" };
            var langilea = new Langilea { Id = 1, Izena = "Ane", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } };
            var erreserba = CreateErreserba(10, langilea, mahaia);

            var dto = new EskariaSortuDto
            {
                ErreserbaId = 10,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 999, Kantitatea = 2 }
                }
            };

            erreserbaRepo.Setup(r => r.Get(dto.ErreserbaId)).Returns(erreserba);
            produktuaRepo.Setup(r => r.Get(999)).Returns((Produktua?)null);

            var result = controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            eskariaRepo.Verify(r => r.Add(It.Is<Eskaria>(e => e.Produktuak.Count == 0 && e.Prezioa == 0)), Times.Once);
        }

        [Fact]
        public void Sortu_BadRequestItzultzenDu_ProduktuaStockikEzDuenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var mahaia = new Mahaia { Id = 1, Zenbakia = 2, PertsonaKopurua = 2, Kokapena = "Barra" };
            var langilea = new Langilea { Id = 1, Izena = "Jon", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } };
            var erreserba = CreateErreserba(1, langilea, mahaia);

            var produktua = new Produktua { Id = 5, Izena = "Kafea", Prezioa = 1.2, Stock = 0, MotaId = 1 };
            var dto = new EskariaSortuDto
            {
                ErreserbaId = 1,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 5, Kantitatea = 1 }
                }
            };

            erreserbaRepo.Setup(r => r.Get(1)).Returns(erreserba);
            produktuaRepo.Setup(r => r.Get(5)).Returns(produktua);

            var result = controller.Sortu(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ez dago stock-ik 'Kafea' produktuan.", badRequest.Value);
        }

        [Fact]
        public void Sortu_OkItzultzenDu_KantitateaDoitzenDuenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var mahaia = new Mahaia { Id = 1, Zenbakia = 7, PertsonaKopurua = 4, Kokapena = "Egongela" };
            var langilea = new Langilea { Id = 1, Izena = "Amaia", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } };
            var erreserba = CreateErreserba(7, langilea, mahaia);

            var produktua = new Produktua { Id = 2, Izena = "Ogitartekoa", Prezioa = 4.0, Stock = 3, MotaId = 1 };
            var dto = new EskariaSortuDto
            {
                ErreserbaId = 7,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 2, Kantitatea = 5 }
                }
            };

            erreserbaRepo.Setup(r => r.Get(7)).Returns(erreserba);
            produktuaRepo.Setup(r => r.Get(2)).Returns(produktua);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(2)).Returns(new List<ProduktuaOsagaia>());

            var result = controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 2 && p.Stock == 0)), Times.Once);
            var prezioaTotala = (double)GetAnonymousProp(okResult.Value!, "PrezioaTotala");
            Assert.Equal(12.0, prezioaTotala);
        }

        [Fact]
        public void Sortu_BadRequestItzultzenDu_OsagaiaStockNahikorikEzDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "Egongela" };
            var langilea = new Langilea { Id = 1, Izena = "Ane", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } };
            var erreserba = CreateErreserba(1, langilea, mahaia);

            var produktua = new Produktua { Id = 3, Izena = "Pizza", Prezioa = 10, Stock = 10, MotaId = 1 };
            var osagaia = new Osagaia { Id = 8, Izena = "Gazta", Stock = 5, Prezioa = 1.0, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktua, Osagaia = osagaia, Kantitatea = 2 };

            var dto = new EskariaSortuDto
            {
                ErreserbaId = 1,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 3, Kantitatea = 3 }
                }
            };

            erreserbaRepo.Setup(r => r.Get(1)).Returns(erreserba);
            produktuaRepo.Setup(r => r.Get(3)).Returns(produktua);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(3)).Returns(new List<ProduktuaOsagaia> { po });

            var result = controller.Sortu(dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ez dago nahikoa stock 'Gazta' osagaian", badRequest.Value);
        }

        [Fact]
        public void Sortu_OkItzultzenDu_EskariaOndoSortzean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            var mahaia = new Mahaia { Id = 1, Zenbakia = 4, PertsonaKopurua = 2, Kokapena = "Terraza" };
            var langilea = new Langilea { Id = 1, Izena = "Nora", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } };
            var erreserba = CreateErreserba(4, langilea, mahaia);

            var produktua = new Produktua { Id = 9, Izena = "Hamburguesa", Prezioa = 8, Stock = 10, MotaId = 1 };
            var osagaia = new Osagaia { Id = 11, Izena = "Ogia", Stock = 20, Prezioa = 0.5, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktua, Osagaia = osagaia, Kantitatea = 1 };

            var dto = new EskariaSortuDto
            {
                ErreserbaId = 4,
                Egoera = "prestatzen",
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 9, Kantitatea = 2 }
                }
            };

            erreserbaRepo.Setup(r => r.Get(4)).Returns(erreserba);
            produktuaRepo.Setup(r => r.Get(9)).Returns(produktua);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(9)).Returns(new List<ProduktuaOsagaia> { po });

            var result = controller.Sortu(dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 9 && p.Stock == 8)), Times.Once);
            osagaiaRepo.Verify(r => r.Update(It.Is<Osagaia>(o => o.Id == 11 && o.Stock == 18)), Times.Once);
            eskariaRepo.Verify(r => r.Add(It.Is<Eskaria>(e => e.Produktuak.Count == 1 && e.Prezioa == 16)), Times.Once);
        }

        [Fact]
        public void Eguneratu_BadRequestItzultzenDu_EskariaEzDeneanAurkitu()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            eskariaRepo.Setup(r => r.Get(1)).Returns((Eskaria?)null);

            var dto = new EskariaSortuDto { Egoera = "prestatzen", Prezioa = 0, Produktuak = new List<EskariaProduktuaSortuDto>() };
            var result = controller.Eguneratu(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Eskaria ez da aurkitu", badRequest.Value);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_ProduktuaKenduzEtaStockaLeheneratuz()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var produktua2 = new Produktua { Id = 2, Izena = "P2", Prezioa = 3, Stock = 1, MotaId = 1 };
            var osagaia = new Osagaia { Id = 100, Izena = "O1", Stock = 0, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktua2, Osagaia = osagaia, Kantitatea = 2 };

            var eskaria = new Eskaria
            {
                Id = 10,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 },
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua2, Kantitatea = 2, Prezioa = 3 }
                }
            };
            foreach (var ep in eskaria.Produktuak) ep.Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(10)).Returns(eskaria);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(2)).Returns(new List<ProduktuaOsagaia> { po });

            var dto = new EskariaSortuDto
            {
                ErreserbaId = 1,
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 1, Prezioa = 2 }
                }
            };

            var result = controller.Eguneratu(10, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 2 && p.Stock == 3)), Times.Once);
            osagaiaRepo.Verify(r => r.Update(It.Is<Osagaia>(o => o.Id == 100 && o.Stock == 4)), Times.Once);
            eskariaRepo.Verify(r => r.Update(It.IsAny<Eskaria>()), Times.Once);
            Assert.Single(eskaria.Produktuak);
            Assert.Equal(1, eskaria.Produktuak[0].Produktua.Id);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_DiffZeroDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 2, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 10,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 2, Prezioa = 99 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.IsAny<Produktua>()), Times.Never);
            eskariaRepo.Verify(r => r.Update(It.Is<Eskaria>(e => e.Egoera == "berria" && e.Prezioa == 10)), Times.Once);
            Assert.Equal(99, eskaria.Produktuak[0].Prezioa);
        }

        [Fact]
        public void Eguneratu_BadRequestItzultzenDu_DiffPositiboanProduktuStockikEzDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 0, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 2, Prezioa = 2 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ez dago stock-ik 'P1' produktuan.", badRequest.Value);
        }

        [Fact]
        public void Eguneratu_BadRequestItzultzenDu_DiffPositiboanOsagaiStockikEzDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 10, MotaId = 1 };
            var osagaia = new Osagaia { Id = 7, Izena = "O1", Stock = 0, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktua1, Osagaia = osagaia, Kantitatea = 1 };

            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(1)).Returns(new List<ProduktuaOsagaia> { po });

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 2, Prezioa = 2 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ez dago nahikoa stock 'O1' osagaian", badRequest.Value);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_DiffPositiboaArrakastaz()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 10, MotaId = 1 };
            var osagaia = new Osagaia { Id = 7, Izena = "O1", Stock = 10, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktua1, Osagaia = osagaia, Kantitatea = 2 };

            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(1)).Returns(new List<ProduktuaOsagaia> { po });

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 3, Prezioa = 2 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 1 && p.Stock == 8)), Times.Once);
            osagaiaRepo.Verify(r => r.Update(It.Is<Osagaia>(o => o.Id == 7 && o.Stock == 6)), Times.Once);
            Assert.Equal(3, eskaria.Produktuak[0].Kantitatea);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_DiffNegatiboaDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 0, MotaId = 1 };
            var osagaia = new Osagaia { Id = 7, Izena = "O1", Stock = 0, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktua1, Osagaia = osagaia, Kantitatea = 2 };

            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 3, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(1)).Returns(new List<ProduktuaOsagaia> { po });

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 1, Prezioa = 2 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 1 && p.Stock == 2)), Times.Once);
            osagaiaRepo.Verify(r => r.Update(It.Is<Osagaia>(o => o.Id == 7 && o.Stock == 4)), Times.Once);
            Assert.Equal(1, eskaria.Produktuak[0].Kantitatea);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_ProduktuaBerriaEzDeneanAurkituEtaSaltatzenDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuaRepo.Setup(r => r.Get(999)).Returns((Produktua?)null);

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 1, Prezioa = 2 },
                    new EskariaProduktuaSortuDto { ProduktuaId = 999, Kantitatea = 1, Prezioa = 1 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            Assert.Single(eskaria.Produktuak);
            Assert.Equal(1, eskaria.Produktuak[0].Produktua.Id);
        }

        [Fact]
        public void Eguneratu_BadRequestItzultzenDu_ProduktuaBerriaStockikEzDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            var produktuaBerria = new Produktua { Id = 2, Izena = "P2", Prezioa = 1, Stock = 0, MotaId = 1 };

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuaRepo.Setup(r => r.Get(2)).Returns(produktuaBerria);

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 1, Prezioa = 2 },
                    new EskariaProduktuaSortuDto { ProduktuaId = 2, Kantitatea = 1, Prezioa = 1 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ez dago stock-ik 'P2' produktuan.", badRequest.Value);
        }

        [Fact]
        public void Eguneratu_BadRequestItzultzenDu_ProduktuaBerriaOsagaiStockikEzDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            var produktuaBerria = new Produktua { Id = 2, Izena = "P2", Prezioa = 1, Stock = 10, MotaId = 1 };
            var osagaia = new Osagaia { Id = 7, Izena = "O1", Stock = 0, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktuaBerria, Osagaia = osagaia, Kantitatea = 1 };

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuaRepo.Setup(r => r.Get(2)).Returns(produktuaBerria);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(2)).Returns(new List<ProduktuaOsagaia> { po });

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 1, Prezioa = 2 },
                    new EskariaProduktuaSortuDto { ProduktuaId = 2, Kantitatea = 2, Prezioa = 1 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Ez dago nahikoa stock 'O1' osagaian", badRequest.Value);
        }

        [Fact]
        public void Eguneratu_OkItzultzenDu_ProduktuaBerriaOndoGehitzean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua1 = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua1, Kantitatea = 1, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            var produktuaBerria = new Produktua { Id = 2, Izena = "P2", Prezioa = 1, Stock = 10, MotaId = 1 };
            var osagaia = new Osagaia { Id = 7, Izena = "O1", Stock = 10, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktuaBerria, Osagaia = osagaia, Kantitatea = 2 };

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuaRepo.Setup(r => r.Get(2)).Returns(produktuaBerria);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(2)).Returns(new List<ProduktuaOsagaia> { po });

            var dto = new EskariaSortuDto
            {
                Egoera = "berria",
                Prezioa = 0,
                Produktuak = new List<EskariaProduktuaSortuDto>
                {
                    new EskariaProduktuaSortuDto { ProduktuaId = 1, Kantitatea = 1, Prezioa = 2 },
                    new EskariaProduktuaSortuDto { ProduktuaId = 2, Kantitatea = 2, Prezioa = 1 }
                }
            };

            var result = controller.Eguneratu(1, dto);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 2 && p.Stock == 8)), Times.Once);
            osagaiaRepo.Verify(r => r.Update(It.Is<Osagaia>(o => o.Id == 7 && o.Stock == 6)), Times.Once);
            Assert.Equal(2, eskaria.Produktuak.Count);
            Assert.Contains(eskaria.Produktuak, ep => ep.Produktua.Id == 2 && ep.Kantitatea == 2);
        }

        [Fact]
        public void Ezabatu_BadRequestItzultzenDu_EskariaEzDeneanAurkitu()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            eskariaRepo.Setup(r => r.Get(1)).Returns((Eskaria?)null);

            var result = controller.Ezabatu(1);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Eskaria ez da aurkitu", badRequest.Value);
        }

        [Fact]
        public void Ezabatu_OkItzultzenDu_ProduktuaEzDeneanAurkituEtaHalaEreEzabatzenDuenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 0, MotaId = 1 };
            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua, Kantitatea = 2, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuaRepo.Setup(r => r.Get(1)).Returns((Produktua?)null);

            var result = controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            eskariaRepo.Verify(r => r.Delete(It.Is<Eskaria>(e => e.Id == 1)), Times.Once);
            produktuaRepo.Verify(r => r.Update(It.IsAny<Produktua>()), Times.Never);
        }

        [Fact]
        public void Ezabatu_OkItzultzenDu_EskariaEzabatzeanEtaStockaLeheneratzean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 0, MotaId = 1 };
            var produktuaDb = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 0, MotaId = 1 };
            var osagaia = new Osagaia { Id = 7, Izena = "O1", Stock = 0, Prezioa = 1, HornitzaileakId = 1 };
            var po = new ProduktuaOsagaia { Produktua = produktuaDb, Osagaia = osagaia, Kantitatea = 2 };

            var eskaria = new Eskaria
            {
                Id = 1,
                Egoera = "hasiera",
                Prezioa = 0,
                Erreserba = CreateErreserba(1, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" }),
                Langilea = new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } },
                Mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" },
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua, Kantitatea = 3, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(1)).Returns(eskaria);
            produktuaRepo.Setup(r => r.Get(1)).Returns(produktuaDb);
            produktuOsagaiaRepo.Setup(r => r.GetByProduktuaId(1)).Returns(new List<ProduktuaOsagaia> { po });

            var result = controller.Ezabatu(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(true, okResult.Value);
            produktuaRepo.Verify(r => r.Update(It.Is<Produktua>(p => p.Id == 1 && p.Stock == 3)), Times.Once);
            osagaiaRepo.Verify(r => r.Update(It.Is<Osagaia>(o => o.Id == 7 && o.Stock == 6)), Times.Once);
            eskariaRepo.Verify(r => r.Delete(It.Is<Eskaria>(e => e.Id == 1)), Times.Once);
        }

        [Fact]
        public void GetEskaria_NotFoundItzultzenDu_EskariaEzDeneanExistitzen()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            eskariaRepo.Setup(r => r.Get(1)).Returns((Eskaria?)null);

            var result = controller.GetEskaria(1);

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void GetEskaria_OkItzultzenDu_EskariaExistitzenDenean()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var produktua = new Produktua { Id = 1, Izena = "P1", Prezioa = 2, Stock = 5, MotaId = 1 };
            var erreserba = CreateErreserba(2, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" });
            var eskaria = new Eskaria
            {
                Id = 10,
                Egoera = "E1",
                Prezioa = 12,
                Erreserba = erreserba,
                Langilea = erreserba.Langilea,
                Mahaia = erreserba.Mahaia,
                Produktuak = new List<EskariaProduktua>
                {
                    new EskariaProduktua { Eskaria = null!, Produktua = produktua, Kantitatea = 2, Prezioa = 2 }
                }
            };
            eskaria.Produktuak[0].Eskaria = eskaria;

            eskariaRepo.Setup(r => r.Get(10)).Returns(eskaria);

            var result = controller.GetEskaria(10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var dto = Assert.IsType<EskariaDto>(okResult.Value);
            Assert.Equal(10, dto.Id);
            Assert.Equal(2, dto.ErreserbaId);
            Assert.Single(dto.Produktuak);
            Assert.Equal("P1", dto.Produktuak[0].ProduktuaIzena);
        }

        [Fact]
        public void GetEskariakByErreserba_OkItzultzenDu_ZerrendaHutsarekin()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            eskariaRepo.Setup(r => r.GetAll()).Returns(new List<Eskaria>());

            var result = controller.GetEskariakByErreserba(99);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<EskariaDto>>(okResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public void GetEskariakByErreserba_OkItzultzenDu_Zerrendarekin()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var erreserba1 = CreateErreserba(10, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "K" });
            var erreserba2 = CreateErreserba(11, new Langilea { Id = 2, Izena = "M", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 2, Zenbakia = 2, PertsonaKopurua = 4, Kokapena = "K" });

            var eskariak = new List<Eskaria>
            {
                new Eskaria { Id = 1, Egoera = "E", Prezioa = 1, Erreserba = erreserba1, Langilea = erreserba1.Langilea, Mahaia = erreserba1.Mahaia, Produktuak = new List<EskariaProduktua>() },
                new Eskaria { Id = 2, Egoera = "E", Prezioa = 1, Erreserba = erreserba2, Langilea = erreserba2.Langilea, Mahaia = erreserba2.Mahaia, Produktuak = new List<EskariaProduktua>() }
            };

            eskariaRepo.Setup(r => r.GetAll()).Returns(eskariak);

            var result = controller.GetEskariakByErreserba(10);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<EskariaDto>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal(10, list[0].ErreserbaId);
        }

        [Fact]
        public void GetEskariak_OkItzultzenDu_ZerrendaHutsarekin()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            eskariaRepo.Setup(r => r.GetAll()).Returns(new List<Eskaria>());

            var result = controller.GetEskariak();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<EskariaDto>>(okResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public void GetEskariak_OkItzultzenDu_Zerrendarekin()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var erreserba = CreateErreserba(10, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 12, PertsonaKopurua = 4, Kokapena = "K" });
            var eskariak = new List<Eskaria>
            {
                new Eskaria { Id = 1, Egoera = "E", Prezioa = 1, Erreserba = erreserba, Langilea = erreserba.Langilea, Mahaia = erreserba.Mahaia, Produktuak = new List<EskariaProduktua>() }
            };

            eskariaRepo.Setup(r => r.GetAll()).Returns(eskariak);

            var result = controller.GetEskariak();

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<EskariaDto>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal(12, list[0].MahaiaZenbakia);
        }

        [Fact]
        public void GetEskariakByEgoera_OkItzultzenDu_ZerrendaHutsarekin()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);
            eskariaRepo.Setup(r => r.GetAll()).Returns(new List<Eskaria>());

            var result = controller.GetEskariakByEgoera("prestatzen");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<EskariaDto>>(okResult.Value);
            Assert.Empty(list);
        }

        [Fact]
        public void GetEskariakByEgoera_OkItzultzenDu_Zerrendarekin()
        {
            var controller = CreateController(out var eskariaRepo, out var produktuaRepo, out var erreserbaRepo, out var produktuOsagaiaRepo, out var osagaiaRepo);

            var erreserba = CreateErreserba(10, new Langilea { Id = 1, Izena = "L", Lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Zerbitzaria" } }, new Mahaia { Id = 1, Zenbakia = 12, PertsonaKopurua = 4, Kokapena = "K" });
            var eskariak = new List<Eskaria>
            {
                new Eskaria { Id = 1, Egoera = "Prestatzen", Prezioa = 1, Erreserba = erreserba, Langilea = erreserba.Langilea, Mahaia = erreserba.Mahaia, Produktuak = new List<EskariaProduktua>() },
                new Eskaria { Id = 2, Egoera = "Zerbitzatuta", Prezioa = 1, Erreserba = erreserba, Langilea = erreserba.Langilea, Mahaia = erreserba.Mahaia, Produktuak = new List<EskariaProduktua>() }
            };

            eskariaRepo.Setup(r => r.GetAll()).Returns(eskariak);

            var result = controller.GetEskariakByEgoera("preSTATzen");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var list = Assert.IsType<List<EskariaDto>>(okResult.Value);
            Assert.Single(list);
            Assert.Equal(1, list[0].Id);
        }
    }
}

