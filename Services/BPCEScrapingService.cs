using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Aspi_backend.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Aspi_backend.Services
{
    public class BPCEScrapingService : IBPCEScrapingService
    {
        private readonly HttpClient _httpClient;

        public BPCEScrapingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JobOffer>> ScrapeBPCEOffers() // Renommer la méthode ici
        {
            List<JobOffer> BPCEjobOffers = new List<JobOffer>();

            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless"); // Active le mode headless
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);

            try
            {

                driver.Navigate().GoToUrl("https://recrutement.bpce.fr/offres-emploi?tax_contract=contrat-en-alternance,fr_contrat-en-alternance&tax_place=ile-de-france&external=false");
                await Task.Delay(5000); // Utiliser Task.Delay au lieu de Thread.Sleep dans un contexte asynchrone

                IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
                js.ExecuteScript("document.getElementById('tarteaucitronClose').click();");

                js.ExecuteScript("document.querySelector('.js-loadmore').click();");
                Thread.Sleep(2000);

                var cardElements = driver.FindElements(By.XPath("//*[@id=\"root\"]/div/div/main/div[2]/div/div/div[1]/div[1]/div[position() <= 15]"));


                Console.WriteLine("Offres en apprentissage sur BPCE :");
                foreach (var cardElement in cardElements.Take(20))
                {
                    try
                    {
                        JobOffer jobOffer = new JobOffer
                        {
                            WebSite = "BPCE",
                            CompanieName = "BPCE " + cardElement.FindElement(By.XPath("a/div[1]/div")).Text,
                            JobTitle = cardElement.FindElement(By.XPath("a")).Text,
                            ContractType = cardElement.FindElement(By.XPath("a/div[1]/ul/li[1]")).Text,
                            Location = cardElement.FindElement(By.XPath("a/div[1]/ul/li[3]")).Text,
                            Date = "Indisponible",
                            UrlOffer = cardElement.FindElement(By.XPath("a")).GetAttribute("href"),
                            Type = 1 //Type 1 = CareerCenter
                        };

                        BPCEjobOffers.Add(jobOffer);

                    }
                    catch (Exception ex)
                    {
                        // En cas d'erreur, ajoutez une offre d'emploi avec des valeurs par défaut
                        Console.WriteLine($"An error occurred while processing job offer: {ex.Message}");
                        BPCEjobOffers.Add(new JobOffer
                        {
                            WebSite = "BPCE",
                            CompanieName = "BPCE",
                            JobTitle = "???",
                            Location = "???",
                            Date = "???",
                            UrlOffer = "???",
                            ContractType = "???",
                            //HtmlContent = mainContentElement.FindElement(By.XPath("div")).GetAttribute("outerHTML"),
                            Type = 1 //Type 1 = Jobboard
                        });
                    }
                }
            }
            finally
            {
                driver.Quit();
            }
            return BPCEjobOffers;
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
