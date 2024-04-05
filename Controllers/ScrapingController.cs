using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Aspi_backend.Models;
using Aspi_backend.Services;
using Microsoft.AspNetCore.Mvc;

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
