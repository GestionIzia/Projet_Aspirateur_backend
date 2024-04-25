using Aspi_backend.Models;
using OpenQA.Selenium;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
                Thread.Sleep(10000);

                // Chemin d'accès pour les offres en apprentissage
                var cardElements = driver.FindElements(By.XPath("//*[@id=\"main-content\"]/section/ul/li[position() <= 15]"));

                // Recherche des éléments représentant chaque carte d'offre
                foreach (var cardElement in cardElements)
                {
                    try
                    {
                        JobOffer jobOffer = new JobOffer
                        {
                            WebSite = "Linkedin",
                            CompanieName = cardElement.FindElement(By.XPath("div/div[2]/h4/a")).Text,
                            JobTitle = cardElement.FindElement(By.XPath("div/a/span")).Text,
                            Location = cardElement.FindElement(By.XPath("div/div[2]/div/span")).Text,
                            Date = cardElement.FindElement(By.XPath("div/div[2]/div/time")).Text,
                            UrlOffer = cardElement.FindElement(By.XPath("div/a")).GetAttribute("href"),
                            Type = 1 //Type 1 = Jobboard
                        };

                        // Naviguer vers l'URL de l'offre pour obtenir le HTML complet de la page
                        driverOffer.Navigate().GoToUrl(jobOffer.UrlOffer);

                        // Attendre que la page soit entièrement chargée (peut être remplacé par d'autres mécanismes d'attente)
                        await Task.Delay(10000);

                        // Localiser l'élément principal et obtenir son attribut outerHTML
                        var mainContentElement = driverOffer.FindElement(By.XPath("//*[@id=\"main-content\"]/section[1]/div"));
                        jobOffer.ContractType = mainContentElement.FindElement(By.XPath("div/section[1]/div/ul/li[1]/span")).Text;
                        jobOffer.HtmlContent = mainContentElement.FindElement(By.XPath("div")).GetAttribute("outerHTML");

                        linkedinJobOffers.Add(jobOffer);
                    }
                    catch (Exception ex)
                    {
                        // En cas d'erreur, ajoutez une offre d'emploi avec des valeurs par défaut
                        Console.WriteLine($"An error occurred while processing job offer: {ex.Message}");
                        linkedinJobOffers.Add(new JobOffer
                        {
                            WebSite = "Linkedin",
                            CompanieName = cardElement.FindElement(By.XPath("div/div[2]/h4/a")).Text,
                            JobTitle = cardElement.FindElement(By.XPath("div/a/span")).Text,
                            Location = "???",
                            Date = "???",
                            UrlOffer = cardElement.FindElement(By.XPath("div/a")).GetAttribute("href"),
                            ContractType = "???",
                            HtmlContent ="???",
                        Type = 1 //Type 1 = Jobboard
                        });
                    }
                }
            }
            finally
            {
                //driverOffer.Quit();
                driver.Quit();
            }

            return linkedinJobOffers;
        }

    }
}
