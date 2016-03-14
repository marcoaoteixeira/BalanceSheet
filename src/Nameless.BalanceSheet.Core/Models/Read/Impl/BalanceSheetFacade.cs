using System;
using System.Collections.Generic;
using Nameless.Core;
using Nameless.Framework.Persistence;

namespace Nameless.BalanceSheet.Core.Models.Read.Impl {
    public class BalanceSheetFacade : IBalanceSheetFacade {
        #region Private Read-Only Fields

        private readonly IRepository _repository;

        #endregion

        #region Public Constructors

        public BalanceSheetFacade(IRepository repository) {
            Ensure.ParameterNotNull(repository, "repository");

            _repository = repository;
        }

        #endregion

        #region IBalanceSheetFacade Members

        public BalanceSheetEntryDto Get(Guid id) {
            return _repository.FindOne<BalanceSheetEntryDto>(id);
        }

        public IEnumerable<BalanceSheetEntryDto> List(DateTime begin, DateTime end) {
            return _repository
                .Query<BalanceSheetEntryDto>()
                .Where(_ => _.Date >= begin && _.Date <= end)
                .ToResult()
                .Items;
        }

        #endregion
    }
}