// Program.cs
using Aspi_backend.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace Aspi_backend
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Appel de la méthode de scraping
            ScrapeModal scraper = new ScrapeModal();
            await scraper.ScrapePageAsync();

            // Création et exécution de l'hôte web
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
