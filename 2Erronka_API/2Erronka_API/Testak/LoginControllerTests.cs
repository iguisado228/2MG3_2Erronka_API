using Microsoft.AspNetCore.Mvc;
using Xunit;
using Moq;
using _2Erronka_API.Controllers;
using _2Erronka_API.Domain;
using _2Erronka_API.DTOak;
using _2Erronka_API.Repositorioak;
using NHibernate;

namespace _2Erronka_API.Testak
{
    public class LoginControllerTests
    {
        // "1234" pasahitzaren SHA256 hasha
        private const string ZuzenaHash = "03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4";

        [Fact]
        public void Post_NotFoundItzultzenDu_LangileKodeaEzDeneanExistitzen()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<LangileaRepository>(mockSessionFactory.Object);

            var controller = new LoginController(mockRepo.Object);
            var request = new LoginRequest { Langile_kodea = 999, Pasahitza = "1234" };
            
            mockRepo.Setup(repo => repo.GetByKodea(request.Langile_kodea))
                     .Returns((Langilea?)null);

            // Act
            var result = controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginErantzuna>(okResult.Value);
            
            Assert.False(response.Ok);
            Assert.Equal("not_found", response.Code);
        }

        [Fact]
        public void Post_BadPasswordItzultzenDu_PasahitzarenHashaOkerraDenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<LangileaRepository>(mockSessionFactory.Object);

            var controller = new LoginController(mockRepo.Object);
            var request = new LoginRequest { Langile_kodea = 101, Pasahitza = "okerra" };
            var langilea = new Langilea { Langile_kodea = 101, Pasahitza = ZuzenaHash };
            
            mockRepo.Setup(repo => repo.GetByKodea(request.Langile_kodea))
                     .Returns(langilea);

            // Act
            var result = controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginErantzuna>(okResult.Value);
            
            Assert.False(response.Ok);
            Assert.Equal("bad_password", response.Code);
        }

        [Fact]
        public void Post_ForbiddenItzultzenDu_LanpostuakBaimenikEzDuenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<LangileaRepository>(mockSessionFactory.Object);

            var controller = new LoginController(mockRepo.Object);
            var request = new LoginRequest { Langile_kodea = 101, Pasahitza = "1234" };
            var lanpostua = new Lanpostua { Lanpostu_izena = "Sukaldaria" };
            var langilea = new Langilea 
            { 
                Langile_kodea = 101, 
                Pasahitza = ZuzenaHash,
                Lanpostua = lanpostua
            };
            
            mockRepo.Setup(repo => repo.GetByKodea(request.Langile_kodea))
                     .Returns(langilea);

            // Act
            var result = controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginErantzuna>(okResult.Value);
            
            Assert.False(response.Ok);
            Assert.Equal("forbidden", response.Code);
        }

        [Fact]
        public void Post_OkItzultzenDu_DatuakZuzenakDirenean()
        {
            // Arrange
            var mockSessionFactory = new Mock<ISessionFactory>();
            var mockSession = new Mock<NHibernate.ISession>();
            mockSessionFactory.Setup(sf => sf.GetCurrentSession()).Returns(mockSession.Object);
            var mockRepo = new Mock<LangileaRepository>(mockSessionFactory.Object);

            var controller = new LoginController(mockRepo.Object);
            var request = new LoginRequest { Langile_kodea = 101, Pasahitza = "1234" };
            var lanpostua = new Lanpostua { Id = 1, Lanpostu_izena = "Gerentea" };
            var langilea = new Langilea 
            { 
                Id = 1,
                Izena = "Ane",
                Langile_kodea = 101, 
                Pasahitza = ZuzenaHash,
                Lanpostua = lanpostua
            };
            
            mockRepo.Setup(repo => repo.GetByKodea(request.Langile_kodea))
                     .Returns(langilea);

            // Act
            var result = controller.Login(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsType<LoginErantzuna>(okResult.Value);
            
            Assert.True(response.Ok);
            Assert.Equal("ok", response.Code);
            Assert.Equal("Ane", response.Data.Izena);
        }
    }
}
