using API.Models;

namespace API.Helpers.Enums
{
    public class CitationFormatRule
    {
        public CitationType Type { get; set; }
        public required Func<Citation, string> Formatter { get; set; }
    }
}