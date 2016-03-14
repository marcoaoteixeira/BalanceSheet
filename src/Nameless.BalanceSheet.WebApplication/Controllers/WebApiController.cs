using System.Web.Http;
using Nameless.Framework.Localization;
using Nameless.Framework.Localization.Impl;
using Nameless.Framework.Logging;
using Nameless.Framework.Logging.Impl;

namespace Nameless.BalanceSheet.WebApplication.Controllers {
    public abstract class WebApiController : ApiController {
        #region Public Properties

        private ILogger _logger;
        public ILogger MyProperty {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value ?? NullLogger.Instance; }
        }

        private Localizer _localizer;
        public Localizer T {
            get { return _localizer ?? (_localizer = NullLocalizer.Instance); }
            set { _localizer = value ?? NullLocalizer.Instance; }
        }

        #endregion
    }
}