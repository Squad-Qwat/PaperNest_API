using API.Helpers.Enums;
using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.StateMachines
{
    public class ReviewUtil
    {
        // This static list will act as a mock "repository" for ResearchRequests for the console test
        // In a real application, this would be an injected IResearchRequestRepository.
        private static readonly List<ReviewRequest> _researchRequests = []; // Setara dengan 'new List<ResearchRequest>()'
        private static readonly List<Review> _reviewRequest = []; // Setara dengan 'new List<Review>()'
        private static readonly DocumentBodyService dbs = new(); // Setara dengan 'new DocumentBodyService()'

        public void AddResearchRequest(ReviewRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request), "Research request cannot be null.");
            }
            _researchRequests.Add(request);
        }

        public ReviewRequest? GetResearchRequestById(Guid id)
        {
            return _researchRequests.FirstOrDefault(r => r.Id == id);
        }

        public Review? GetReviewById(Guid id)
        {
            return _reviewRequest.FirstOrDefault(r => r.Id == id);
        }

        public List<ReviewRequest> GetAllResearchRequests()
        {
            return _researchRequests;
        }

        public List<Review> GetAllReviews()
        {
            return _reviewRequest;
        }

        public void ChangeState(ReviewRequest request, ReviewState newState)
        {
            request.State = newState;
        }

        public void AddReview(ReviewRequest request, Review review)
        {
            request.Reviews.Add(review);
        }

        public void ProcessReview(ReviewRequest request, ReviewStatus result, string reviewerComment = "")
        {
            request.State.Process(request, result, reviewerComment);

            // After processing a review, if approved, update the Document's current DocumentBody
            if (request.State is ApprovedState)
            {
                // This is the "merge" or "accept pull request" part of the Git analogy
                // The DocumentService should update the Document's CurrentDocumentBodyId
                // to the DocumentBody that was submitted in this ResearchRequest.
                var document = DocumentService.GetById(request.DocumentId);
                if (document == null)
                {
                    Console.WriteLine($"Document with ID {request.DocumentId} not found.");
                    return;
                }

                // Invalidate previous current version of the Document
                var previousCurrent = dbs.GetCurrentVersion(document.Id);
                if (previousCurrent != null)
                {
                    previousCurrent.IsCurrentVersion = false;
                }

                // Set the submitted Document as the new CurrentDocumentBody for the Document
                var submittedBody = dbs.GetDocumentBodiesByDocumentId(request.DocumentBodyId).FirstOrDefault(db => db.IsCurrentVersion);
                if (submittedBody != null)
                {
                    submittedBody.IsCurrentVersion = true;
                    // document.CurrentDocumentBodyId = submittedBody.Id;
                    document.SavedContent = null; // Clear draft after merge
                    // document.HasDraft = false;
                    document.UpdateAt = DateTime.Now;
                    // No need to call DocumentService.Update here as DocumentService operates on its own static list.
                }
            }
            else if (request.State is NeedsRevisionState)
            {
                Console.WriteLine($"Research request {request.Id} needs revision.");
            }
        }
    }
}