// Emplacement : Aspi_backend/Models/JobOffer.cs
namespace Aspi_backend.Models
{
    public class JobOffer
    {
        public string? WebSite { get; set; }
        public string? CompanieName { get; set; }
        public string? JobTitle { get; set; }
        public string? ContractType { get; set; }
        public string? Location { get; set; }
        public string? Date { get; set; }
        public string? UrlOffer { get; set; }
        public string? HtmlContent { get; set; }

        public int? Type { get; set; } // (Jobboard = 1, CareerCenter = 2)
    }
}
