using System;
using Nameless.Framework.EventSourcing.Commands;

namespace Nameless.BalanceSheet.Core.Models.Write.Commands {
    public class CreateBalanceSheetEntry : ICommand {
        #region Public Read-Only Fields

        public string Description { get; }
        public string Category { get; }
        public decimal Value { get; }
        public DateTime Date { get; }
        public BalanceSheetEntryType Type { get; }

        #endregion

        #region Public Constructors

        public CreateBalanceSheetEntry(Guid id, string description, string category, decimal value, DateTime date, BalanceSheetEntryType type) {
            ID = id;
            Description = description;
            Category = category;
            Value = value;
            Date = date;
            Type = type;
        }

        #endregion

        #region ICommand Members

        public Guid ID { get; set; }
        public int Version { get; set; }

        #endregion
    }
}