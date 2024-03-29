using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Aspi_backend.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Aspi_backend.Services
{
    public class WTTJScrapingService : IScrapingService
    {
        private readonly HttpClient _httpClient;

        public WTTJScrapingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JobOffer>> ScrapeJobOffers(string jobBoard)
        {
            List<JobOffer> jobOffers = new List<JobOffer>();

            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless"); // Active le mode headless
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);

            try
            {
                driver.Navigate().GoToUrl("https://www.welcometothejungle.com/fr/jobs?query=apprentissage");
                await Task.Delay(5000); // Utiliser Task.Delay au lieu de Thread.Sleep dans un contexte asynchrone

                var cardElements = driver.FindElements(By.XPath("//*[@id='pages_jobs']/div[2]/div/ul/li[position() <= 12]/div/div"));

                if (cardElements != null)
                {
                    Console.WriteLine("Offres en apprentissage sur Welcome to the Jungle :");
                    foreach (var cardElement in cardElements)
                    {
                        JobOffer jobOffer = new JobOffer
                        {
                            CompanieName = cardElement.FindElement(By.XPath("./div/div[1]/span")).Text,
                            JobTitle = cardElement.FindElement(By.XPath("./div/div[2]/a/h4/div")).Text,
                            ContractType = cardElement.FindElement(By.XPath("./div/div[2]/div[2]/div/span")).Text,
                            Location = cardElement.FindElement(By.XPath("./div/div[2]/div[1]/p/span/span")).Text,
                            Date = cardElement.FindElement(By.XPath("./div/div[2]/div[3]/div[1]/p/time/span")).Text,
                            UrlOffer = cardElement.FindElement(By.XPath("./a")).GetAttribute("href")
                        };

                        // Récupérer le contenu HTML de l'offre d'emploi et l'ajouter à l'objet JobOffer
                        jobOffer.HtmlContent = await GetHtmlContentAsync(jobOffer.UrlOffer);

                        jobOffers.Add(jobOffer);
                    }
                    //(new System.Collections.Generic.ICollectionDebugView<Aspi_backend.Models.JobOffer>(jobOffers).Items[0]).HtmlContent
                }
                else
                {
                    Console.WriteLine("Aucune offre en apprentissage trouvée sur Welcome to the Jungle.");
                }
            }
            finally
            {
                driver.Quit();
            }

            return jobOffers;
        }

        public async Task<string> GetHtmlContentAsync(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Une erreur s'est produite lors de la récupération du contenu de la page : {e.Message}");
                throw; // Propager l'exception pour une gestion ultérieure
            }
        }
    }
}
