using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml;
using Aspi_backend.Models;
using Aspi_backend.Services;
using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium;
using HtmlAgilityPack;
using Aspi_backend.Services.Helpers;

namespace Aspi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapingController : ControllerBase
    {
        private readonly IWTTJScrapingService _wttjScrapingService; 
        private readonly ISGScrapingService _sgScrapingService; 
        private readonly LinkedinScrapingService _linkedinScrapingService;

        public ScrapingController(IWTTJScrapingService wttjScrapingService, ISGScrapingService sgScrapingService, LinkedinScrapingService linkedinScrapingService)
        {
            _wttjScrapingService = wttjScrapingService;
            _sgScrapingService = sgScrapingService;
            _linkedinScrapingService = linkedinScrapingService;
        }

        [HttpGet]
        [Route("scrape-wttj")]
        public async Task<IActionResult> ScrapeWTTJ()
        {
            try
            {
                var jobOffers = await _wttjScrapingService.ScrapeWTTJOffers(); // Appel de la méthode ScrapeWTTJOffers

                foreach (var jobOffer in jobOffers)
                {
                    var htmlContent = await _wttjScrapingService.GetHtmlContentAsync(jobOffer.UrlOffer);
                    jobOffer.HtmlContent = htmlContent;
                }

                return Ok(jobOffers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while scraping WTTJ job offers with HTML content: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("scrape-sg")]
        public async Task<IActionResult> ScrapeSG()
        {
            try
            {
                var jobOffers = await _sgScrapingService.ScrapeSGOffers(); // Appel de la méthode ScrapeSGOffers

                foreach (var jobOffer in jobOffers)
                {
                    var htmlContent = await _sgScrapingService.GetHtmlContentAsync(jobOffer.UrlOffer);
                    jobOffer.HtmlContent = htmlContent;

                    // Utilisation de HtmlAgilityPack pour extraire la date
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);
                    // Recherche de l'élément span avec un enfant strong
                    var dateNode = doc.DocumentNode.SelectSingleNode("//div[1]/div/div/div[4]/span/strong");

                    // Si l'élément est trouvé, récupérer le texte à l'intérieur (la date)
                    if (dateNode != null)
                    {
                        jobOffer.Date = DateHelper.FormatDate(dateNode.InnerText);
                    }
                    else
                    {
                        // Si aucun élément correspondant n'est trouvé, définir la date sur null ou une valeur par défaut
                        jobOffer.Date = null;
                    }
                }

                return Ok(jobOffers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while scraping SG job offers with HTML content: {ex.Message}");
                return StatusCode(500);
            }
        }

        [HttpGet]
        [Route("scrape-linkedin")]
        public async Task<IActionResult> ScrapeLinkedIn()
        {
            try
            {
                var jobOffers = await _linkedinScrapingService.ScrapeJobOffers();
                return Ok(jobOffers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while scraping LinkedIn job offers: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
