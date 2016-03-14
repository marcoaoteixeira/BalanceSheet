using Nameless.Core;
using Nameless.Framework.EventSourcing.Commands;
using Nameless.Framework.EventSourcing.Domain;
using Nameless.BalanceSheet.Core.Models.Write.Commands;
using Nameless.BalanceSheet.Core.Models.Write.Domain;

namespace Nameless.BalanceSheet.Core.Models.Write.Handlers {
    public class BalanceSheetEntryCommandHandler : ICommandHandler<CreateBalanceSheetEntry>,
                                                   ICommandHandler<ChangeBalanceSheetEntry>,
                                                   ICommandHandler<DeleteBalanceSheetEntry> {

        #region Private Read-Only Fields

        private readonly ISession _session;

        #endregion Private Read-Only Fields

        #region Public Constructors

        public BalanceSheetEntryCommandHandler(ISession session) {
            Ensure.ParameterNotNull(session, "session");

            _session = session;
        }

        #endregion Public Constructors

        #region ICommandHandler<CreateBalanceSheetEntry> Members

        public void Handle(CreateBalanceSheetEntry message) {
            var item = new BalanceSheetEntry(message.ID, message.Description, message.Category, message.Value, message.Date, message.Type);
            _session.Add(item);
            _session.Commit();
        }

        #endregion ICommandHandler<CreateBalanceSheetEntry> Members

        #region ICommandHandler<ChangeBalanceSheetEntry> Members

        public void Handle(ChangeBalanceSheetEntry message) {
            var item = _session.Get<BalanceSheetEntry>(message.ID, message.Version);

            item.Change(message.Description, message.Category, message.Version, message.Date, message.Type);

            _session.Commit();
        }

        #endregion ICommandHandler<ChangeBalanceSheetEntry> Members

        #region ICommandHandler<DeleteBalanceSheetEntry> Members

        public void Handle(DeleteBalanceSheetEntry message) {
            var item = _session.Get<BalanceSheetEntry>(message.ID, message.Version);

            item.Delete();

            _session.Commit();
        }

        #endregion ICommandHandler<DeleteBalanceSheetEntry> Members
    }
}