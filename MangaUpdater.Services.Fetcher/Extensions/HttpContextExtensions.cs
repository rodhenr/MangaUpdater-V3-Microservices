namespace MangaUpdater.Services.Fetcher.Extensions;

public static class HttpContentExtensions
{
    public static async Task<T?> TryToReadJsonAsync<T>(this HttpContent httpContent) where T : class
    {
        try
        {
            return await httpContent.ReadFromJsonAsync<T>();
        }
        catch (Exception ex)
        {
            return null;
        }
    }
}