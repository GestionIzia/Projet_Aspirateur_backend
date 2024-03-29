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

        public ScrapingController(IScrapingService scrapingService)
        {
            _scrapingService = scrapingService;
        }

        [HttpGet]
        [Route("scrape-html-content")]
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

    }
}
