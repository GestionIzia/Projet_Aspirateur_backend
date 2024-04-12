using System;

namespace Aspi_backend.Services.Helpers
{
    public class DateHelper
    {
        public static string FormatDate(string date)
        {
            DateTime parsedDate;
            if (DateTime.TryParse(date, out parsedDate))
            {
                TimeSpan timeSince = DateTime.Now - parsedDate.Date;
                if (timeSince.Days == 0)
                {
                    return "aujourd'hui";
                }
                else if (timeSince.Days == 1)
                {
                    return "hier";
                }
                else if (timeSince.Days == 2)
                {
                    return "avant-hier";
                }
                else if (timeSince.Days < 7)
                {
                    return $"Il y a {timeSince.Days} jours";
                }
                else
                {
                    return date; // Au-delà de 7 jours, retourner simplement la date d'origine
                }
            }
            else
            {
                return date; // Si la date ne peut pas être analysée, retourner simplement la date d'origine
            }
        }
    }
    }
