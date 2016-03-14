using System;
using Nameless.Framework.EventSourcing.Domain;
using Nameless.BalanceSheet.Core.Models.Read.Events;

namespace Nameless.BalanceSheet.Core.Models.Write.Domain {
    public class BalanceSheetEntry : AggregateRoot  {
        #region Public Constructors

        public BalanceSheetEntry(Guid id, string description, string category, decimal value, DateTime date, BalanceSheetEntryType type) {
            ID = id;

            ApplyChange(new BalanceSheetEntryCreated(id, description, category, value, date, type));
        }

        #endregion

        #region Public Methods

        public void Change(string description, string category, decimal value, DateTime date, BalanceSheetEntryType type) {
            ApplyChange(new BalanceSheetEntryChanged(ID, description, category, value, date, type));
        }

        public void Delete() {
            ApplyChange(new BalanceSheetEntryDeleted(ID));
        }

        #endregion
    }
}