public class RedirectMiddleware
{
    private readonly RequestDelegate _next;

    public RedirectMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Проверяем, не авторизован ли пользователь
        if (!context.User.Identity.IsAuthenticated)
        {
            // Список страниц, доступных без авторизации
            var allowedPaths = new[] { "/Login", "/Registration", "/AccessDenied" };
            var currentPath = context.Request.Path;

            // Если текущая страница не в списке разрешенных
            if (!allowedPaths.Any(path => currentPath.StartsWithSegments(path)))
            {
                // Перенаправляем на страницу входа
                context.Response.Redirect("/Login");
                return;
            }
        }

        await _next(context);
    }
}