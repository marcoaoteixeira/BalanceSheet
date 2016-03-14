using System.Web.Http;

namespace Nameless.BalanceSheet.WebApplication {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureWebApiRoutes(HttpRouteCollection routes) {
            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }

        #endregion
    }
}