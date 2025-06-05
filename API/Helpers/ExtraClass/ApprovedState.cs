using API.Helpers.Enums;
using API.Models;

namespace API.Helpers.ExtraClass
{
    public class ApprovedState : ReviewState
    {
        public string Name => "Approved";
        public void Process(Review request, ReviewStatus result, string reviewerComment)
        {
            // Change the state to Under Review
            // ReviewUtil manager = new(); <- Setara dengan 'new  ReviewUtil()'
            Console.WriteLine($"Permintaan peninjauan {request} telah disetujui: {result}");
            Console.WriteLine($"Komentar reviewer: {reviewerComment}");
            // manager.ChangeState(request, new DoneState());
        }
    }
}