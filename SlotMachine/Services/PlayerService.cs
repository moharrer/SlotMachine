using Core;
using SlotMachine.Infrastructure;
using SlotMachine.Validations;

namespace SlotMachine.Services
{
    public interface IPlayerService
    {
        Task<Player> GetPlayerByEmailAsync(string email);
        Task RegisterAsync(Player player);
        Task UpdateBalanceAsync(string email, decimal amount);
    }
    public class PlayerService : IPlayerService
    {
        private readonly IRepository<Player> playerRepository;
        private readonly IUnitOfWork unitOfWork;

        public PlayerService(IRepository<Player> playerRepository, IUnitOfWork unitOfWork)
        {
            this.playerRepository = playerRepository;
            this.unitOfWork = unitOfWork;
        }

        public async Task<Player> GetPlayerByEmailAsync(string email)
        {
            var session = unitOfWork.GetSession();
            if (session != null)
            {
                return await playerRepository.FindOneAsync(a => a.Email.ToLower() == email.ToLower(), session);
            }
            else
            {
                return await playerRepository.FindOneAsync(a => a.Email.ToLower() == email.ToLower());
            }
            
        }

        public async Task RegisterAsync(Player player)
        {
            //Validate player before registeration
            if (player == null)
                throw new UserFriendlyException("Invalid player detected!");


            if (string.IsNullOrEmpty(player.Email) || !EmailValidator.ValidateEmailAddress(player.Email))
            {
                throw new UserFriendlyException("The Email is not in the correct format.");
            }

            var dbPlayer = await playerRepository.FindOneAsync(a => a.Email == player.Email);

            if (dbPlayer != null)
                throw new UserFriendlyException("A player with this email already exists.");

            await playerRepository.InsertOneAsync(player);
        }

        public async Task UpdateBalanceAsync(string email, decimal amount)
        {

            var session = unitOfWork.GetSession();

            //Each player has own Lock to handle more concurrent player
            LockProvider<string> locker = new();
            string playerLockId = email;
            try
            {
                // Use lock to handle race conditions
                locker.Wait(playerLockId);

                var player = await GetPlayerByEmailAsync(email);
                if (player == null)
                {
                    throw new UserFriendlyException("Invalid player detected!");
                }

                if (player.Balance + amount < 0)
                {
                    throw new UserFriendlyException("Insufficient Balance");
                }

                player.AddBalance(amount);

                if (session != null)
                {
                    await playerRepository.ReplaceOneAsync(player, session);
                }
                else
                {
                    await playerRepository.ReplaceOneAsync(player);
                }
            }
           
            finally
            {
                
                locker.Release(playerLockId);
            }

        }

    }
}
