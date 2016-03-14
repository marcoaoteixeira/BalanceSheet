using System.Web.Http;

namespace Nameless.BalanceSheet.WebApplication.Controllers {
    public class HomeController : WebApiController {
        #region Public Methods

        public IHttpActionResult Get() {
            return Ok(new[] { "1", "2", "3" });
        }

        #endregion
    }
}