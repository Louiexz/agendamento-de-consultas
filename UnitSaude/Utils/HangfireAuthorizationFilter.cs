using Hangfire.Dashboard;

namespace UnitSaude.Utils
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize(DashboardContext context)
        {
            // Em produção, implemente uma lógica de autorização adequada
            var httpContext = context.GetHttpContext();

            // Exemplo: Permitir apenas usuários autenticados com role de administrador
            // return httpContext.User.Identity.IsAuthenticated && httpContext.User.IsInRole("Administrador");

            // Para desenvolvimento, permitir todos (remova isso em produção)
            return true;
        }
    }
}
