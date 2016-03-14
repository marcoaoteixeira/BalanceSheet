using System.ComponentModel.DataAnnotations;
using Nameless.BalanceSheet.WebApplication.Resources;

namespace Nameless.BalanceSheet.WebApplication.Models.Account {
    public class RemoveLoginBindingModel {
        #region Public Properties

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Displays), Name = "LoginProvider")]
        public string LoginProvider { get; set; }

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Displays), Name = "ProviderKey")]
        public string ProviderKey { get; set; }

        #endregion
    }
}