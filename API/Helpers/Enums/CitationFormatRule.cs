using API.Models;

namespace API.Helpers.Enums
{
    public class CitationFormatRule
    {
        public CitationType Type { get; set; }
        public Func<Citation, string> Formatter { get; set; }
    }
}