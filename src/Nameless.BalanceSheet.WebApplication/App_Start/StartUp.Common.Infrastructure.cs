using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Nameless.BalanceSheet.WebApplication {
    public partial class StartUp {
        #region Private Methods

        private void ConfigureCommonInfrastructure() {
            JsonConvert.DefaultSettings = () => {
                return new JsonSerializerSettings {
                    ContractResolver = new DefaultContractResolver {
                        IgnoreSerializableAttribute = true
                    }
                };
            };
        }

        #endregion
    }
}