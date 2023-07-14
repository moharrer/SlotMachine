using Core;
using Moq;
using NUnit.Framework;
using SlotMachine.Infrastructure;
using SlotMachine.Services;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SlotMachine.UnitTests.Mocking
{
    [TestFixture]
    public class PlayerServiceTests
    {
        private IPlayerService playerService;
        private Mock<IRepository<Player>> mockPlayerRepository;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Player player;

        [SetUp]
        public void Setup()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockPlayerRepository = new Mock<IRepository<Player>>();

            playerService = new PlayerService(mockPlayerRepository.Object, mockUnitOfWork.Object);
        }


        [TestCase("a@a.com")]
        [TestCase("x@a.com")]
        [Test]
        public void RegisterAsync_WhenCall_ShouldRegisterSuccessfully(string email)
        {
            player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = email, Balance = 100 };

            mockPlayerRepository.Setup(a => a.FindOne(a => a.Email == email)).Returns(player);

            playerService.RegisterAsync(player);

            mockPlayerRepository.Verify(a => a.InsertOneAsync(player));

        }

        [TestCase("invalidEmail")]
        [TestCase("a@a")]
        [TestCase("")]
        [Test]
        public void RegisterAsync_InvalidEmail_ShouldThrowAnException(string email)
        {
            player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = email, Balance = 100 };

            Assert.ThrowsAsync<UserFriendlyException>(async () => await playerService.RegisterAsync(player));

        }

        [TestCase("a@a.com")]
        [TestCase("x@a.com")]
        [Test]
        public void RegisterAsync_PlayerAlreadyExist_ShouldThrowAnException(string email)
        {
            player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = email, Balance = 100 };
            
            mockPlayerRepository.Setup(a => a.FindOneAsync(It.IsAny<Expression<Func<Player, bool>>>())).Returns(Task.FromResult(player));


            Assert.ThrowsAsync<UserFriendlyException>(async () => await playerService.RegisterAsync(player));

        }

        //[TestCase("a@a.com", 500)]
        //[TestCase("x@a.com", 60)]
        //[Test]
        //public void UpdateBalanceAsync_WhenCall_UpdateBalanceSuccessfully(string email, decimal amount)
        //{
        //    player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = email, Balance = 100 };

        //    mockPlayerRepository.Setup(a => a.FindOneAsync(It.IsAny<Expression<Func<Player, bool>>>())).Returns(Task.FromResult(player));
        //    mockPlayerRepository.Setup(a => a.ReplaceOneAsync(player));

        //    playerService.UpdateBalanceAsync(email, amount);

        //    mockPlayerRepository.Verify(a => a.ReplaceOneAsync(player));

        //}



    }
}
