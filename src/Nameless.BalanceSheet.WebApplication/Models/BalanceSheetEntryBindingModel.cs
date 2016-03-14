using System;
using Nameless.BalanceSheet.Core.Models;

namespace Nameless.BalanceSheet.WebApplication.Models {
    public class BalanceSheetEntryBindingModel {
        #region Public Properties

        public string Description { get; set; }
        public string Category { get; set; }
        public decimal Value { get; set; }
        public DateTime Date { get; set; }
        public BalanceSheetEntryType Type { get; set; }
        public int Version { get; set; }

        #endregion
    }
}