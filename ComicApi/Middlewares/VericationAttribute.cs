using ComicApi.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ComicApi.Middlewares
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class VericationAttribute : Attribute, IAsyncAuthorizationFilter
    {
        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.HttpContext.GetEndpoint()?.Metadata.GetMetadata<IAllowAnonymous>() != null) return;

            context.HttpContext.Request.Headers.TryGetValue("appkey", out var appKeyValue);
            var appKey = appKeyValue.ToString();
            if (false == appKey.Equals("alu.idv.tw", StringComparison.CurrentCultureIgnoreCase))
            {
                context.Result = new UnauthorizedObjectResult(
                new ResponseModel()
                {
                    Data = new object(),
                    Message = "驗證錯誤",
                    Success = false,
                    Code = 401
                });
            }
        }
    }
}
