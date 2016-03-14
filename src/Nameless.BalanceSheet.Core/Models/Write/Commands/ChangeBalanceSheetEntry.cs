using System;
using Nameless.Framework.EventSourcing.Commands;

namespace Nameless.BalanceSheet.Core.Models.Write.Commands {
    public class ChangeBalanceSheetEntry : ICommand {
        #region Public Read-Only Fields

        public string Description { get; }
        public string Category { get; }
        public decimal Value { get; }
        public DateTime Date { get; }
        public BalanceSheetEntryType Type { get; }

        #endregion

        #region Public Constructors

        public ChangeBalanceSheetEntry(Guid id, string description, string category, decimal value, DateTime date, BalanceSheetEntryType type, int originalVersion) {
            ID = id;
            Description = description;
            Category = category;
            Value = value;
            Date = date;
            Type = type;
            Version = originalVersion;
        }

        #endregion

        #region ICommand Members

        public Guid ID { get; set; }
        public int Version { get; set; }

        #endregion
    }
}