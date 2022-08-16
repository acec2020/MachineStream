namespace MachineStream
{
    public interface IMachineService
    {
        Task<Machine> GetMachine(string machineId);

        Task SaveMachine(string stream);
    }
}
