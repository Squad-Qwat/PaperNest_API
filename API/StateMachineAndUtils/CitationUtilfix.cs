using API.Helpers.Enums;
using API.Helpers.ExtraClass;
using API.Models;
using System;

namespace API.StateMachineAndUtils
{
    public class CitationUtilfix
    {
        private static readonly CitationFormatRule[] _formattingRules =
        {
            // Setara dengan 'new CitationFormatRule' dalam kode C#
            new() 
            {
                Type = CitationType.Book,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}."
            },
            new() 
            {
                Type = CitationType.JournalArticle,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}."
            },
            new() 
            {
                Type = CitationType.Website,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}. Diakses dari {citation.AccessDate}"
            },
            new() 
            {
                Type = CitationType.ConferencePaper,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. In {citation.PublicationInfo}."
            },
            new() 
            {
                Type = CitationType.Thesis,
                Formatter = citation => $"{citation.Author}. ({citation.PublicationDate?.Year}). {citation.Title}. {citation.PublicationInfo}."
            }
        };

        public static string GenerateAPAStyle(Citation citation)
        {
            foreach (var rule in _formattingRules)
            {
                if (rule.Type != citation.Type)
                {
                    continue;
                }
                return rule.Formatter(citation);
            }
            // Default case if no rule matches
            return "Format sitasi tidak didukung.";
        }
    }
}