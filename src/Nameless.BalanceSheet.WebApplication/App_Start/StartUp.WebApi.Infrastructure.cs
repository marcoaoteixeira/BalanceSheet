using System.Web.Http;
using Microsoft.Owin.Security.OAuth;

namespace Nameless.BalanceSheet.WebApplication {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureWebApiInfrastructure(HttpConfiguration configuration) {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            configuration.SuppressDefaultHostAuthentication();
            configuration.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            // Web API routes
            configuration.MapHttpAttributeRoutes();
        }

        #endregion
    }
}