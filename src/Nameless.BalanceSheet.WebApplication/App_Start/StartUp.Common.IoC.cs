using System;
using System.Linq;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using Nameless.BalanceSheet.Core.Models.Read;
using Nameless.BalanceSheet.Core.Models.Read.Impl;
using Nameless.Framework.Configuration;
using Nameless.Framework.Configuration.Impl;
using Nameless.Framework.Localization.Impl;
using Nameless.Framework.Logging.Impl;
using Nameless.IoC.Autofac;
using Nameless.IoC.Autofac.Impl;
using Owin;

namespace Nameless.BalanceSheet.WebApplication {
    public partial class StartUp {

        #region Private Methods

        private void ConfigureCommonIoC(IAppBuilder app, HttpConfiguration configuration) {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies()
                .Where(_ => _.GetName().Name.StartsWith("Nameless.BalanceSheet.WebApplication"))
                .ToArray();

            var compositionRoot = new CompositionRoot();
            compositionRoot.Register(_ => {
                var builder = (ContainerBuilder)_;

                builder.RegisterModule<NamelessCoreIOModule>();
                builder.RegisterModule<NamelessCoreTextModule>();
                builder.RegisterModule<NamelessFrameworkCachingModule>();
                builder.RegisterModule(new NamelessFrameworkConfigurationModule(typeof(JsonConfigurationProvider)));
                builder.RegisterModule<NamelessFrameworkEnvironmentModule>();
                builder.RegisterModule(new NamelessFrameworkEventSourcingModule(assemblies));
                builder.RegisterModule(new NamelessFrameworkIdentityModule(app));
                builder.RegisterModule(new NamelessFrameworkLocalizationModule(typeof(JsonLocalizationStreamParser)));
                builder.RegisterModule(new NamelessFrameworkLoggingModule(typeof(LoggerFactory)));
                builder.RegisterModule<NamelessFrameworkNetworkModule>();
                builder.RegisterModule(new NamelessFrameworkPersistenceModule(assemblies));
                builder.RegisterModule(new NamelessFrameworkProjectionsModule(assemblies));
                builder.RegisterModule<NamelessFrameworkServiceModule>();
                builder.RegisterModule(new NamelessFrameworkSearchModule(assemblies));
                builder.RegisterModule<NamelessFrameworkSecurityModule>();
                builder.RegisterModule<NamelessFrameworkXmlModule>();

                #region Application Registration

                builder.RegisterType<JsonConfigurationSectionConverter>().As<IJsonConfigurationSectionConverter>().SingleInstance();
                builder.RegisterType<BalanceSheetFacade>().As<IBalanceSheetFacade>().InstancePerRequest();

                #endregion Application Registration

                builder.RegisterApiControllers(assemblies);
            });
            compositionRoot.Resolve(_ => {
                var container = (IContainer)_;

                configuration.DependencyResolver = new AutofacWebApiDependencyResolver(container);

                app.UseAutofacMiddleware(container);
                app.UseAutofacWebApi(configuration);
            });
        }

        #endregion Private Methods
    }
}