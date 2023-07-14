using Core;
using Microsoft.AspNetCore.Mvc;
using SlotMachine.Services;
using SlotMachine.ViewModel;

namespace SlotMachine.Controllers
{

    [Route("[controller]")]
    public class PlayerController : ControllerBase
    {
        private readonly IPlayerService playerService;

        public PlayerController(IPlayerService playerService)
        {
            this.playerService = playerService;
        }


        [HttpPost("registeration", Name = "Registeration")]
        public async Task<IActionResult> RegisterationAsync([FromBody] RegisterPlayerInputModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid input detected.");
            }

            var player = new Player() { Email = model.Email };

            await playerService.RegisterAsync(player);

            return NoContent();
        }

        
        [HttpPost("addbalance", Name = "IncreaseBalance")]
        public async Task<IActionResult> AddBalanceAsync(IncreaseBalanceInput model)
        {
            if (model == null)
            {
                return BadRequest("Invalid input detected.");
            }

            await playerService.UpdateBalanceAsync(model.Email, model.Amount);

            return NoContent();
        }


    }
}
