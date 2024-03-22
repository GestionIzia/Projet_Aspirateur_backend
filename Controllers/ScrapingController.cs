using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Aspi_backend.Models;
using Aspi_backend.Services;
using OpenQA.Selenium.DevTools.V120.HeadlessExperimental;


namespace Aspi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapingController : ControllerBase
    {
        [HttpGet]
        [Route("scrape-html-content")]
    public async Task<IActionResult> ScrapeHtmlContent()
    {
        
            var scraper = new ScrapeModal();
        string htmlContent = await scraper.GetHtmlContentAsync();
        if (htmlContent != null)
        {
            return new ContentResult { Content = htmlContent, ContentType = "text/html" };
        }
        else
        {
            return new StatusCodeResult(500);
        }
    }
        [HttpGet]
        [Route("scrape-job-offers")]
        public IActionResult ScrapeJobOffers()
        {
            var edgeDriverPath = "C:\\Users\\Bradley GOEH AKUE\\Desktop\\ME\\msedgedriver.exe";
            var edgeOptions = new EdgeOptions();
            edgeOptions.AddArgument("headless"); // Active le mode headless
            var driver = new EdgeDriver(edgeDriverPath, edgeOptions);

            driver.Navigate().GoToUrl("https://www.welcometothejungle.com/fr/jobs?query=apprentissage");
            Thread.Sleep(5000);

            var cardElements = driver.FindElements(By.XPath("//*[@id='pages_jobs']/div[2]/div/ul/li[position() <= 12]/div/div"));
            //*[@id='pages_jobs']/div[2]/div/ol/div[position() <= 12]/li/div/div

            //*[@id="pages_jobs"]/div[2]/div/ul/li[1]/div/div/div/div[2]/a/h4/div/text()
            //*[@id="pages_jobs"]/div[2]/div/ul/li[1]/div/div/div/div[2]/a/h4/div
            var cardinfos = new List<CardInfos>();

            if (cardElements != null)
            {
                Console.WriteLine("Offres en apprentissage sur Welcome to the Jungle :");
                foreach (var cardElement in cardElements)
                {
                    cardinfos.Add(new CardInfos()
                    {
                        //*[@id="pages_jobs"]/div[2]/div/ul/li[1]/div/div/div/div[1]/span
                        CompanieName = cardElement.FindElement(By.XPath("./div/div[1]/span")).Text,
                        JobTitle = cardElement.FindElement(By.XPath("./div/div[2]/a/h4/div")).Text,

                        ContractType = cardElement.FindElement(By.XPath("./div/div[2]/div[2]/div/span")).Text,
                        Location = cardElement.FindElement(By.XPath("./div/div[2]/div[1]/p/span/span")).Text,
                        Date = cardElement.FindElement(By.XPath("./div/div[2]/div[3]/div[1]/p/time/span")).Text,
                        //*[@id="pages_jobs"]/div[2]/div/ul/li[1]/div/div/a
                        UrlOffer = cardElement.FindElement(By.XPath("./a")).GetAttribute("href") 
                    });
                }
            }
            else
            {
                Console.WriteLine("Aucune offre en apprentissage trouvée sur Welcome to the Jungle.");
                driver.Quit();

            }

        
            return Ok(cardinfos); // Retourne les données scrappées en tant que réponse JSON
        }
    }
}