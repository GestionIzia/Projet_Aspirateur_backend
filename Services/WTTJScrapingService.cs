using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Aspi_backend.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Aspi_backend.Services
{
    public class WTTJScrapingService : IWTTJScrapingService
    {
        private readonly HttpClient _httpClient;

        public WTTJScrapingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JobOffer>> ScrapeWTTJOffers() // Renommer la méthode ici
        {
            List<JobOffer> WTTJjobOffers = new List<JobOffer>();

            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless"); // Active le mode headless
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);

            try
            {

                driver.Navigate().GoToUrl("https://www.welcometothejungle.com/fr/jobs?query=&refinementList%5Boffices.country_code%5D%5B%5D=FR&refinementList%5Bcontract_type%5D%5B%5D=apprenticeship&refinementList%5Boffices.state%5D%5B%5D=%C3%8Ele-de-France&page=1&aroundQuery=%C3%8Ele-de-France%2C%20France");

                await Task.Delay(5000); // Utiliser Task.Delay au lieu de Thread.Sleep dans un contexte asynchrone

                var cardElements = driver.FindElements(By.XPath("//*[@id='pages_jobs']/div[2]/div/ul/li[position() <= 15]/div/div"));

                if (cardElements != null)
                {
                    Console.WriteLine("Offres en apprentissage sur Welcome to the Jungle :");
                    foreach (var cardElement in cardElements)
                    {
                        try
                        {
                            JobOffer jobOffer = new JobOffer
                            {
                                //*[@id="pages_jobs"]/div[2]/div/ul/li[1]/div/div/div/div[2]/div[3]/button/span
                                WebSite = "WTTJ",
                                CompanieName = cardElement.FindElement(By.XPath("./div/div[1]/span")).Text,
                                JobTitle = cardElement.FindElement(By.XPath("./div/div[2]/a/h4/div")).Text,
                                ContractType = cardElement.FindElement(By.XPath("./div/div[2]/div[2]/div/span")).Text,
                                Location = cardElement.FindElement(By.XPath("./div/div[2]/div[1]/p/span/span")).Text,
                                Date = cardElement.FindElement(By.XPath("./div/div[2]/div[3]/div[1]/p/time/span")).Text,
                                UrlOffer = cardElement.FindElement(By.XPath("./a")).GetAttribute("href"),
                                Type = 1 //Type 1 = Jobboard
                            };

                            WTTJjobOffers.Add(jobOffer);


                        }
                        catch (Exception ex)
                        {
                            // En cas d'erreur, ajoutez une offre d'emploi avec des valeurs par défaut
                            Console.WriteLine($"An error occurred while processing job offer: {ex.Message}");
                            WTTJjobOffers.Add(new JobOffer
                            {
                                WebSite = "WTTJ",
                                CompanieName = cardElement.FindElement(By.XPath("./div/div[1]/span")).Text,
                                JobTitle = cardElement.FindElement(By.XPath("./div/div[2]/a/h4/div")).Text,
                                ContractType = cardElement.FindElement(By.XPath("./div/div[2]/div[2]/div/span")).Text,
                                Location = cardElement.FindElement(By.XPath("./div/div[2]/div[1]/p/span/span")).Text,
                                Date = "???",
                                UrlOffer = cardElement.FindElement(By.XPath("./a")).GetAttribute("href"),
                                Type = 1 //Type 1 = Jobboard
                            });
                        }

                    }
                }
            }
            finally
            {
                driver.Quit();
            }

            return WTTJjobOffers;
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
