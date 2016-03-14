using System.ComponentModel.DataAnnotations;
using Nameless.BalanceSheet.WebApplication.Resources;

namespace Nameless.BalanceSheet.WebApplication.Models.Account {
    public class AddExternalLoginBindingModel {
        #region Public Properties

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Displays), Name = "ExternalAccessToken")]
        public string ExternalAccessToken { get; set; }

        #endregion
    }
}