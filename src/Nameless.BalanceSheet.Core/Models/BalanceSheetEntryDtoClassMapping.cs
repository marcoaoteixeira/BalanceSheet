using Nameless.Framework.Persistence;
using NHibernate.Type;

namespace Nameless.BalanceSheet.Core.Models {
    public class BalanceSheetEntryDtoClassMapping : EntityBaseClassMapping<BalanceSheetEntryDto> {
        #region Public Constructors

        public BalanceSheetEntryDtoClassMapping()
            : base("balance_sheet_entries", "balance_sheet_entry_id") {
            Property(property => property.AggregateID
                , mapping => {
                    mapping.Column("aggregate_id");
                    mapping.Length(256);
                    mapping.NotNullable(true);
                    mapping.Unique(true);
                    mapping.Index("idx_balance_sheet_entry_aggregate_id");
                });
            Property(property => property.Description
                , mapping => {
                    mapping.Column("description");
                    mapping.Length(256);
                    mapping.NotNullable(true);
                });
            Property(property => property.Category
                , mapping => {
                    mapping.Column("category");
                    mapping.Length(256);
                    mapping.NotNullable(true);
                });
            Property(property => property.Value
                , mapping => {
                    mapping.Column("value");
                    mapping.NotNullable(true);
                    mapping.Precision(16);
                    mapping.Scale(3);
                });
            Property(property => property.Date
                , mapping => {
                    mapping.Column("date");
                    mapping.NotNullable(true);
                });
            Property(property => property.Type
                , mapping => {
                    mapping.Column("type");
                    mapping.NotNullable(true);
                    mapping.Type<EnumStringType<BalanceSheetEntryType>>();
                });
            Property(property => property.Version
                , mapping => {
                    mapping.Column("version");
                    mapping.NotNullable(true);
                });
        }

        #endregion
    }
}