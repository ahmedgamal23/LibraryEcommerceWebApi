namespace LibraryEcommerceWebApi.Extensions
{
    public static class RenderUnAuthorize
    {
        public static void Render(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
                {
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(
                        "{ \"error\": \"Not Authorize\", \"Status Code\" : 401  }"
                    );
                }
            });
        }

    }
}
