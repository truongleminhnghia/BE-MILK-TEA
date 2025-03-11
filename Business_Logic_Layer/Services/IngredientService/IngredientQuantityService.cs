using AutoMapper;
using Business_Logic_Layer.Models.Requests;
using Business_Logic_Layer.Models.Responses;
using Data_Access_Layer.Entities;
using Data_Access_Layer.Enum;
using Data_Access_Layer.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services.IngredientService
{
    public class IngredientQuantityService : IIngredientQuantityService
    {
        private readonly IIngredientQuantityRepository _ingredientQuantityRepository;
        private readonly IMapper _mapper;
        private readonly IIngredientRepository _ingredientRepository;

        public IngredientQuantityService(IIngredientQuantityRepository ingredientQuantityRepository, IMapper mapper, IIngredientRepository ingredientRepository)
        {
            _ingredientQuantityRepository = ingredientQuantityRepository;
            _mapper = mapper;
            _ingredientRepository = ingredientRepository;
        }

        public async Task<PageResult<IngredientQuantityResponse>> GetAllAsync(
    string? search,
    Guid? ingredientId,
    ProductType? productType,
    int? minQuantity,
    int? maxQuantity,
    DateTime? startDate,
    DateTime? endDate,
    string? sortBy,
    bool isDescending,
    int pageCurrent,
    int pageSize)
        {
            var query = _ingredientQuantityRepository.GetAll(
                search, ingredientId, productType, minQuantity, maxQuantity, startDate, endDate
            );

            // Sắp xếp dữ liệu
            var validSortColumns = new HashSet<string> { "Quantity", "CreateAt", "UpdateAt", "ProductType" };
            if (!string.IsNullOrEmpty(sortBy) && validSortColumns.Contains(sortBy))
            {
                query = isDescending
                    ? query.OrderByDescending(e => EF.Property<object>(e, sortBy))
                    : query.OrderBy(e => EF.Property<object>(e, sortBy));
            }
            else
            {
                query = query.OrderByDescending(iq => iq.CreateAt);
            }

            // Tổng số bản ghi
            int total = await query.CountAsync();

            // Phân trang
            var items = await query
                .Skip((pageCurrent - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PageResult<IngredientQuantityResponse>
            {
                Data = _mapper.Map<List<IngredientQuantityResponse>>(items),
                PageCurrent = pageCurrent,
                PageSize = pageSize,
                Total = total
            };
        }


        public async Task<List<IngredientQuantityResponse>> GetByIngredientId(Guid ingredientId)
        {
            try
            {
                var ingredientQuantities = await _ingredientQuantityRepository.GetByIngredientId(ingredientId);
                if (ingredientQuantities == null || !ingredientQuantities.Any())
                {
                    return new List<IngredientQuantityResponse>();
                }
                return _mapper.Map<List<IngredientQuantityResponse>>(ingredientQuantities);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return new List<IngredientQuantityResponse>();
            }
        }

        public async Task<IngredientQuantityResponse> CreateAsync(IngredientQuantityRequest request)
        {
            try
            {
                var ingredient = await _ingredientRepository.GetById(request.IngredientId);
                if (ingredient == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy nguyên liệu");
                }

                var newIngredientQuantity = _mapper.Map<IngredientQuantity>(request);
                newIngredientQuantity.CreateAt = DateTime.UtcNow;

                await _ingredientQuantityRepository.AddAsync(newIngredientQuantity);
                return _mapper.Map<IngredientQuantityResponse>(newIngredientQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo IngredientQuantity: " + ex.Message);
            }
        }

        public async Task<IngredientQuantityResponse> UpdateAsync(Guid id, IngredientQuantityRequest request)
        {
            try
            {
                var ingredientQuantity = await _ingredientQuantityRepository.GetById(id);
                if (ingredientQuantity == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy mẫu mã sản phẩm");
                }

                _mapper.Map(request, ingredientQuantity);
                ingredientQuantity.UpdateAt = DateTime.UtcNow;

                await _ingredientQuantityRepository.UpdateAsync(ingredientQuantity);
                return _mapper.Map<IngredientQuantityResponse>(ingredientQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật IngredientQuantity: " + ex.Message);
            }
        }

        public async Task<IngredientQuantityResponse> GetByIdAndProductType(Guid ingredientId, ProductType ProductType)
        {
            try
            {
                var ingredientQuantity = await _ingredientQuantityRepository.GetByIdAndProductType(ingredientId, ProductType);
                if (ingredientQuantity == null)
                {
                    throw new KeyNotFoundException("Không tìm thấy nguyên liệu với loại sản phẩm đã chỉ định");
                }
                return _mapper.Map<IngredientQuantityResponse>(ingredientQuantity);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi truy vấn IngredientQuantity: " + ex.Message);
            }
        }
    }
}
