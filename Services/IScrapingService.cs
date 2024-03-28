using System.Collections.Generic;
using System.Threading.Tasks;
using Aspi_backend.Models;

namespace Aspi_backend.Services
{
    public interface IScrapingService
    {
        Task<List<JobOffer>> ScrapeJobOffers(string jobBoard);
        Task<string> GetHtmlContentAsync(string url);
    }
}
