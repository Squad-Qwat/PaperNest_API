using API.Helpers.Enums;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UnitTesting
{
    [TestClass]
    [DoNotParallelize]
    public class ReviewServiceTests
    {
        private ReviewService _reviewService;

        [TestInitialize]
        public void Setup()
        {
            _reviewService = new ReviewService();

            ReviewRepository.Reviews = new List<Review>();
        }

        #region GetAllReviews
        [TestMethod]
        public void GetAllReviews_ReturnsAllReviews()
        {
            
            var review1 = new Review
            {
                FK_DocumentBodyId = Guid.NewGuid(),
                FK_UserLecturerId = Guid.NewGuid(),
                Comment = "Comment 1",
                Status = ReviewStatus.Approved
            };
            var review2 = new Review
            {
                FK_DocumentBodyId = Guid.NewGuid(),
                FK_UserLecturerId = Guid.NewGuid(),
                Comment = "Comment 2",
                Status = ReviewStatus.NeedsRevision
            };
            ReviewRepository.Reviews.Add(review1);
            ReviewRepository.Reviews.Add(review2);

            
            var result = _reviewService.GetAllReviews();

            
            Assert.AreEqual(2, result.Count);
            CollectionAssert.Contains(result, review1);
            CollectionAssert.Contains(result, review2);
        }

        [TestMethod]
        public void GetAllReviews_WhenEmpty_ReturnsEmptyList()
        {
            var result = _reviewService.GetAllReviews();
            Assert.AreEqual(0, result.Count);
        }
        #endregion

        #region GetReviewById
        [TestMethod]
        public void GetReviewById_WhenExists_ReturnsReview()
        {
            
            var review = new Review
            {
                FK_DocumentBodyId = Guid.NewGuid(),
                FK_UserLecturerId = Guid.NewGuid(),
                Comment = "Test Comment",
                Status = ReviewStatus.Approved
            };
            ReviewRepository.Reviews.Add(review);
            var reviewId = review.Id;

            
            var result = _reviewService.GetReviewById(reviewId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(reviewId, result.Id);
            Assert.AreEqual("Test Comment", result.Comment);
            Assert.AreEqual(ReviewStatus.Approved, result.Status);
        }

        [TestMethod]
        public void GetReviewById_WhenNotExists_ReturnsNull()
        {

            var nonExistentReviewId = Guid.NewGuid();
            var result = _reviewService.GetReviewById(nonExistentReviewId);
            
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetReviewById_WithEmptyGuid_ThrowsArgumentException()
        {
            
            _reviewService.GetReviewById(Guid.Empty);
        }
        #endregion

        #region AddReview
        [TestMethod]
        public void AddReview_AddsNewReview()
        {
            
            var documentBodyId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var comment = "New Review Comment";
            var status = ReviewStatus.NeedsRevision;

            
            _reviewService.AddReview(documentBodyId, userId, comment, status);

            
            Assert.AreEqual(1, ReviewRepository.Reviews.Count);
            var addedReview = ReviewRepository.Reviews.First();
            Assert.AreEqual(documentBodyId, addedReview.FK_DocumentBodyId);
            Assert.AreEqual(userId, addedReview.FK_UserLecturerId);
            Assert.AreEqual(comment, addedReview.Comment);
            Assert.AreEqual(status, addedReview.Status);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReview_WithEmptyDocumentBodyId_ThrowsArgumentException()
        {
            
            _reviewService.AddReview(Guid.Empty, Guid.NewGuid(), "Comment", ReviewStatus.Approved);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReview_WithEmptyUserId_ThrowsArgumentException()
        {
            
            _reviewService.AddReview(Guid.NewGuid(), Guid.Empty, "Comment", ReviewStatus.Approved);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReview_WithEmptyComment_ThrowsArgumentException()
        {
            
            _reviewService.AddReview(Guid.NewGuid(), Guid.NewGuid(), "", ReviewStatus.Approved);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReview_WithNullComment_ThrowsArgumentException()
        {
            
            _reviewService.AddReview(Guid.NewGuid(), Guid.NewGuid(), null, ReviewStatus.Approved);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void AddReview_WithWhitespaceComment_ThrowsArgumentException()
        {
            
            _reviewService.AddReview(Guid.NewGuid(), Guid.NewGuid(), "   ", ReviewStatus.Approved);
        }
        #endregion

        #region GetReviewByDocumentBodyId
        [TestMethod]
        public void GetReviewByDocumentBodyId_WhenExists_ReturnsReview()
        {
            
            var documentBodyId = Guid.NewGuid();
            var review = new Review
            {
                FK_DocumentBodyId = documentBodyId,
                FK_UserLecturerId = Guid.NewGuid(),
                Comment = "Document Body Review",
                Status = ReviewStatus.Approved
            };
            ReviewRepository.Reviews.Add(review);

            
            var result = _reviewService.GetReviewByDocumentBodyId(documentBodyId);

            
            Assert.IsNotNull(result);
            Assert.AreEqual(documentBodyId, result.FK_DocumentBodyId);
            Assert.AreEqual("Document Body Review", result.Comment);
            Assert.AreEqual(ReviewStatus.Approved, result.Status);
        }

        [TestMethod]
        public void GetReviewByDocumentBodyId_WhenNotExists_ReturnsNull()
        {
            
            var nonExistentDocumentBodyId = Guid.NewGuid();

            
            var result = _reviewService.GetReviewByDocumentBodyId(nonExistentDocumentBodyId);
            
            Assert.IsNull(result);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void GetReviewByDocumentBodyId_WithEmptyGuid_ThrowsArgumentException()
        {
            
            _reviewService.GetReviewByDocumentBodyId(Guid.Empty);
        }

        [TestMethod]
        public void RemoveReview_WhenExists_ReturnsTrue()
        {

            var review = new Review
            {
                FK_DocumentBodyId = Guid.NewGuid(),
                FK_UserLecturerId = Guid.NewGuid(),
                Comment = "Test Comment",
                Status = ReviewStatus.Approved
            };
            ReviewRepository.Reviews.Add(review);

            var result = _reviewService.RemoveReview(review.Id);

            Assert.IsTrue(result);
            Assert.AreEqual(0, ReviewRepository.Reviews.Count);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void RemoveReview_WithEmptyGuid_ThrowsArgumentException()
        {
            _reviewService.RemoveReview(Guid.Empty);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RemoveReview_WithNonExistentId_ThrowsInvalidOperationException()
        {
            var nonExistentReviewId = Guid.NewGuid();
            _reviewService.RemoveReview(nonExistentReviewId);
        }
        #endregion
    }
}
