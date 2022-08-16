namespace MachineStream.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MachineController: ControllerBase
    {
        private readonly ILogger<MachineController> _logger;
        private readonly IMachineService _machineService;
        public MachineController(ILogger<MachineController> logger, IMachineService machineService)
        {
            _logger = logger;
            _machineService = machineService;
        }

        [HttpGet(Name = "GetMachine")]
        public async Task<Machine> Get(string machineId)
        {
            return await _machineService.GetMachine(machineId);
        }
    }
}
