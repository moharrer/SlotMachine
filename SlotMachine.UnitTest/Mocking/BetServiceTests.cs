using Core;
using Moq;
using NUnit.Framework;
using SlotMachine.Algo;
using SlotMachine.Infrastructure;
using SlotMachine.Services;
using System;
using System.Threading.Tasks;

namespace SlotMachine.UnitTests.Mocking
{
    [TestFixture]
    public class BetServiceTests
    {
        private IBetService betService;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private Mock<ISlotMachine> mockSlotMachine;
        private Mock<IWinStrategy> mockWinStrategy;
        private Mock<IConfigurationService> mockConfigurationService;
        private Mock<IPlayerService> mockPlayerService;
        private Player player;

        private const int DefultMatrixHeigth = 3;
        private const int DefultMatrixLength = 5;

        [SetUp]
        public void SetUp()
        {
            mockUnitOfWork = new Mock<IUnitOfWork>();
            mockSlotMachine = new Mock<ISlotMachine>();
            mockWinStrategy = new Mock<IWinStrategy>();

            mockConfigurationService = new Mock<IConfigurationService>();
            mockPlayerService = new Mock<IPlayerService>();


            var appConfig = new Core.AppConfiguration();
            appConfig.SlotMachine.SlotHeight = DefultMatrixHeigth;
            appConfig.SlotMachine.SlotLength = DefultMatrixLength;

            mockConfigurationService.Setup(a => a.GetConfiguration()).Returns(appConfig);

            var slotResponse = new Algo.SlotMachine().Spin(DefultMatrixHeigth, DefultMatrixLength);
            mockSlotMachine.Setup(a => a.Spin(DefultMatrixHeigth, DefultMatrixLength)).Returns(slotResponse);
            mockWinStrategy.Setup(a => a.CalculateWin(slotResponse));


            betService = new BetService(mockSlotMachine.Object,
                mockWinStrategy.Object,
                mockConfigurationService.Object,
                mockPlayerService.Object,
                mockUnitOfWork.Object);
        }

        [TestCase("a@a.com", 100)]
        [TestCase("b@b.com", 10)]
        [Test]
        public async Task BetServiceBetNowAsync_WhenCall_GetSuccessfullResponse(string email, decimal betAmount)
        {
            //Arrange
            player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = email, Balance = 100 };

            mockUnitOfWork.Setup(a => a.StartTransaction());
            mockPlayerService.Setup(a => a.GetPlayerByEmailAsync(email)).Returns(Task.Run(() => player));

            mockUnitOfWork.Setup(a => a.Commit());

            var result = await betService.BetNowAsync(email, betAmount);

            mockUnitOfWork.Verify(uow => uow.Commit(), Times.Once);

        }

        
        [Test]
        public void BetServiceBetNowAsync_PlayerDoesNotExist_ShouldRaiseException()
        {
            //Arrange

            mockUnitOfWork.Setup(a => a.StartTransaction());
            mockPlayerService.Setup(a => a.GetPlayerByEmailAsync(It.IsAny<string>())).Returns(Task.FromResult<Player>(null));

            Assert.ThrowsAsync<UserFriendlyException>(async () => await betService.BetNowAsync(It.IsAny<string>(), It.IsAny<decimal>()));

            mockUnitOfWork.Verify(a=> a.Commit(), Times.Never);
        }

        [TestCase(100, 1250)]
        [TestCase(0, 1250)]
        [Test]
        public void BetServiceBetNowAsync_PlayerBetAmountIsGreaterThanBalance_ShouldRaiseException(int balance, decimal betAmount)
        {
            //Arrange
            player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = It.IsAny<string>(), Balance = balance };

            mockPlayerService.Setup(a => a.GetPlayerByEmailAsync(It.IsAny<string>())).Returns(Task.Run(() => player));

            Assert.ThrowsAsync<UserFriendlyException>(async () => await betService.BetNowAsync(It.IsAny<string>(), betAmount));

            mockUnitOfWork.Verify(a => a.Commit(), Times.Never);
        }


        [Test]
        public void BetServiceBetNowAsync_WhenUpdateBalanceRaiseException_TransactionMustRollBack()
        {
            //Arrange
            player = new Player() { Id = MongoDB.Bson.ObjectId.GenerateNewId(), Email = It.IsAny<string>(), Balance = It.IsAny<decimal>() };

            mockUnitOfWork.Setup(a => a.StartTransaction());

            mockPlayerService.Setup(a =>  a.UpdateBalanceAsync(It.IsAny<string>(), It.IsAny<decimal>())).Throws<Exception>();

            mockPlayerService.Setup(a => a.GetPlayerByEmailAsync(It.IsAny<string>())).Returns(Task.Run(() => player));

            Assert.ThrowsAsync<UserFriendlyException>(async () => await betService.BetNowAsync(It.IsAny<string>(), It.IsAny<decimal>()));
            //Rollback must call
            mockUnitOfWork.Verify(uow => uow.Rollback(), Times.Once);

        }

    }
}
