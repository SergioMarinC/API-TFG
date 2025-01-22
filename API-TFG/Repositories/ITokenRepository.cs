using API_TFG.Models.Domain;

namespace API_TFG.Repositories
{
    public interface ITokenRepository
    {
        string CreateJWTToken(User user, List<string> roles);

    }
}
