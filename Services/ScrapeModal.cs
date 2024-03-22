using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Aspi_backend.Services
{
    public class ScrapeModal
    {
        private string url = "https://www.welcometothejungle.com/fr/companies/securitesociale/jobs/apprentissage-teleconseiller_paris?q=751523ac4b48ef3ecd407a93e82caaf1&o=65ae3397-7166-406a-8b29-d8bacbcedcfa";

        public async Task<string> GetHtmlContentAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Une erreur s'est produite lors de la récupération du contenu de la page : {e.Message}");
                    return null;
                }
            }
        }
        public async Task ScrapePageAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode(); // Throw on error code
                    string htmlContent = await response.Content.ReadAsStringAsync();

                    // Maintenant, vous pouvez utiliser htmlContent pour afficher le contenu de la page externe sur votre site.
                    Console.WriteLine(htmlContent);
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine($"Une erreur s'est produite lors de la récupération du contenu de la page : {e.Message}");
                }
            }
        }
    }
}
