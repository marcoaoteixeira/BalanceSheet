using System;
using System.Collections.Generic;

namespace Nameless.BalanceSheet.Core.Models.Read {
    public interface IBalanceSheetFacade {
        #region Methods

        BalanceSheetEntryDto Get(Guid id);
        IEnumerable<BalanceSheetEntryDto> List(DateTime begin, DateTime end);

        #endregion
    }
}