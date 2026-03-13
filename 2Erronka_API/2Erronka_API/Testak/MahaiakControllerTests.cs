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
    public class MahaiakControllerTests
    {
        [Fact]
        public void GetAll_OkItzultzenDu_MahaiakExistitzenDirenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<MahaiaRepository>(mockSessionFactory.Object);

            var mahaiak = new List<Mahaia>
            {
                new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "Egongela" },
                new Mahaia { Id = 2, Zenbakia = 2, PertsonaKopurua = 2, Kokapena = "Terraza" }
            };

            mockRepo.Setup(r => r.GetAll()).Returns(mahaiak);
            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.GetAll();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedMahaiak = Assert.IsType<List<MahaiaDto>>(okResult.Value);
            Assert.Equal(2, returnedMahaiak.Count);
            Assert.Equal(1, returnedMahaiak[0].Zenbakia);
            Assert.Equal(2, returnedMahaiak[1].Zenbakia);
        }

        [Fact]
        public void Create_OkItzultzenDu_MahaiaBerriaSortzean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<MahaiaRepository>(mockSessionFactory.Object);

            var dto = new MahaiaDto { Zenbakia = 3, PertsonaKopurua = 6, Kokapena = "Egongela" };
            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.Create(dto);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepo.Verify(r => r.Add(It.IsAny<Mahaia>()), Times.Once);
        }

        [Fact]
        public void Update_NotFoundItzultzenDu_MahaiaEzDeneanExistitzen()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<MahaiaRepository>(mockSessionFactory.Object);

            mockRepo.Setup(r => r.Get(1)).Returns((Mahaia?)null);
            var controller = new MahaiakController(mockRepo.Object);
            var dto = new MahaiaDto { Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "Egongela" };

            // Act
            var result = controller.Update(1, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Update_OkItzultzenDu_MahaiaEguneratzean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<MahaiaRepository>(mockSessionFactory.Object);

            var mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "Egongela" };
            mockRepo.Setup(r => r.Get(1)).Returns(mahaia);
            var controller = new MahaiakController(mockRepo.Object);
            var dto = new MahaiaDto { Zenbakia = 1, PertsonaKopurua = 5, Kokapena = "Egongela Berria" };

            // Act
            var result = controller.Update(1, dto);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepo.Verify(r => r.Update(It.IsAny<Mahaia>()), Times.Once);
            Assert.Equal(5, mahaia.PertsonaKopurua);
            Assert.Equal("Egongela Berria", mahaia.Kokapena);
        }

        [Fact]
        public void Delete_NotFoundItzultzenDu_MahaiaEzDeneanExistitzen()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<MahaiaRepository>(mockSessionFactory.Object);

            mockRepo.Setup(r => r.Get(1)).Returns((Mahaia?)null);
            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void Delete_OkItzultzenDu_MahaiaEzabatzean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<MahaiaRepository>(mockSessionFactory.Object);

            var mahaia = new Mahaia { Id = 1, Zenbakia = 1, PertsonaKopurua = 4, Kokapena = "Egongela" };
            mockRepo.Setup(r => r.Get(1)).Returns(mahaia);
            var controller = new MahaiakController(mockRepo.Object);

            // Act
            var result = controller.Delete(1);

            // Assert
            Assert.IsType<OkResult>(result);
            mockRepo.Verify(r => r.Delete(mahaia), Times.Once);
        }
    }
}
