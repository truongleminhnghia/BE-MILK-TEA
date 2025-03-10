using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services.IngredientReviewService
{
    public class IngredientReviewService : IIngredientReviewService
    {
        private readonly IIngredientReviewRepository _ingredientReviewRepository;
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IAccountRepository _accountRepository;

        public IngredientReviewService(
            IIngredientReviewRepository ingredientReviewRepository,
            IIngredientRepository ingredientRepository,
            IAccountRepository accountRepository
        )
        {
            _ingredientReviewRepository = ingredientReviewRepository;
            _ingredientRepository = ingredientRepository;
            _accountRepository = accountRepository;
        }

        public async Task<IEnumerable<IngredientReviewResponse>> GetAllAsync()
        {
            var reviews = await _ingredientReviewRepository.GetAllAsync();
            return reviews.Select(MapToResponse);
        }

        public async Task<IEnumerable<IngredientReviewResponse>> GetByIngredientIdAsync(
            Guid ingredientId
        )
        {
            var reviews = await _ingredientReviewRepository.GetByIngredientIdAsync(ingredientId);
            return reviews.Select(MapToResponse);
        }

        public async Task<IEnumerable<IngredientReviewResponse>> GetByAccountIdAsync(Guid accountId)
        {
            var reviews = await _ingredientReviewRepository.GetByAccountIdAsync(accountId);
            return reviews.Select(MapToResponse);
        }

        public async Task<IngredientReviewResponse> GetByIdAsync(Guid id)
        {
            var review = await _ingredientReviewRepository.GetByIdAsync(id);
            if (review == null)
                return null;

            return MapToResponse(review);
        }

        public async Task<IngredientReviewResponse> CreateAsync(
            CreateIngredientReviewRequest request
        )
        {
            // Xác thực thành phần và tài khoản tồn tại
            var ingredient = await _ingredientRepository.GetByIdAsync(request.IngredientId);
            if (ingredient == null)
                throw new ArgumentException("Không tìm thấy thành phần");

            var account = await _accountRepository.GetById(request.AccountId);
            if (account == null)
                throw new ArgumentException("Không tìm thấy tài khoản");

            // Kiểm tra xem tài khoản này đã xem xét thành phần này chưa
            bool reviewExists =
                await _ingredientReviewRepository.ExistsReviewByAccountAndIngredientAsync(
                    request.AccountId,
                    request.IngredientId
                );
            if (reviewExists)
                throw new InvalidOperationException("Tài khoản này đã xem xét thành phần này");

            // Xác thực tỷ lệ (từ 0 đến 5)
            if (request.Rate < 0 || request.Rate > 5)
                throw new ArgumentException("Tỷ lệ phải nằm trong khoảng từ 0 đến 5");

            var review = new IngredientReview
            {
                IngredientId = request.IngredientId,
                AccountId = request.AccountId,
                Comment = request.Comment,
                Rate = request.Rate,
            };

            var createdReview = await _ingredientReviewRepository.CreateAsync(review);
            return MapToResponse(createdReview);
        }

        public async Task<IngredientReviewResponse> UpdateAsync(
            Guid id,
            UpdateIngredientReviewRequest request
        )
        {
            var existingReview = await _ingredientReviewRepository.GetByIdAsync(id);
            if (existingReview == null)
                return null;

            // Xác thực tỷ lệ (từ 0 đến 5)
            if (request.Rate < 0 || request.Rate > 5)
                throw new ArgumentException("Tỷ lệ phải nằm trong khoảng từ 0 đến 5");

            existingReview.Comment = request.Comment;
            existingReview.Rate = request.Rate;

            var updatedReview = await _ingredientReviewRepository.UpdateAsync(existingReview);
            return MapToResponse(updatedReview);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            return await _ingredientReviewRepository.DeleteAsync(id);
        }

        private IngredientReviewResponse MapToResponse(IngredientReview review)
        {
            return new IngredientReviewResponse
            {
                Id = review.Id,
                IngredientId = review.IngredientId,
                AccountId = review.AccountId,
                Comment = review.Comment,
                Rate = review.Rate,
                IngredientName = review.Ingredient?.IngredientName,
                AccountName = $"{review.Account?.FirstName} {review.Account?.LastName}",
            };
        }
    }
}
