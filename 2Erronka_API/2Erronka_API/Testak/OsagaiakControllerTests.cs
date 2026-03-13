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
    public class OsagaiakControllerTests
    {
        [Fact]
        public void GetAll_OkItzultzenDu_OsagaiakExistitzenDirenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            var osagaiak = new List<Osagaia>
            {
                new Osagaia { Id = 1, Izena = "Tomatea", Prezioa = 1.5, Stock = 10, HornitzaileakId = 1 },
                new Osagaia { Id = 2, Izena = "Gazta", Prezioa = 2.0, Stock = 5, HornitzaileakId = 1 }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(osagaiak);
            var controller = new OsagaiakController(mockRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOsagaiak = Assert.IsType<List<OsagaiaDto>>(okResult.Value);
            Assert.Equal(2, returnedOsagaiak.Count);
            Assert.Equal("Tomatea", returnedOsagaiak[0].Izena);
            Assert.Equal("Gazta", returnedOsagaiak[1].Izena);
        }

        [Fact]
        public void Get_NotFoundItzultzenDu_OsagaiaEzDeneanExistitzen()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            mockRepo.Setup(r => r.Get(1)).Returns((Osagaia?)null);
            var controller = new OsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Get_OkItzultzenDu_OsagaiaExistitzenDenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<OsagaiaRepository>(mockSessionFactory.Object);

            var osagaia = new Osagaia { Id = 1, Izena = "Tomatea", Prezioa = 1.5, Stock = 10, HornitzaileakId = 1 };
            mockRepo.Setup(r => r.Get(1)).Returns(osagaia);
            var controller = new OsagaiakController(mockRepo.Object);

            // Act
            var result = controller.Get(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedOsagaia = Assert.IsType<OsagaiaDto>(okResult.Value);
            Assert.Equal(1, returnedOsagaia.Id);
            Assert.Equal("Tomatea", returnedOsagaia.Izena);
        }
    }
}
