namespace Nameless.BalanceSheet.Core.Models {
    public enum BalanceSheetEntryType : int {
        Payment,
        Credit,
        TransferBetweenAccounts,
        TransferForOthers,
        Withdraw
    }
}