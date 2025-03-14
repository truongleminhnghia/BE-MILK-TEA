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
                //var ingredient = await _ingredientRepository.GetById(request.IngredientId);
                //if (ingredient == null)
                //{
                //    throw new KeyNotFoundException("Không tìm thấy nguyên liệu");
                //}
                if (request.Id != null) request.Id = null;
                var newIngredientQuantity = _mapper.Map<IngredientQuantity>(request);
                newIngredientQuantity.CreateAt = DateTime.UtcNow;

                var created = await _ingredientQuantityRepository.AddAsync(newIngredientQuantity);
                var response = _mapper.Map<IngredientQuantityResponse>(created);
                response.Id = created.Id;
                return response;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi tạo IngredientQuantity: " + ex.Message);
            }
        }
        public async Task<List<IngredientQuantityResponse>> CreateQuantitiesAsync(Guid ingredientId, List<IngredientQuantityRequest> request)
        {
            try
            {
                var list = new List<IngredientQuantityResponse>();
                var ingredientExisted = await _ingredientRepository.GetById(ingredientId);
                if (ingredientExisted == null)
                {
                    throw new Exception("Nguyên liệu không tồn tại");
                }
                foreach (var item in request)
                {
                    item.IngredientId = ingredientId;
                    var savedQuantities = await CreateAsync(item);
                    list.Add(savedQuantities);
                }
                return list;
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
        public async Task<List<IngredientQuantityResponse>> UpdateQuantitiesAsync(Guid ingredientId, List<IngredientQuantityRequest> request)
        {
            try
            {
                var list = new List<IngredientQuantityResponse>();
                var ingredientExisted = await _ingredientRepository.GetById(ingredientId);
                if (ingredientExisted == null)
                {
                    throw new Exception("Nguyên liệu không tồn tại");
                }
                foreach (var item in request)
                {
                    item.IngredientId = ingredientId;
                    var savedQuantities = await UpdateAsync((Guid)item.Id, item);
                    list.Add(savedQuantities);
                }
                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật danh sach IngredientQuantity: " + ex.Message);
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
        public async Task<IngredientQuantity> Save(IngredientQuantity ingredientQuantity)
        {
            await _ingredientQuantityRepository.AddAsync(ingredientQuantity);
            return ingredientQuantity;
        }

        public Task<IngredientQuantity> SaveList(Guid ingredientId, List<IngredientQuantity> ingredientQuantity)
        {
            throw new NotImplementedException();
        }
    }
}
