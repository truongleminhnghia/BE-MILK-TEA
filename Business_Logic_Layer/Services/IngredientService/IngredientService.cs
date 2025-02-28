using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Business_Logic_Layer.Models;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Business_Logic_Layer.Utils;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;

namespace Business_Logic_Layer.Services.IngredientService
{
    public class IngredientService : IIngredientService
    {
        private readonly IIngredientRepository _ingredientRepository;
        private readonly IMapper _mapper;
        private readonly ICategoryService _categoryService;
        private readonly Source _source;
        private readonly IImageService _imageSerivce;

        public IngredientService(IIngredientRepository ingredientRepository, IMapper mapper, ICategoryService categoryService, Source source, IImageService imageService)
        {
            _ingredientRepository = ingredientRepository;
            _mapper = mapper;
            _categoryService = categoryService;
            _source = source;
            _imageSerivce = imageService;
        }

        public async Task<IngredientResponse> CreateIngredientAsync(IngredientRequest request)
        {
            try
            {
                var categoryExists = await _categoryService.GetByIdAsync(request.CategoryId);
                if (categoryExists == null)
                {
                    throw new Exception("Danh mục không tồn tại");
                }
                if (request.ImageRequest == null || !request.ImageRequest.Any())
                {
                    throw new Exception("Danh sách ảnh trống");
                }
                var ingredient = _mapper.Map<Ingredient>(request);
                ingredient.IngredientCode = "P" + _source.GenerateRandom8Digits();
                if (!await _ingredientRepository.CheckCode(ingredient.IngredientCode))
                {
                    throw new Exception("Mã nguyên liệu đã tồn tại");
                }
                if (_source.CheckDate(ingredient.ExpiredDate) == 1 || _source.CheckDate(ingredient.ExpiredDate) == -1)
                {
                    throw new Exception("Hạng sử dụng chỉ còn 10 ngày hoặc hết hạn");
                }
                IngredientResponse ingredientResponse = _mapper.Map<IngredientResponse>(await _ingredientRepository.CreateAsync(ingredient));

                if (ingredientResponse == null)
                {
                    throw new Exception("Tạo nguyên liệu mới không thành công");
                }
                List<ImageRespone> imageRespones = await _imageSerivce.AddImages(ingredientResponse.Id, request.ImageRequest);
                ingredientResponse.Categories = _mapper.Map<CategoryResponse>(categoryExists);
                ingredientResponse.Images = imageRespones;
                return ingredientResponse;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: ", ex.Message);
                return null;
            }
        }

        public async Task<IngredientResponse> GetIngredientByIdAsync(Guid id)
        {
            var ingredient = await _ingredientRepository.GetByIdAsync(id);
            if (ingredient == null)
            {
                throw new KeyNotFoundException("Ingredient not found");
            }
            return _mapper.Map<IngredientResponse>(ingredient);
        }

        // public async Task<IEnumerable<Ingredient>> GetAllIngredientsAsync(
        //     string? search,
        //     Guid? categoryId,
        //     string? sortBy,
        //     bool isDescending,
        //     int page,
        //     int pageSize,
        //     DateTime? startDate,
        //     DateTime? endDate,
        //     IngredientStatus? status
        // )
        // {
        //     return await _ingredientRepository.GetAllAsync(
        //         search,
        //         categoryId,
        //         sortBy,
        //         isDescending,
        //         page,
        //         pageSize,
        //         startDate,
        //         endDate,
        //         status
        //     );
        // }
    }
}
