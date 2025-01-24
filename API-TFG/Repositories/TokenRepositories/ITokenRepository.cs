using API_TFG.Models.Domain;

namespace API_TFG.Repositories.TokenRepositories
{
    public interface ITokenRepository
    {
        Task<string> CreateJWTToken(User user, List<string> roles);
    }
}
