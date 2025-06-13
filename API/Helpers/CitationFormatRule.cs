using API.Helpers.Enums;
using API.Models;

namespace API.Helpers
{
    public class CitationFormatRule
    {
        public CitationType Type { get; set; }
        public required Func<Citation, string> Formatter { get; set; }
    }
}