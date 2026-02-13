using Microsoft.EntityFrameworkCore;

namespace FolioWebAPI.Extensions
{
    public static class HttpContextExtensions
    {
        public async static Task 
            InsertPaginationParametersInHeader<T>(this HttpContext httpContext, IQueryable<T> queryable)
        {
            if (httpContext is null)
                ArgumentNullException.ThrowIfNull(httpContext);

            double amount = await queryable.CountAsync();

            httpContext.Response.Headers.Append("total-records-amount", amount.ToString());
        }
    }
}
