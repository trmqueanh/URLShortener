using System.Security.Cryptography;
using System.Text;
using URLShortener.MVC.Data;
using URLShortener.MVC.Models;

namespace URLShortener.MVC.Services
{
    public interface IUrlShortenerService
    {
        string GenerateShortenedUrl(int characterCount);
        bool IsShortenedUrlUnique(string shortenedUrl, DataContext context);
    }

    public class UrlShortenerService : IUrlShortenerService
    {
        private const string AllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public string GenerateShortenedUrl(int characterCount)
        {
            if (characterCount < 3 || characterCount > 20)
                throw new ArgumentException("Character count must be between 3 and 20");

            var random = new Random();
            var result = new StringBuilder(characterCount);

            for (int i = 0; i < characterCount; i++)
            {
                result.Append(AllowedCharacters[random.Next(AllowedCharacters.Length)]);
            }

            return result.ToString();
        }

        public bool IsShortenedUrlUnique(string shortenedUrl, DataContext context)
        {
            return !context.HomeModels.Any(x => x.ShortenedUrl.ToLower() == shortenedUrl.ToLower());
        }
    }
} 