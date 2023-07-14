using Microsoft.AspNetCore.Mvc;
using SlotMachine.Algo;
using SlotMachine.Services;
using SlotMachine.ViewModel;

namespace SlotMachine.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SlotMachineController : ControllerBase
    {
        private readonly ILogger<SlotMachineController> _logger;
        private readonly IBetService betService;
        private readonly IPlayerService playerService;

        public SlotMachineController(ILogger<SlotMachineController> logger, IBetService betService, IPlayerService playerService)
        {
            _logger = logger;
            this.betService = betService;
            this.playerService = playerService;
        }


        [HttpPost("spin", Name = "Spin")]
        public async Task<IActionResult> SpinAsync([FromBody] SlotMachineSpinInput model)
        {
            if (model == null)
            {
                return BadRequest("Invalid input detected.");
            }

            //Bet
            var (spinResult, win) = await betService.BetNowAsync(model.Email, model.BetAmount);

            var player = await playerService.GetPlayerByEmailAsync(model.Email);

            var balance = player.Balance;

            var response = new SlotMachineResultModel(spinResult, win, balance);
            string json = Newtonsoft.Json.JsonConvert.SerializeObject(response);

            return Content(json);
        }
    }
}