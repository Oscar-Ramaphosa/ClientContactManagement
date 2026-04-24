namespace ClientContactManagement.Services
{
    public interface IClientCodeGeneratorService
    {
        //this Generates a unique client code based on the client name
        Task<string> GenerateClientCodeAsync(string clientName);
    }
}
