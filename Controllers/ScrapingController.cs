using Microsoft.AspNetCore.Mvc;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using Aspi_backend.Models;
using Aspi_backend.Services;
using System.Threading.Tasks;
using System.Threading;

namespace Aspi_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScrapingController : ControllerBase
    {
        private readonly IScrapingService _scrapingService;
        private readonly LinkedinScrapingService _linkedinScrapingService;

        public ScrapingController(IScrapingService scrapingService, LinkedinScrapingService linkedinScrapingService)
        {
            _scrapingService = scrapingService;
            _linkedinScrapingService = linkedinScrapingService;
        }

        [HttpGet]
        [Route("scrape-wttj")]
        public async Task<IActionResult> ScrapeHtmlContent()
        {
            try
            {
                var jobOffers = await _scrapingService.ScrapeJobOffers("WTTJ");
                var jobOffersWithHtmlContent = new List<JobOffer>();

                foreach (var jobOffer in jobOffers)
                {
                    var htmlContent = await _scrapingService.GetHtmlContentAsync(jobOffer.UrlOffer);
                    jobOffer.HtmlContent = htmlContent;
                    jobOffersWithHtmlContent.Add(jobOffer);
                }

                return Ok(jobOffersWithHtmlContent);
          
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while scraping job offers with HTML content: {ex.Message}");
                return new StatusCodeResult(500);
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
