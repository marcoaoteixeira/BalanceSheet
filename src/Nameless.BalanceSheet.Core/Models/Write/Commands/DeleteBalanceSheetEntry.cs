using System;
using Nameless.Framework.EventSourcing.Commands;

namespace Nameless.BalanceSheet.Core.Models.Write.Commands {
    public class DeleteBalanceSheetEntry : ICommand {
        #region Public Constructors

        public DeleteBalanceSheetEntry(Guid id, int originalVersion) {
            ID = id;
            Version = originalVersion;
        }

        #endregion

        #region ICommand Members

        public Guid ID { get; set; }
        public int Version { get; set; }

        #endregion
    }
}