namespace WebApi.Interface
{
    public interface ITotpService
    {
        string GenerateSecret();
        string GenerateProvisioningUri(string secretBase32, string accountEmail, string issuer = "NexoVida");
        bool VerifyCode(string secretBase32, string code);
    }
}
