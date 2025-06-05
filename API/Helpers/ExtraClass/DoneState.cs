using API.Helpers.Enums;
using API.Models;

namespace API.Helpers.ExtraClass
{
    public class DoneState : ReviewState
    {
        public string Name => "Done";
        public void Process(Review request, ReviewStatus result, string reviewerComment)
        {
            Console.WriteLine("Dokumen sudah ditinjau.");
            Console.WriteLine($"Permintaan peninjauan {request} sudah selesai: {result}.");
            Console.WriteLine($"Komentar reviewer: {reviewerComment}");
        }
    }
}