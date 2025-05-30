﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Data_Access_Layer.Entities;

namespace Data_Access_Layer.Repositories
{
    public interface IImageRepository
    {
        Task<List<Image>> GetByIdAndIngredientByList(Guid id, Guid ingredientId);
        Task<List<Image>> GetByIngredient(Guid ingredientId);
        Task<Image> GetIdAndIngredient(Guid? id, Guid ingredientId);
        Task<Image> AddImageAsync(Image image);
        Task<Image> GetById(Guid id);
        Task<bool> Update(Guid id, Image image);
        Task<bool> Delete(Guid id);
    }
}
