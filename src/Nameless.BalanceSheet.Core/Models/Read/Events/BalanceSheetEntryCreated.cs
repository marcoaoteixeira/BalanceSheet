using System;
using Nameless.Framework.EventSourcing.Events;

namespace Nameless.BalanceSheet.Core.Models.Read.Events {
    public class BalanceSheetEntryCreated : IEvent {
        #region Public Read-Only Properties

        public string Description { get; }
        public string Category { get; }
        public decimal Value { get; }
        public DateTime Date { get; }
        public BalanceSheetEntryType Type { get; }

        #endregion

        #region Public Constructors

        public BalanceSheetEntryCreated(Guid id, string description, string category, decimal value, DateTime date, BalanceSheetEntryType type) {
            ID = id;
            Description = description;
            Category = category;
            Value = value;
            Date = date;
            Type = type;
        }

        #endregion

        #region IEvent Members

        public Guid ID { get; set; }
        public DateTimeOffset TimeStamp { get; set; }
        public int Version { get; set; }

        #endregion
    }
}
