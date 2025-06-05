using API.Helpers.Enums;
using API.Models;
using API.StateMachineAndUtils;

namespace API.Helpers.ExtraClass
{
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

            Console.WriteLine("Research request is submitted and pending review.");
            manager.ChangeState(request, new UnderReviewState());
        }
    }
}