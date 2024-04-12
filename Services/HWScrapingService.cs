using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Aspi_backend.Models;
using HtmlAgilityPack;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;

namespace Aspi_backend.Services
{
    public class HWScrapingService : IHWScrapingService
    {
        private readonly HttpClient _httpClient;

        public HWScrapingService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<JobOffer>> ScrapeHWOffers() // Renommer la méthode ici
        {
            List<JobOffer> HWjobOffers = new List<JobOffer>();

            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless"); // Active le mode headless
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);



            driver.Navigate().GoToUrl("https://www.hellowork.com/fr-fr/emploi/recherche.html?k=alternance&k_autocomplete=alternance&l=%C3%8Ele-de-France&l_autocomplete=http%3A%2F%2Fwww.rj.com%2Fcommun%2Flocalite%2Fregion%2F11&ray=50&c=Alternance&msa=&cod=all&d=all&c_idesegal=");
            await Task.Delay(5000); // Utiliser Task.Delay au lieu de Thread.Sleep dans un contexte asynchrone

            //IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            //js.ExecuteScript("document.getElementById('tarteaucitronClose').click();");

            //js.ExecuteScript("document.querySelector('.js-loadmore').click();");
            //Thread.Sleep(2000);

            var cardElements = driver.FindElements(By.XPath("/html/body/main/section/div/section/ul[1]/li[position() <= 20]"));


            Console.WriteLine("Offres en apprentissage sur Hello Work :");
            foreach (var cardElement in cardElements)
            {
                try
                {
                    JobOffer jobOffer = new JobOffer
                    {
                        WebSite = "HW",
                        UrlOffer = cardElement.FindElement(By.CssSelector("h3 a")).GetAttribute("href"),
                        Type = 2 //Type 2 = JobBoards
                    };

                    var htmlContent = await GetHtmlContentAsync(jobOffer.UrlOffer);
                    var (jobTitle, companieName, location, contractType) = ExtractJobDetails(htmlContent);

                    jobOffer.JobTitle = jobTitle;
                    jobOffer.CompanieName = companieName;
                    jobOffer.Location = location;
                    jobOffer.ContractType = contractType;

                    HWjobOffers.Add(jobOffer);

                }
                catch (Exception ex)
                {
                    // Gestion des erreurs
                }
                
            }
            driver.Quit();
            return HWjobOffers;
        }
    
        private (string JobTitle, string CompanyName, string Location, string ContractType) ExtractJobDetails(string htmlContent)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(htmlContent);

            var jobTitleNode = doc.DocumentNode.SelectSingleNode("/html/body/main/div[3]/div[3]/div/div[1]/div/div[1]/h1/span[1]");
            var companyNameNode = doc.DocumentNode.SelectSingleNode("/html/body/main/div[3]/div[3]/div/div[1]/div/div[1]/h1/span[2]");
            var locationNode = doc.DocumentNode.SelectSingleNode("//li[contains(@class, 'tw-tag-contract-s')]");
            var contractTypeNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'tw-text-grey')][2]");

            string jobTitle = jobTitleNode?.InnerText.Trim() ?? "Titre du poste non trouvé";
            string companyName = companyNameNode?.InnerText.Trim() ?? "Nom de l'entreprise non trouvé";
            string location = locationNode?.InnerText.Trim() ?? "Lieu non trouvé";
            string contractType = contractTypeNode?.InnerText.Trim() ?? "Type de contrat non trouvé";

            return (jobTitle, companyName, location, contractType);
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
                throw; // Retourner null en cas d'erreur
            }
        }
    }
}
