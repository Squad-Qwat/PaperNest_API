using API.Helpers.Enums;
using API.Models;
using API.StateMachineAndUtils;

namespace API.Helpers.ExtraClass
{
    public class NeedsRevisionState : ReviewState
    {
        public string Name => "Needs Revision";
        public void Process(Review request, ReviewStatus result, string reviewerComment)
        {
            ReviewUtil manager = new(); // Setara dengan 'new  ReviewUtil()'
            if (result != ReviewStatus.Approved)
            {
                Console.WriteLine($"Research request {request} is still under revision or received another review result: {result}");
                return;

            }

            manager.ChangeState(request, new ApprovedState());
        }
    }
}