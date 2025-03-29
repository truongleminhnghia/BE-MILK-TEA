﻿using Business_Logic_Layer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business_Logic_Layer.Services
{
    public interface ISearchService
    {
        List<SearchSuggestionDto> GetSuggestions(string keyword, int pageSize);
    }
}
