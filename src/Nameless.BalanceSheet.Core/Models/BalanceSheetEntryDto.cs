using System;
using Nameless.Framework.Models;

namespace Nameless.BalanceSheet.Core.Models {
    public class BalanceSheetEntryDto : EntityBase {
        #region Public Read-Only Properties

        public virtual Guid AggregateID { get; set; }
        public virtual string Description { get; set; }
        public virtual string Category { get; set; }
        public virtual decimal Value { get; set; }
        public virtual DateTime Date { get; set; }
        public virtual BalanceSheetEntryType Type { get; set; }
        public virtual int Version { get; set; }

        #endregion
    }
}