using System.Security.Cryptography;

namespace Movie_Ticket.Helpers
{
    public class RefreshTokenCreator
    {
        public static string CreateRefreshToken()
        {
            var bytes = RandomNumberGenerator.GetBytes(32);
            return Convert.ToBase64String(bytes);
        }
    }
}
