using System.Web.Http;
using Microsoft.Owin;
using Nameless.BalanceSheet.WebApplication;
using Owin;

[assembly: OwinStartup(typeof(StartUp))]
namespace Nameless.BalanceSheet.WebApplication {
    public partial class StartUp {
        #region Private Read-Only Fields

        private readonly HttpConfiguration _configuration;

        #endregion

        #region Public Constructors

        public StartUp() {
            _configuration = GlobalConfiguration.Configuration;
        }

        #endregion

        #region Public Methods

        public void Configuration(IAppBuilder app) {
            // Configure WEBAPI
            ConfigureWebApiFilters(_configuration.Filters);
            ConfigureWebApiRoutes(_configuration.Routes);
            ConfigureWebApiInfrastructure(_configuration);

            // Configure Common
            ConfigureCommonInfrastructure();
            ConfigureCommonIoC(app, _configuration);
            ConfigureAuth(app);

            _configuration.EnsureInitialized();
        }

        #endregion
    }
}