using Nameless.BalanceSheet.Core.Models.Read.Events;
using Nameless.Core;
using Nameless.Framework.EventSourcing.Events;
using Nameless.Framework.Persistence;

namespace Nameless.BalanceSheet.Core.Models.Read.Handlers {
    public class BalanceSheetEventHandler : IEventHandler<BalanceSheetEntryCreated>,
                                            IEventHandler<BalanceSheetEntryChanged>,
                                            IEventHandler<BalanceSheetEntryDeleted> {

        #region Private Read-Only Fields

        private readonly IRepository _repository;

        #endregion Private Read-Only Fields

        #region Public Constructors

        public BalanceSheetEventHandler(IRepository repository) {
            Ensure.ParameterNotNull(repository, "repository");

            _repository = repository;
        }

        #endregion Public Constructors

        #region IEventHandler<BalanceSheetEntryCreated> Members

        public void Handle(BalanceSheetEntryCreated message) {
            _repository.SaveOrUpdate(new BalanceSheetEntryDto {
                AggregateID = message.ID,
                Category = message.Category,
                Date = message.Date,
                Description = message.Description,
                Type = message.Type,
                Value = message.Value,
                Version = message.Version
            });
        }

        #endregion IEventHandler<BalanceSheetEntryCreated> Members

        #region IEventHandler<BalanceSheetEntryChanged>

        public void Handle(BalanceSheetEntryChanged message) {
            var entry = _repository.FindOne<BalanceSheetEntryDto>(_ => _.AggregateID == message.ID);

            entry.Category = message.Category;
            entry.Description = message.Description;
            entry.Type = message.Type;
            entry.Value = message.Value;
            entry.Version = message.Version;

            _repository.SaveOrUpdate(entry);
        }

        #endregion IEventHandler<BalanceSheetEntryChanged>

        #region IEventHandler<BalanceSheetEntryDeleted>

        public void Handle(BalanceSheetEntryDeleted message) {
            var entry = _repository.FindOne<BalanceSheetEntryDto>(_ => _.AggregateID == message.ID);

            _repository.Delete(entry);
        }

        #endregion IEventHandler<BalanceSheetEntryDeleted>
    }
}