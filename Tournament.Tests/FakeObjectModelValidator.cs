using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tournament.Core.DTOs;

namespace Tournament.Tests
{
    public class FakeObjectModelValidator : IObjectModelValidator
    {
        public void Validate(ActionContext actionContext, ValidationStateDictionary validationState, string prefix, object model)
        {
            //Används inte

            //var dto = model as GameUpdateDTO;

            //// Simulate failure when Title is null or empty
            //if (string.IsNullOrEmpty(dto?.Title))
            //{
            //    actionContext.ModelState.AddModelError("Title", "Title is required.");
            //}
        }
    }
}
