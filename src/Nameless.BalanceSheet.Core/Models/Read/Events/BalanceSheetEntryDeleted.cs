using System;
using Nameless.Framework.EventSourcing.Events;

namespace Nameless.BalanceSheet.Core.Models.Read.Events {
    public class BalanceSheetEntryDeleted : IEvent {
        #region Public Constructors

        public BalanceSheetEntryDeleted(Guid id) {
            ID = id;
        }

        #endregion

        #region IEvent Members

        public Guid ID { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public int Version { get; set; }

        #endregion
    }
}
