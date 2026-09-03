namespace WebApi.Interface
{
    public interface IRefreshTokenService
    {
        Task<string> CreateAsync(int idUsuario, TimeSpan lifetime);

        Task<int?> ValidateAsync(string plainTextToken);

        Task RevokeAsync(string plainTextToken);

        Task RevokeAllForUserAsync(int idUsuario);
    }
}
