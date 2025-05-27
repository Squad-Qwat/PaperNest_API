using API.Helpers.Enums;
using API.Models;

namespace API.Helpers
{
    public static class CitationFormatter
    {
        // Menggunakan aturan format sitasi dengan teknik stair-step table-driven
        private static readonly CitationFormatRule[] _formattingRules =
        {
            new CitationFormatRule
            {
                Type = CitationType.Book,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}."
            },
            new CitationFormatRule
            {
                Type = CitationType.JournalArticle,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}."
            },
            new CitationFormatRule
            {
                Type = CitationType.Website,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}. Diakses dari {citation.AccessDate}"
            },
            new CitationFormatRule
            {
                Type = CitationType.ConferencePaper,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. In {citation.PublicationInfo}."
            },
            new CitationFormatRule
            {
                Type = CitationType.Thesis,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}."
            }
        };

        public static string GenerateAPAStyle(Citation citation)
        {
            foreach (var rule in _formattingRules)
            {
                if (rule.Type == citation.Type)
                {
                    return rule.Formatter(citation);
                }
            }
            // Default case if no rule matches
            return "Format sitasi tidak didukung.";
        }
    }
}