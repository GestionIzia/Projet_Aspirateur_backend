using Aspi_backend.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Aspi_backend.Services
{
    public class LinkedinScrapingService
    {
        public async Task<List<JobOffer>> ScrapeJobOffers()
        {
            List<JobOffer> linkedinJobOffers = new List<JobOffer>();

            // Chemin du Microsoft Edge WebDriver
            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless");
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);
            var driverOffer = new EdgeDriver(edgeDriverPath, edgeOptions);

            try
            {
                // Charger la page de recherche d'offres en apprentissage sur LinkedIn
                driver.Navigate().GoToUrl("https://fr.linkedin.com/jobs/search?keywords=&locationId=&geoId=105015875&f_TPR=&f_E=1&position=1&pageNum=0");

                // Attendre que la page soit entièrement chargée (peut être remplacé par d'autres mécanismes d'attente)
                Thread.Sleep(15000);

                // Chemin d'accès pour les offres en apprentissage
                var cardElements = driver.FindElements(By.XPath("//*[@id=\"main-content\"]/section/ul/li[position() <= 12]"));

                // Recherche des éléments représentant chaque carte d'offre
                if (cardElements != null)
                {
                    foreach (var cardElement in cardElements)
                    {
                        JobOffer jobOffer = new JobOffer
                        {
                            WebSite = "Linkedin",
                            CompanieName = cardElement.FindElement(By.XPath("div/div[2]/h4/a")).Text,
                            JobTitle = cardElement.FindElement(By.XPath("div/a/span")).Text,
                            ContractType = "Alternance",
                            Location = cardElement.FindElement(By.XPath("div/div[2]/div/span")).Text,
                            Date = cardElement.FindElement(By.XPath("div/div[2]/div/time")).Text,
                            UrlOffer = cardElement.FindElement(By.XPath("div/a")).GetAttribute("href")
                        };

                        // Naviguer vers l'URL de l'offre pour obtenir le HTML complet de la page
                        driverOffer.Navigate().GoToUrl(jobOffer.UrlOffer);

                        // Attendre que la page soit entièrement chargée (peut être remplacé par d'autres mécanismes d'attente)
                        Thread.Sleep(10000);

                        // Localiser l'élément principal et obtenir son attribut outerHTML
                        var mainContentElement = driverOffer.FindElement(By.XPath("//*[@id=\"main-content\"]/section[1]/div"));
                        jobOffer.HtmlContent = mainContentElement.FindElement(By.XPath("div")).GetAttribute("outerHTML");

                        linkedinJobOffers.Add(jobOffer);
                    }
                }
                else
                {
                    Console.WriteLine("Aucune offre en apprentissage trouvée sur LinkedIn.");
                }
            }
            finally
            {
                driverOffer.Quit();
                driver.Quit();
            }

            return linkedinJobOffers;
        }
    }
}
