using System.ComponentModel.DataAnnotations;

namespace WebApi.Dto
{
    public class LoginRequest
    {
        [Required, EmailAddress]
        public string Correo { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;
        public string? TotpCode { get; set; }
    }

    public class LoginResponse
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        public int ExpiresInSeconds { get; set; }
        public bool RequiresTwoFactor { get; set; }
    }

    public class RefreshRequest
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }

    public class TwoFactorSetupResponse
    {
        public string Secret { get; set; } = string.Empty;
        public string ProvisioningUri { get; set; } = string.Empty;
    }

    public class TwoFactorVerifyRequest
    {
        [Required, StringLength(6, MinimumLength = 6)]
        public string Code { get; set; } = string.Empty;
    }
}
