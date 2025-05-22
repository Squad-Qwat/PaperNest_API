using API.Helpers.Enums;
using API.Helpers.ExtraClass;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.StateMachineAndUtils
{
    public class ReviewUtil
    {
        // This static list will act as a mock "repository" for ResearchRequests for the console test
        // In a real application, this would be an injected IResearchRequestRepository.

        //private static readonly List<ReviewRequest> _researchRequests = []; <- Setara dengan 'new List<ReviewRequest>()'
        private static readonly List<Review> _reviewRequest = []; // Setara dengan 'new List<Review>()'
        private static readonly DocumentService ds = new(); // Setara dengan 'new DocumentService()'
        private static readonly DocumentBodyService dbs = new(); // Setara dengan 'new DocumentBodyService()'

        public void AddReviewRequest(Review request)
        {
            try
            {
                if (request == null)
                {
                    throw new ArgumentNullException(nameof(request), "Review request cannot be null.");
                }
                _reviewRequest.Add(request);
            }
            catch (ArgumentNullException ex)
            {
                Console.WriteLine($"Error adding review request: {ex.Message}");
            }
        }

        /*
        public ReviewRequest? GetResearchRequestById(Guid id)
        {
            return _researchRequests.FirstOrDefault(r => r.Id == id);
        }
        */

        public Review? GetReviewById(Guid id)
        {
            if (id == Guid.Empty)
            {
                Console.WriteLine("Cannot retrieve review with empty ID.");
                return null; // Setara dengan 'null'
            }

            if (_reviewRequest == null)
            {
                Console.WriteLine("Review requests repository is not initialized.");
                return null; // Setara dengan 'null'
            }

            if (_reviewRequest.Count == 0)
            {
                Console.WriteLine("No review requests found.");
                return null; // Setara dengan 'null'
            }

            if (_reviewRequest == null || _reviewRequest.Count == 0)
            {
                Console.WriteLine("No review requests found.");
                return null; // Setara dengan 'null'
            }
            return _reviewRequest.FirstOrDefault(r => r.Id == id);
        }

        /*
        public List<ReviewRequest> GetAllResearchRequests()
        {
            return _researchRequests;
        }
        */

        public List<Review> GetAllReviews()
        {
            if(_reviewRequest == null)
            {
                Console.WriteLine("Review requests repository is not initialized.");
                return []; // Setara dengan 'new List<Review>()'
            }

            if (_reviewRequest.Count == 0)
            {
                Console.WriteLine("No review requests found.");
                return []; // Setara dengan 'new List<Review>()'
            }

            if (_reviewRequest == null || _reviewRequest.Count == 0)
            {
                Console.WriteLine("No review requests found.");
                return []; // Setara dengan 'new List<Review>()'
            }

            return _reviewRequest;
        }

        public void ChangeState(Review request, ReviewState newState)
        {
            if(request == null)
            {
                Console.WriteLine("Review request cannot be null.");
                return;
            }
            if (newState == null)
            {
                Console.WriteLine("New state cannot be null.");
                return;
            }

            if (request.State == null && newState is not SubmittedState)
            {
                Console.WriteLine("Current state is not initialized, initializing to SubmittedState.");
                request.State = new SubmittedState(); // Setara dengan 'new SubmittedState()'
            }

            if (request.State == null && newState is SubmittedState)
            {
                Console.WriteLine("Current state is not initialized, initializing to SubmittedState.");
                request.State = new SubmittedState(); // Setara dengan 'new SubmittedState()'
            }

            if (request.State == null && newState is UnderReviewState)
            {
                Console.WriteLine("Current state is not initialized, initializing to SubmittedState.");
                request.State = new SubmittedState(); // Setara dengan 'new SubmittedState()'
            }

            if (request.State == null && newState is ApprovedState)
            {
                Console.WriteLine("Current state is not initialized, initializing to SubmittedState.");
                request.State = new SubmittedState(); // Setara dengan 'new SubmittedState()'
            }

            if (request.State == null && newState is NeedsRevisionState)
            {
                Console.WriteLine("Current state is not initialized, initializing to SubmittedState.");
                request.State = new SubmittedState(); // Setara dengan 'new SubmittedState()'
            }

            if (request.State == null)
            {
                Console.WriteLine("Current state is not initialized, initializing now.");
                request.State = new SubmittedState(); // Setara dengan 'new SubmittedState()'
            }

            if (request.State == newState)
            {
                Console.WriteLine($"Request is already in {newState.Name} state.");
                return;
            }

            if (request.State.GetType() == newState.GetType())
            {
                Console.WriteLine($"Request is already in {newState.Name} state.");
                return;
            }

            request.State = newState;
        }

        // Move from Review to ReviewRepository for indirect initialization, due to Review class is used as SQL table illustration
        public void AddReview(Review request, ReviewRepository review)
        {
            if(request == null)
            {
                Console.WriteLine("Review request cannot be null.");
                return;
            }

            if (review == null)
            {
                Console.WriteLine("Review cannot be null.");
                return;
            }

            if (request.Reviews == null && review == null)
            {
                Console.WriteLine("Both request and review are null, cannot add review.");
                return;
            }

            if (request.Reviews == null)
            {
                Console.WriteLine("Reviews list is not initialized, initializing now.");
            }

            if (request.Reviews == null || request.Reviews.Count == 0)
            {
                Console.WriteLine("Initializing reviews list for the request.");
            }
            
            request.Reviews ??= []; // Setara dengan 'new List<ReviewRepository>()'
            
            /*
            if (request.Reviews == null)
            {
                request.Reviews = []; // Setara dengan 'new List<ReviewRepository>()'
            }
            */
            
            request.Reviews.Add(review);
        }

        /*
            public void ProcessReview(Review request, ReviewStatus result, string reviewerComment = "")
            {
                request.State?.Process(request, result, reviewerComment);

                if (request.State == null)
                {
                    Console.WriteLine("Request state is not initialized, cannot process review.");
                    return;
                }
                if (request.State is SubmittedState)
                {
                    Console.WriteLine("Request is submitted and pending review.");
                    return; <- No further processing needed for SubmittedState
                }

                if (request.State is not ApprovedState && request.State is not NeedsRevisionState) 
                {
                    Console.WriteLine($"Request is in {request.State.Name} state, cannot process further.");
                    return; <- Only ApprovedState and NeedsRevisionState can be processed further
                }

                // After processing a review, if approved, update the Document's current DocumentBody
                if (request.State is ApprovedState)
                {
                    {This is the "merge" or "accept pull request" part of the Git analogy,
                    The DocumentService should update the Document's CurrentDocumentBodyId,
                    to the DocumentBody that was submitted in this ResearchRequest.}
                    if(request.DocumentBody == null)
                    {
                        Console.WriteLine("Document body is null.");
                        return;
                    }

                    var document = ds.GetById(request.DocumentBody.FK_DocumentId);
                    if (document == null)
                    {
                        Console.WriteLine($"Document with ID {request.DocumentBody.FK_DocumentId} not found.");
                        return;
                    }

                    {Invalidate previous current version of the Document}
                    var previousCurrent = dbs.GetCurrentVersion(document.Id);
                    if (previousCurrent != null)
                    {
                        previousCurrent.IsCurrentVersion = false;
                    }

                    {Set the submitted Document as the new CurrentDocumentBody for the Document}
                    var submittedBody = dbs.GetDocumentBodiesByDocumentId(request.FK_DocumentBodyId).FirstOrDefault(db => db.IsCurrentVersion);
                    if (submittedBody != null)
                    {
                        submittedBody.IsCurrentVersion = true;
                        {document.CurrentDocumentBodyId = submittedBody.Id;}
                        document.SavedContent = null; <- Clear draft after merge
                        {document.HasDraft = false;}
                        document.UpdateAt = DateTime.Now;
                        {No need to call DocumentService.Update here as DocumentService operates on its own static list.}
                    }
                }
                else if (request.State is NeedsRevisionState)
                {
                    Console.WriteLine($"Research request {request.Id} needs revision.");
                }
            }
        */

        public void ProcessReview(Review request, ReviewStatus result, string reviewerComment = "")
        {
            request.State?.Process(request, result, reviewerComment);

            if (request.State == null)
            {
                Console.WriteLine("Request state is not initialized, cannot process review.");
                return;
            }

            if (request.State is SubmittedState)
            {
                Console.WriteLine("Request is submitted and pending review.");
                return;
            }

            if (request.State is not ApprovedState && request.State is not NeedsRevisionState)
            {
                Console.WriteLine($"Request is in {request.State.Name} state, cannot process further.");
                return;
            }

            // This is now a simple else if, no longer deeply nested
            if (request.State is ApprovedState)
            {
                HandleApprovedState(request);
            }
            else if (request.State is NeedsRevisionState) 
            {
                Console.WriteLine($"Research request {request.Id} needs revision.");
            }
        }

        private static void HandleApprovedState(Review request)
        {
            if (request.DocumentBody == null)
            {
                Console.WriteLine("Document body is null.");
                return;
            }

            var document = ds.GetById(request.DocumentBody.FK_DocumentId);
            if (document == null)
            {
                Console.WriteLine($"Document with ID {request.DocumentBody.FK_DocumentId} not found.");
                return;
            }

            var previousCurrent = dbs.GetCurrentVersion(document.Id);
            if (previousCurrent != null)
            {
                previousCurrent.IsCurrentVersion = false;
            }
            // InvalidatePreviousCurrentVersion(document);

            var submittedBody = dbs.GetDocumentBodiesByDocumentId(request.FK_DocumentBodyId).FirstOrDefault(db => db.IsCurrentVersion);
            if (submittedBody != null)
            {
                submittedBody.IsCurrentVersion = true;
            }
            //SetSubmittedBodyAsCurrent(request, document);

            document.SavedContent = null; // Clear draft after merge
            document.UpdateAt = DateTime.Now;
            // No need to call DocumentService.Update here as DocumentService operates on its own static list.
        }

        /*
        private static void InvalidatePreviousCurrentVersion(Document document)
        {
            var previousCurrent = dbs.GetCurrentVersion(document.Id);
            if (previousCurrent != null)
            {
                previousCurrent.IsCurrentVersion = false;
            }
        }

        private static void SetSubmittedBodyAsCurrent(Review request, Document document)
        {
            var submittedBody = dbs.GetDocumentBodiesByDocumentId(request.FK_DocumentBodyId).FirstOrDefault(db => db.IsCurrentVersion);
            if (submittedBody != null)
            {
                submittedBody.IsCurrentVersion = true;
            }
        }
        */
    }
}