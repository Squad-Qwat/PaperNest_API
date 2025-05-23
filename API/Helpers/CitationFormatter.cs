using API.Models;

namespace API.Helpers
{
    public static class CitationFormatter
    {
        public static string GenerateAPAStyle(Citation citation)
        {
            string apaString = "";
            switch (citation.Type)
            {
                case Enums.CitationType.Book:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}.";
                    break;
                case Enums.CitationType.JournalArticle:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}.";
                    break;
                case Enums.CitationType.Website:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}. Diakses dari {citation.AccessDate}";
                    break;
                case Enums.CitationType.ConferencePaper:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. In {citation.PublicationInfo}.";
                    break;
                case Enums.CitationType.Thesis:
                    apaString = $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}.";
                    break;
                default:
                    apaString = "Format sitasi tidak didukung.";
                    break;
            }
            return apaString;
        }
    }
}