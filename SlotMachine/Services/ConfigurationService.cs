using Core;
using SlotMachine.Infrastructure;
using SlotMachine.Validations;

namespace SlotMachine.Services
{
    public interface IConfigurationService
    {
        AppConfiguration GetConfiguration();
        Task SetSlotMachineDimentionAsync(int length, int height);
    }
    public class ConfigurationService : IConfigurationService
    {
        private readonly IRepository<AppConfiguration> configurationRepository;

        public ConfigurationService(IRepository<AppConfiguration> configurationRepository)
        {
            this.configurationRepository = configurationRepository;
        }


        public AppConfiguration GetConfiguration()
        {
            var config = configurationRepository.AsQueryable().FirstOrDefault();

            if (config == null)
            {
                return new AppConfiguration();
            }

            return config;
        }

        public async Task SetSlotMachineDimentionAsync(int length, int height)
        {
            //The length and height of the array (size of the slot machine) is configurable, and the configuration value is stored in the database.

            var isValid = ConfiguratonValidation.DimentionIsValid(length, height);
            if (!isValid)
                throw new UserFriendlyException("The length and height must be greater than 0.");

            var config = configurationRepository.AsQueryable().FirstOrDefault();

            if (config == null)
            {
                await configurationRepository.InsertOneAsync(new AppConfiguration() { SlotMachine = new SlotMachineConfiguration() { SlotLength = length, SlotHeight = height } });
            }
            else
            {
                if (config.SlotMachine == null)
                    config.SlotMachine = new SlotMachineConfiguration();

                config.SlotMachine.SlotLength = length;
                config.SlotMachine.SlotHeight = height;

                await configurationRepository.ReplaceOneAsync(config);
            }

        }

    }
}
