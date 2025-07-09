﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tournament.Core.Request
{
    public class RequestParams
    {
        [Range(1, int.MaxValue)]
        public int PageNumber { get; set; } = 1;

        [Range(1, 20)]
        public int PageSize { get; set; } = 20;
    }

    public class CompanyRequestParams : RequestParams
    {
        public bool IncludeEmployees { get; set; } = false;
    }
}
