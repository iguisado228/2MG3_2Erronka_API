using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using _2Erronka_API.Controllers;
using _2Erronka_API.Modeloak;
using _2Erronka_API.DTOak;
using _2Erronka_API.Repositorioak;
using NHibernate;
using System.Collections.Generic;
using System.Linq;

namespace _2Erronka_API.Testak
{
    public class ProduktuakControllerTest
    {
        [Fact]
        public void GetAll_OkItzultzenDu_ProduktuakExistitzenDirenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<ProduktuaRepository>(mockSessionFactory.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSessionFactory.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            var produktuak = new List<Produktua>
            {
                new Produktua { Id = 1, Izena = "CocaCola", Prezioa = 2.5, MotaId = 1, Stock = 50 },
                new Produktua { Id = 2, Izena = "Tortilla pintxoa", Prezioa = 1.8, MotaId = 1, Stock = 20 }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(produktuak);
            var controller = new ProduktuakController(mockRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduktuak = Assert.IsType<List<ProduktuaDto>>(okResult.Value);
            Assert.Equal(2, returnedProduktuak.Count);
            Assert.Equal("CocaCola", returnedProduktuak[0].Izena);
            Assert.Equal("Tortilla pintxoa", returnedProduktuak[1].Izena);
        }

        [Fact]
        public void Get_NotFoundItzultzenDu_ProduktuaEzDeneanExistitzen()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<ProduktuaRepository>(mockSessionFactory.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSessionFactory.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            mockRepo.Setup(r => r.Get(It.IsAny<int>())).Returns((Produktua?)null);
            var controller = new ProduktuakController(mockRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Get(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_OkItzultzenDu_ProduktuaExistitzenDenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<ProduktuaRepository>(mockSessionFactory.Object);
            var mockProduktuaOsagaiaRepo = new Mock<ProduktuaOsagaiaRepository>(mockSessionFactory.Object);
            var mockOsagaiaRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            var produktua = new Produktua { Id = 1, Izena = "CocaCola", Prezioa = 2.5, MotaId = 1, Stock = 50 };
            mockRepo.Setup(r => r.Get(1)).Returns(produktua);
            var controller = new ProduktuakController(mockRepo.Object, mockProduktuaOsagaiaRepo.Object, mockOsagaiaRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduktua = Assert.IsType<ProduktuaDto>(okResult.Value);
            Assert.Equal(1, returnedProduktua.Id);
            Assert.Equal("CocaCola", returnedProduktua.Izena);
        }
    }
}
