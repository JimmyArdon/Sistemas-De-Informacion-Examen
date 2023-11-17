namespace WebApiAutores.Services
{
    public interface IWebPurifyService
    {
        Task<string> CheckForProfanity(string text);
    }
}
