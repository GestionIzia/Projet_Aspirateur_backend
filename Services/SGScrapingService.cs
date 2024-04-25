using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Aspi_backend.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Aspi_backend.Services
{
    public class SGScrapingService : ISGScrapingService
    {
        private readonly HttpClient _httpClient;

        public SGScrapingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JobOffer>> ScrapeSGOffers() // Renommer la méthode ici
        {
            List<JobOffer> SGjobOffers = new List<JobOffer>();

            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless"); // Active le mode headless
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);

            try
            {

                driver.Navigate().GoToUrl("https://careers.societegenerale.com/rechercher?refinementList[jobType][0]=APPRENTICESHIP");
                await Task.Delay(5000); // Utiliser Task.Delay au lieu de Thread.Sleep dans un contexte asynchrone

                var cardElements = driver.FindElements(By.CssSelector(".oJob-offer"));
                //*[@id="240006UD-fr"]/div[2]/a
                //*[@id="240005HK-fr"]/div[2]/a

                Console.WriteLine("Offres en apprentissage sur Société Générale :");
                foreach (var cardElement in cardElements.Take(15))
                {
                    try
                    {
                        JobOffer jobOffer = new JobOffer
                        {
                            WebSite = "SG",
                            CompanieName = "Société Générale", // Ici, vous pouvez utiliser un nom spécifique pour chaque entreprise si nécessaire
                            JobTitle = cardElement.FindElement(By.CssSelector(".hit-text > .text-extra-dark-gray")).Text,
                            ContractType = cardElement.FindElement(By.CssSelector(".tags > .alternance > li")).Text,
                            Location = cardElement.FindElement(By.CssSelector(".tags > .hit-details:nth-child(2) > li")).Text,
                            //Date = "hier", // Vous pouvez obtenir la date si elle est disponible sur la page
                            UrlOffer = cardElement.FindElement(By.CssSelector(".hit-text")).GetAttribute("href"),
                            Type = 2 //Type 1 = CareerCenter
                        };

                        SGjobOffers.Add(jobOffer);

                    }
                    catch (Exception ex)
                    {
                        // En cas d'erreur, ajoutez une offre d'emploi avec des valeurs par défaut
                        Console.WriteLine($"An error occurred while processing job offer: {ex.Message}");
                        SGjobOffers.Add(new JobOffer
                        {
                            WebSite = "SG",
                            CompanieName = "Société Générale",
                            JobTitle = "???",
                            Location = "???",
                            Date = "???",
                            UrlOffer = "???",
                            ContractType = "???",
                            //HtmlContent = mainContentElement.FindElement(By.XPath("div")).GetAttribute("outerHTML"),
                            Type = 2 //Type 1 = Jobboard
                        });
                    }
                }
            }
            finally
            {
                driver.Quit();
            }
            return SGjobOffers;
        }
        /*
 Infos a scrapper : //*[@id="offer-240004NE-en"]/section[2]
                    //*[@id="offer-240004NE-en"]/section[3]
 */
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
                throw; // Retourner null en cas d'erreur
            }
        }
    }
}
