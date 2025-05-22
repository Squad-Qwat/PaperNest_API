using API.Models;

namespace API.StateMachines
{
    public static class CitationFormatter
    {
        public static string GenerateAPAStyle(Citation citation)
        {
            string apaString = "";
            switch (citation.Type)
            {
                case CitationType.Book:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. {citation.PublicationInfo}.";
                    break;
                case CitationType.JournalArticle:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. *{citation.PublicationInfo}*.";
                    break;
                case CitationType.Website:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. {citation.PublicationInfo}. Diakses dari {citation.AccessDate}";
                    break;
                case CitationType.ConferencePaper:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. In *{citation.PublicationInfo}*.";
                    break;
                case CitationType.Thesis:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). *{citation.Title}*. {citation.PublicationInfo}.";
                    break;
                default:
                    apaString = "Format sitasi tidak didukung.";
                    break;
            }
            return apaString;
        }
    }
}
