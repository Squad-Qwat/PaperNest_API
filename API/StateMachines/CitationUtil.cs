using API.Models;
using System.Text.RegularExpressions;

namespace API.StateMachines
{
    public static class CitationFormatter
    {
        public static string GenerateAPAStyle(Citation citation)
        {
            string apaString = "";

            // Determine the URL to use for online resources
            string finalUrl = "";
            if (!string.IsNullOrWhiteSpace(citation.URL))
            {
                finalUrl = citation.URL;
            }
            else if (!string.IsNullOrWhiteSpace(citation.AccessURL))
            {
                finalUrl = citation.AccessURL;
            }
            string retrievedFromUrlPart = string.IsNullOrWhiteSpace(finalUrl) ? "" : $"Diakses dari {finalUrl}";


            switch (citation.Type)
            {
                case CitationType.Book:
                    // Format: Author. (Year). Title. Publisher Location: Publisher.
                    // If online: Author. (Year). Title. Publisher. URL.
                    string bookPublisherDetails = "";
                    if (!string.IsNullOrWhiteSpace(citation.Publisher))
                    {
                        bookPublisherDetails = $"{citation.PublisherLocation}{citation.Publisher}";
                    }

                    if (!string.IsNullOrWhiteSpace(finalUrl))
                    {
                        apaString = $"{citation.Author} ({citation.PublicationDate?.Year}). {citation.Title} {bookPublisherDetails} {retrievedFromUrlPart}.";
                    }
                    else
                    {
                        apaString = $"{citation.Author} ({citation.PublicationDate?.Year}). {citation.Title} {bookPublisherDetails}.";
                    }
                    break;

                case CitationType.JournalArticle:
                    // Format: Author, A. A. (Year). Title of article. *Title of Periodical, Volume*(Issue), pages. DOI
                    string volumeIssueString = "";
                    if (!string.IsNullOrWhiteSpace(citation.Volume))
                    {
                        volumeIssueString += citation.Volume;
                        if (!string.IsNullOrWhiteSpace(citation.Issue))
                        {
                            volumeIssueString += citation.Issue;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(citation.Issue)) // Only issue provided
                    {
                        volumeIssueString += citation.Issue;
                    }

                    string pagesRangeString = string.IsNullOrWhiteSpace(citation.Pages) ? "" : $", {citation.Pages}";
                    string journalDetails = "";

                    if (!string.IsNullOrWhiteSpace(citation.PublicationInfo) || !string.IsNullOrWhiteSpace(volumeIssueString) || !string.IsNullOrWhiteSpace(pagesRangeString))
                    {
                        journalDetails = $"*{citation.PublicationInfo}*{volumeIssueString}{pagesRangeString}.";
                    }
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {journalDetails}. {citation.DOI}";
                    break;

                case CitationType.Website:
                    // Format: Author, A. A. (Year, Month Day). Title. Site Name. Retrieved from URL. Access Date.
                    // Assuming PublicationInfo is "Site Name"
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
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {websiteDetails}";
                    break;

                case CitationType.ConferencePaper:
                    // Format: Author, A. A. (Year, Month Day). Title. In Editor, E. E. (Ed.), Title of symposium or conference (pp. pages). Publisher. DOI.
                    // Assuming PublicationInfo is "Title of symposium or conference"
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
                        conferenceDetails += "."; // Close italics if no pages
                    }

                    if (!string.IsNullOrWhiteSpace(citation.Publisher))
                    {
                        conferenceDetails += $" {citation.Publisher}";
                    }
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {conferenceDetails}. {citation.DOI}";
                    break;

                case CitationType.Thesis:
                    // Format: Author, A. A. (Year). Title (Doctoral dissertation or master's thesis, Name of Institution). URL.
                    // Assuming PublicationInfo is "Name of Institution"
                    string thesisDetails = "";
                    if (!string.IsNullOrWhiteSpace(citation.PublicationInfo))
                    {
                        thesisDetails = $"({citation.PublicationInfo}).";
                    }
                    if (!string.IsNullOrWhiteSpace(finalUrl))
                    {
                        thesisDetails += $" {retrievedFromUrlPart}.";
                    }
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {thesisDetails}";
                    break;

                default:
                    apaString = "Format sitasi tidak didukung.";
                    break;
            }

            // Clean up multiple spaces and trim, then remove any trailing periods that might result from empty parts
            apaString = Regex.Replace(apaString.Trim(), @"\s+", " ");
            apaString = Regex.Replace(apaString, @"\s*\.$", "."); // Ensure single trailing period
            return apaString;
        }
    }
}
