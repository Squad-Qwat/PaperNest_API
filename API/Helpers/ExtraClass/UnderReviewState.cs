using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using API.StateMachineAndUtils;

namespace API.Helpers.ExtraClass
{
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
}