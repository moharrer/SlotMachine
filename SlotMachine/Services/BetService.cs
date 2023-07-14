using SlotMachine.Infrastructure;
using SlotMachine.Services;

namespace SlotMachine.Algo
{
    public interface IBetService
    {
        Task<(int[,], decimal)> BetNowAsync(string playerUserName, decimal betAmount);
    }

    public class BetService : IBetService
    {
        private readonly ISlotMachine slotMachine;
        private readonly IWinStrategy winCalculator;
        private readonly IConfigurationService configurationService;
        private readonly IPlayerService playerService;
        private readonly IUnitOfWork unitOfWork;

        public BetService(ISlotMachine slotMachine, 
            IWinStrategy winCalculator, 
            IConfigurationService configurationService, 
            IPlayerService playerService, 
            IUnitOfWork unitOfWork)
        {
            this.slotMachine = slotMachine;
            this.winCalculator = winCalculator;
            this.configurationService = configurationService;
            this.playerService = playerService;
            this.unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Start a new Bet
        /// </summary>
        /// <returns>
        /// result 1: result of the spin in the form of a multi-dimensional integer
        /// result 2: calculated win value
        /// </returns>
        public async Task<(int[,], decimal)> BetNowAsync(string playerEmail, decimal betAmount)
        {
            //We need to open a transaction because we have 2 UpdateBalanceAsync one for deducte and other for increase the balance
            unitOfWork.StartTransaction();

            //The length and height of the array (size of the slot machine) is configurable, and the configuration value is stored in the database.
            var config = configurationService.GetConfiguration();

            var rows = config.SlotMachine.SlotHeight;
            var columns = config.SlotMachine.SlotLength;
            
            try
            {

                if (betAmount <= 0)
                {
                    throw new UserFriendlyException("Bet Amount must be greater than zero.");
                }

                var player = await playerService.GetPlayerByEmailAsync(playerEmail);

                if (player == null)
                {
                    throw new UserFriendlyException("Invalid player detected!");
                }

                //The requesting player’s balance should be checked, if the bet amount is higher than the user’s balance the request should not continue
                if (player.Balance < betAmount)
                {
                    throw new UserFriendlyException("Insufficient Blance!");
                }

                // The bet should be deducted from the requesting player’s balance
                await playerService.UpdateBalanceAsync(playerEmail, betAmount * -1);

                //The result array of the slot machine should be randomly selected as a single digit integer (0-9) for each array cell. 
                var spinResult = slotMachine.Spin(rows, columns);

                // The win should be calculated as the game bet multiplied by the sum of consecutive identical digits (where series length > 2) starting from position zero on a specific win line
                var win = winCalculator.CalculateWin(spinResult);

                //The Win should be added to the player balance.
                await playerService.UpdateBalanceAsync(playerEmail, win * betAmount);

                //Commit transaction
                unitOfWork.Commit();

                return (spinResult, win);
            }
            catch (Exception ex)
            {
                unitOfWork.Rollback();

                throw ex;
            }
            

        }


    }
}
