using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Nameless.BalanceSheet.Core.Models.Read;
using Nameless.BalanceSheet.Core.Models.Write.Commands;
using Nameless.BalanceSheet.WebApplication.Models;
using Nameless.Core;
using Nameless.Framework.EventSourcing.Bus;

namespace Nameless.BalanceSheet.WebApplication.Controllers {
    public class BalanceSheetController : WebApiController {
        #region Private Read-Only Fields

        private readonly ICommandBus _commandBus;
        private readonly IBalanceSheetFacade _balanceSheetFacade;

        #endregion

        #region Public Constructors

        public BalanceSheetController(ICommandBus commandBus, IBalanceSheetFacade balanceSheetFacade) {
            Ensure.ParameterNotNull(commandBus, "commandBus");
            Ensure.ParameterNotNull(balanceSheetFacade, "balanceSheetFacade");

            _commandBus = commandBus;
            _balanceSheetFacade = balanceSheetFacade;
        }

        #endregion

        #region Public Methods

        [HttpDelete]
        public IHttpActionResult Delete(Guid id, int version) {
            _commandBus.Send(new DeleteBalanceSheetEntry(id, version));

            return Ok(Request.CreateResponse(HttpStatusCode.NoContent));
        }

        [HttpGet]
        public IHttpActionResult Get(Guid id) {
            var dto = _balanceSheetFacade.Get(id);

            return Ok(dto);
        }

        [HttpPost]
        public IHttpActionResult Post(BalanceSheetEntryBindingModel model) {
            var id = Guid.NewGuid();

            _commandBus.Send(new CreateBalanceSheetEntry(id, model.Description, model.Category, model.Value, model.Date, model.Type));

            return Created(Url.Route("Get", new { id = id }), model);
        }

        [HttpPut]
        public IHttpActionResult Put(Guid id, BalanceSheetEntryBindingModel model) {
            _commandBus.Send(new ChangeBalanceSheetEntry(id, model.Description, model.Category, model.Value, model.Date, model.Type, model.Version));

            return Ok(model);
        }

        #endregion
    }
}