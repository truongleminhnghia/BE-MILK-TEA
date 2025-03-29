using Business_Logic_Layer.Models;
using Data_Access_Layer.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public class SearchService : ISearchService
    {
        private readonly ApplicationDbContext _context;

        public SearchService(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<SearchSuggestionDto> GetSuggestions(string keyword, int pageSize)
        {
            keyword = $"%{keyword}%"; // Wildcard search

            var results = _context.Recipes
                .Select(r => new { Id = r.Id, Type = "Recipe", Title = r.RecipeTitle })
                .Union(_context.Ingredients.Select(i => new { Id = i.Id, Type = "Ingredient", Title = i.IngredientName }))
                .Union(_context.Promotions.Select(p => new { Id = p.Id, Type = "Promotion", Title = p.PromotionDetail.PromotionName }))
                .ToList();

            // Compute Levenshtein Distance for approximate matching
            var scoredResults = results.Select(r => new
            {
                r.Id,
                r.Type,
                r.Title,
                Distance = LevenshteinDistance(r.Title.ToLower(), keyword.ToLower())
            })
            .OrderBy(r => r.Distance) // Sort by closest match
            .ThenBy(r => r.Title)     // Secondary sort alphabetically
            .Take(pageSize)
            .Select(r => new SearchSuggestionDto
            {
                Id = r.Id,
                Type = r.Type,
                Title = r.Title
            })
            .ToList();

            return scoredResults;
        }

        public static int LevenshteinDistance(string source, string target)
        {
            if (string.IsNullOrEmpty(source))
                return target?.Length ?? 0;
            if (string.IsNullOrEmpty(target))
                return source.Length;

            int[,] dp = new int[source.Length + 1, target.Length + 1];

            for (int i = 0; i <= source.Length; i++)
                dp[i, 0] = i;
            for (int j = 0; j <= target.Length; j++)
                dp[0, j] = j;

            for (int i = 1; i <= source.Length; i++)
            {
                for (int j = 1; j <= target.Length; j++)
                {
                    int cost = (source[i - 1] == target[j - 1]) ? 0 : 1;
                    dp[i, j] = Math.Min(
                        Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                        dp[i - 1, j - 1] + cost
                    );
                }
            }

            return dp[source.Length, target.Length];
        }
    }
}
