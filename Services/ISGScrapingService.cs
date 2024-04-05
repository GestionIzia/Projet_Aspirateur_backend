using System.Collections.Generic;
using System.Threading.Tasks;
using Aspi_backend.Models;

namespace Aspi_backend.Services
{
    public interface ISGScrapingService : IScrapingService
    {
        Task<List<JobOffer>> ScrapeSGOffers();
    }
}
