using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using BasicBilling.API.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BasicBilling.API.Utils
{
    public class PaymentRequestFilterAttribute : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                foreach (var entry in context.ModelState)
                {
                    if (entry.Key == "$.clientId")
                    {
                        var errors = entry.Value.Errors.ToList();
                        foreach (var error in errors)
                        {
                            if (error.ErrorMessage.Contains("convert"))
                            {
                                var customError = new ModelError("The 'clientId' field must be a number and greater than 02");
                                entry.Value.Errors.Remove(error);
                                entry.Value.Errors.Add(customError);
                            }
                        }
                    }
                    if (entry.Key == "$.period")
                    {
                        var errors = entry.Value.Errors.ToList();
                        foreach (var error in errors)
                        {
                            if (error.ErrorMessage.Contains("convert"))
                            {
                                var customError = new ModelError("The field 'period' must be a number in YYYYMM format");
                                entry.Value.Errors.Remove(error);
                                entry.Value.Errors.Add(customError);
                            }
                        }
                    }
                    if (entry.Key == "paymentRequest")
                    {
                        var errors = entry.Value.Errors.ToList();
                        foreach (var error in errors)
                        {
                            entry.Value.Errors.Remove(error);
                        }
                    }
                    if (entry.Key == "$.category" || entry.Key == "Category")
                    {
                        var errors = entry.Value.Errors.ToList();
                        foreach (var error in errors)
                        {
                            if (error.ErrorMessage.Contains("convert") || error.ErrorMessage.Contains("valid Category"))
                            {
                                var validCategories = string.Join(", ", Enum.GetNames(typeof(BillCategory)));
                                var customError = new ModelError($"The 'category' field must be a valid Category. Valid values are: {validCategories}");
                                entry.Value.Errors.Remove(error);
                                entry.Value.Errors.Add(customError);
                            }
                        }
                    }
                }
                context.Result = new BadRequestObjectResult(new ErrorResponse
                {
                    Errors = context.ModelState.Values.SelectMany(x => x.Errors, (x, y) => y.ErrorMessage).ToList()
                });
            } else
            {
                var model = context.ActionArguments["paymentRequest"] as PaymentRequest;
                if (!Enum.IsDefined(typeof(BillCategory), model!.Category))
                {
                    var validCategories = string.Join(", ", Enum.GetNames(typeof(BillCategory)));
                    context.Result = new BadRequestObjectResult(new ErrorResponse
                    {
                        Errors = { $"The 'category' field must be a valid Category. Valid values are: {validCategories}" }
                    });
                }
            }
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }
}
