using Core;
using Microsoft.AspNetCore.Mvc;
using SlotMachine.Infrastructure;
using SlotMachine.Services;
using SlotMachine.Validations;
using SlotMachine.ViewModel;

namespace SlotMachine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationService configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            this.configurationService = configurationService;
        }

        [HttpPost(Name = "SetDimention")]
        public async Task<IActionResult> SetSlotMachineDimentionAsync([FromBody] SlotMachineDimentionInputModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid input detected.");
            }

            if (!ConfiguratonValidation.DimentionIsValid(model.Length, model.Heigth))
            {
                return BadRequest("The length must be greater then 2 and height must be greater than 0.");
            }

            await configurationService.SetSlotMachineDimentionAsync(model.Length, model.Heigth);

            return NoContent();
        }

    }
}
