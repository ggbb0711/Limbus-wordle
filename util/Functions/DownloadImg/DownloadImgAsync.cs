namespace Limbus_wordle.util.Functions.DownloadImg
{
    public class DownloadImgAsync
    {
        public static async Task Download(string url, string savePath)
        {
            using var httpClient = new HttpClient();
            try
            {
                // Send a GET request to the image URL
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Throw if not a success code

                // Read the image data as a byte array
                var imageBytes = await response.Content.ReadAsByteArrayAsync();

                // Save the byte array to a file
                await File.WriteAllBytesAsync(savePath, imageBytes);

                Console.WriteLine($"Image downloaded and saved to {savePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
        }
    }
}