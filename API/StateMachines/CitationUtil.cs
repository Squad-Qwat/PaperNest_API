using API.Models;
using System.Text.RegularExpressions;

namespace API.StateMachines
{
    public static class CitationFormatter
    {
        // Define a delegate for the formatting function
        private delegate string FormatCitationDelegate(Citation citation, string finalUrl, string retrievedFromUrlPart);

        // The "table" that maps CitationType to its formatting logic
        private static readonly Dictionary<CitationType, FormatCitationDelegate> _formatters;

        static CitationFormatter()
        {
            _formatters = new Dictionary<CitationType, FormatCitationDelegate>
            {
                { CitationType.Book, FormatBookCitation },
                { CitationType.JournalArticle, FormatJournalArticleCitation },
                { CitationType.Website, FormatWebsiteCitation },
                { CitationType.ConferencePaper, FormatConferencePaperCitation },
                { CitationType.Thesis, FormatThesisCitation }
            };
        }

        public static string GenerateAPAStyle(Citation citation)
        {
            string finalUrl = GetFinalUrl(citation);
            string retrievedFromUrlPart = string.IsNullOrWhiteSpace(finalUrl) ? "" : $"Diakses dari {finalUrl}";

            // Use the table to get the appropriate formatter
            if (_formatters.TryGetValue(citation.Type, out FormatCitationDelegate? formatter))
            {
                string apaString = formatter(citation, finalUrl, retrievedFromUrlPart);
                return CleanUpApaString(apaString);
            }
            else
            {
                return "Format sitasi tidak didukung.";
            }
        }

        private static string GetFinalUrl(Citation citation)
        {
            if (!string.IsNullOrWhiteSpace(citation.URL))
            {
                return citation.URL;
            }
            else if (!string.IsNullOrWhiteSpace(citation.AccessURL))
            {
                return citation.AccessURL;
            }
            return "";
        }

        private static string CleanUpApaString(string apaString)
        {
            apaString = Regex.Replace(apaString.Trim(), @"\s+", " ");
            apaString = Regex.Replace(apaString, @"\s*\.$", "."); // Ensure single trailing period
            return apaString;
        }

        // Individual formatting methods, made private as they are not intended for external use
        private static string FormatBookCitation(Citation citation, string finalUrl, string retrievedFromUrlPart)
        {
            string bookPublisherDetails = "";
            if (!string.IsNullOrWhiteSpace(citation.Publisher))
            {
                bookPublisherDetails = $"{citation.PublisherLocation}: {citation.Publisher}"; // Corrected publisher format
            }

            if (!string.IsNullOrWhiteSpace(finalUrl))
            {
                return $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {bookPublisherDetails} {retrievedFromUrlPart}.";
            }
            else
            {
                return $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {bookPublisherDetails}.";
            }
        }

        private static string FormatJournalArticleCitation(Citation citation, string finalUrl, string retrievedFromUrlPart)
        {
            string volumeIssueString = "";
            if (!string.IsNullOrWhiteSpace(citation.Volume))
            {
                volumeIssueString += citation.Volume;
                if (!string.IsNullOrWhiteSpace(citation.Issue))
                {
                    volumeIssueString += $"({citation.Issue})"; // Corrected issue format
                }
            }
            else if (!string.IsNullOrWhiteSpace(citation.Issue))
            {
                volumeIssueString += $"({citation.Issue})";
            }

            string pagesRangeString = string.IsNullOrWhiteSpace(citation.Pages) ? "" : $", {citation.Pages}";
            string journalDetails = "";

            if (!string.IsNullOrWhiteSpace(citation.PublicationInfo) || !string.IsNullOrWhiteSpace(volumeIssueString) || !string.IsNullOrWhiteSpace(pagesRangeString))
            {
                journalDetails = $"*{citation.PublicationInfo}*{volumeIssueString}{pagesRangeString}.";
            }
            string doiPart = string.IsNullOrWhiteSpace(citation.DOI) ? "" : $" {citation.DOI}";
            return $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {journalDetails}{doiPart}";
        }

        private static string FormatWebsiteCitation(Citation citation, string finalUrl, string retrievedFromUrlPart)
        {
            string websiteDetails = "";
            if (!string.IsNullOrWhiteSpace(citation.PublicationInfo))
            {
                websiteDetails = $"{citation.PublicationInfo}.";
            }
            if (!string.IsNullOrWhiteSpace(retrievedFromUrlPart))
            {
                websiteDetails += $" {retrievedFromUrlPart}.";
            }
            if (!string.IsNullOrWhiteSpace(citation.AccessDate))
            {
                websiteDetails += $" {citation.AccessDate}.";
            }
            return $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {websiteDetails}";
        }

        private static string FormatConferencePaperCitation(Citation citation, string finalUrl, string retrievedFromUrlPart)
        {
            string conferenceDetails = "";
            if (!string.IsNullOrWhiteSpace(citation.PublicationInfo))
            {
                conferenceDetails += $"In *{citation.PublicationInfo}*";
            }
            if (!string.IsNullOrWhiteSpace(citation.Pages))
            {
                conferenceDetails += $" (pp. {citation.Pages}).";
            }
            else if (!string.IsNullOrWhiteSpace(conferenceDetails))
            {
                conferenceDetails += ".";
            }

            if (!string.IsNullOrWhiteSpace(citation.Publisher))
            {
                conferenceDetails += $" {citation.Publisher}."; // Added period
            }
            string doiPart = string.IsNullOrWhiteSpace(citation.DOI) ? "" : $" {citation.DOI}";
            return $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {conferenceDetails}{doiPart}";
        }

        private static string FormatThesisCitation(Citation citation, string finalUrl, string retrievedFromUrlPart)
        {
            string thesisDetails = "";
            if (!string.IsNullOrWhiteSpace(citation.PublicationInfo))
            {
                thesisDetails = $"({citation.PublicationInfo}).";
            }
            if (!string.IsNullOrWhiteSpace(finalUrl))
            {
                thesisDetails += $" {retrievedFromUrlPart}.";
            }
            return $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {thesisDetails}";
        }
    }
}
