using API.Models;
using API.Repositories;
using API.Services;
using API.StateMachines;
using Microsoft.AspNetCore.Mvc;

namespace API.Helpers.Enums
{
    public interface ReviewState
    {
        string Name { get; }
        void Process(Review request, ReviewStatus result, string reviewerComment);
    }

    public class SubmittedState : ReviewState
    {
        public string Name => "Submitted";
        
        public void Process(Review request, ReviewStatus result, string reviewerComment)
        {
            // Change the state to Under Review
            ReviewUtil manager = new(); // Setara dengan 'new  ReviewUtil()'
            if (result != ReviewStatus.Pending)
            {
                Console.WriteLine($"Error: Cannot directly set to {result} from {Name}. Requires review first.");
                return;
            }
            else
            {
                Console.WriteLine("Research request is submitted and pending review.");
                manager.ChangeState(request, new UnderReviewState());
            }
        }
    }
    public class UnderReviewState : ReviewState
    {
        public string Name => "Under Review";

        public void Process(Review request, ReviewStatus result, string reviewerComment)
        {
            ReviewUtil manager = new(); // Setara dengan 'new  ReviewUtil()'
            manager.AddReview(request, new ReviewRepository(Guid.NewGuid(), result, request.FK_DocumentBodyId, request.FK_UserId, reviewerComment));

            switch (result)
            {
                case ReviewStatus.Approved:
                    manager.ChangeState(request, new ApprovedState());
                    break;
                case ReviewStatus.NeedsRevision:
                    manager.ChangeState(request, new NeedsRevisionState());
                    break;
                default:
                    Console.WriteLine("Review result is still pending.");
                    break;
            }
        }
    }
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

    public class NeedsRevisionState : ReviewState
    {
        public string Name => "Needs Revision";
        public void Process(Review request, ReviewStatus result, string reviewerComment)
        {
            ReviewUtil manager = new(); // Setara dengan 'new  ReviewUtil()'
            if (result == ReviewStatus.Approved)
            {
                manager.ChangeState(request, new ApprovedState());
            }
            else
            {
                Console.WriteLine($"Research request {request} is still under revision or received another review result: {result}");
            }
        }
    }
}
