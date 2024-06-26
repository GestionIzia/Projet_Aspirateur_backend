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
        private readonly IBPCEScrapingService _bpceScrapingService;
        private readonly IHWScrapingService _hwScrapingService;

        public ScrapingController(IWTTJScrapingService wttjScrapingService, ISGScrapingService sgScrapingService, LinkedinScrapingService linkedinScrapingService, IBPCEScrapingService bpceScrapingService, IHWScrapingService hwScrapingService)
        {
            _wttjScrapingService = wttjScrapingService;
            _sgScrapingService = sgScrapingService;
            _linkedinScrapingService = linkedinScrapingService;
            _bpceScrapingService = bpceScrapingService;
            _hwScrapingService = hwScrapingService;
        }

        [HttpGet]
        [Route("scrape-wttj")]
        public async Task<IActionResult> ScrapeWTTJ()
        {
            try
            {
                var jobOffers = await _wttjScrapingService.ScrapeWTTJOffers(); // Appel de la m�thode ScrapeWTTJOffers

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
                var jobOffers = await _sgScrapingService.ScrapeSGOffers(); // Appel de la m�thode ScrapeSGOffers

                foreach (var jobOffer in jobOffers)
                {
                    var htmlContent = await _sgScrapingService.GetHtmlContentAsync(jobOffer.UrlOffer);
                    jobOffer.HtmlContent = htmlContent;

                    // Utilisation de HtmlAgilityPack pour extraire la date
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);
                    // Recherche de l'�l�ment span avec un enfant strong
                    var dateNode = doc.DocumentNode.SelectSingleNode("//div[1]/div/div/div[4]/span/strong");

                    // Si l'�l�ment est trouv�, r�cup�rer le texte � l'int�rieur (la date)
                    if (dateNode != null)
                    {
                        jobOffer.Date = DateHelper.FormatDate(dateNode.InnerText);
                    }
                    else
                    {
                        // Si aucun �l�ment correspondant n'est trouv�, d�finir la date sur null ou une valeur par d�faut
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
        [HttpGet]
        [Route("scrape-bpce")]
        public async Task<IActionResult> ScrapeBPCE()
        {
            try
            {
                var jobOffers = await _bpceScrapingService.ScrapeBPCEOffers(); // Appel de la m�thode ScrapeSGOffers

                foreach (var jobOffer in jobOffers)
                {
                    var htmlContent = await _bpceScrapingService.GetHtmlContentAsync(jobOffer.UrlOffer);
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
        [Route("scrape-hw")]
        public async Task<IActionResult> ScrapeHW()
        {
            try
            {
                var jobOffers = await _hwScrapingService.ScrapeHWOffers(); // Appel de la m�thode ScrapeSGOffers

                foreach (var jobOffer in jobOffers)
                {

                    var htmlContent = await _hwScrapingService.GetHtmlContentAsync(jobOffer.UrlOffer);
                    jobOffer.HtmlContent = htmlContent;
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(htmlContent);

                    // Recherche de l'�l�ment span contenant la date
                    var dateNode = doc.DocumentNode.SelectSingleNode("//span[contains(@class, 'tw-text-grey') and contains(text(), 'Publi�e le')]");
                    if (dateNode != null)
                    {
                        string dateText = dateNode.InnerText.Trim();
                        string date = dateText.Split(new string[] { " - " }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("Publi�e le ", "");
                        jobOffer.Date = DateHelper.FormatDate(date);

                    }
                    else
                    {
                        jobOffer.Date = "???"; // Si aucune date n'est trouv�e, la date est d�finie sur null
                    }
                }

                return Ok(jobOffers);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while scraping WTTJ job offers with HTML content: {ex.Message}");
                return StatusCode(500);
            }
        }
    }
}
