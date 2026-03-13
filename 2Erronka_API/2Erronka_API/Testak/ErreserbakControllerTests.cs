using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using _2Erronka_API.Controllers;
using _2Erronka_API.Modeloak;
using _2Erronka_API.DTOak;
using _2Erronka_API.Repositorioak;
using _2Erronka_API.Domain;
using NHibernate;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;

namespace _2Erronka_API.Testak
{
    public class ErreserbakControllerTests
    {
        private Mock<ISessionFactory> CreateMockSessionFactory(out Mock<NHibernate.ISession> mockSession)
        {
            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            return mockSessionFactory;
        }

        [Fact]
        public void GetAll_OkItzultzenDu_ErreserbakExistitzenDirenean()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            var erreserbak = new List<Erreserba>
            {
                new Erreserba { Id = 1, BezeroIzena = "Jon", Langilea = new Langilea { Id = 1, Izena = "L1" }, Mahaia = new Mahaia { Id = 1 } }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(erreserbak);
            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedErreserbak = Assert.IsType<List<ErreserbaDto>>(okResult.Value);
            Assert.Single(returnedErreserbak);
            Assert.Equal("Jon", returnedErreserbak[0].BezeroIzena);
        }

        [Fact]
        public void Sortu_OkItzultzenDu_ErreserbaBerriaSortzean()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            var dto = new ErreserbaSortuDto { BezeroIzena = "Ane", LangileaId = 1, MahaiakId = 1 };
            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Sortu(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            mockRepo.Verify(r => r.Add(It.IsAny<Erreserba>()), Times.Once);
        }

        [Fact]
        public void Update_NotFoundItzultzenDu_ErreserbaEzDeneanExistitzen()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            mockRepo.Setup(r => r.Get(1)).Returns((Erreserba?)null);
            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Update(1, new ErreserbaSortuDto());

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_OkItzultzenDu_ErreserbaEguneratzean()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            var erreserba = new Erreserba { Id = 1, BezeroIzena = "Zahar" };
            mockRepo.Setup(r => r.Get(1)).Returns(erreserba);
            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            var dto = new ErreserbaSortuDto { BezeroIzena = "Berria", MahaiakId = 2 };

            // Act
            var result = controller.Update(1, dto);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepo.Verify(r => r.Update(It.IsAny<Erreserba>()), Times.Once);
            Assert.Equal("Berria", erreserba.BezeroIzena);
        }

        [Fact]
        public void Delete_OkItzultzenDu_ErreserbaEzabatzean()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            var erreserba = new Erreserba { Id = 1 };
            mockRepo.Setup(r => r.Get(1)).Returns(erreserba);
            mockRepo.Setup(r => r.ExecuteSerializableTransaction(It.IsAny<Action>())).Callback<Action>(a => a());
            mockEskariaRepo.Setup(r => r.GetAll()).Returns(new List<Eskaria>());

            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.True((bool)okResult.Value);
            mockRepo.Verify(r => r.Delete(erreserba), Times.Once);
        }

        [Fact]
        public void Delete_BadRequestItzultzenDu_ErroreaGertatzean()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            mockRepo.Setup(r => r.ExecuteSerializableTransaction(It.IsAny<Action>())).Throws(new Exception("Errorea"));

            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Errorea", badRequestResult.Value);
        }

        [Fact]
        public void Ordaindu_NotFoundItzultzenDu_ErreserbaEzDeneanExistitzen()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out var mockSession);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            var mockTx = new Mock<ITransaction>();
            mockSession.Setup(s => s.BeginTransaction()).Returns(mockTx.Object);
            mockSession.Setup(s => s.Get<Erreserba>(It.IsAny<int>())).Returns((Erreserba?)null);
            mockRepo.Setup(r => r.OpenSession()).Returns(mockSession.Object);

            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Ordaindu(new ErreserbaOrdainduDto { ErreserbaId = 1 });

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeskargatuTicket_NotFoundItzultzenDu_TiketaEzDeneanExistitzen()
        {
            // Arrange
            var mockSF = CreateMockSessionFactory(out _);
            var mockRepo = new Mock<ErreserbaRepository>(mockSF.Object);
            var mockEskariaRepo = new Mock<EskariaRepository>(mockSF.Object);
            var mockProduktuaRepo = new Mock<ProduktuaRepository>(mockSF.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSF.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSF.Object);

            mockRepo.Setup(r => r.GetAll()).Returns(new List<Erreserba>());
            var controller = new ErreserbakController(mockRepo.Object, mockEskariaRepo.Object, mockProduktuaRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.DeskargatuTicket(1);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }
    }
}
