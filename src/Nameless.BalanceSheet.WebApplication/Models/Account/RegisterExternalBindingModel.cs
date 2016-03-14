using System.ComponentModel.DataAnnotations;
using Nameless.BalanceSheet.WebApplication.Resources;

namespace Nameless.BalanceSheet.WebApplication.Models.Account {
    public class RegisterExternalBindingModel {
        #region Public Properties

        [Required(ErrorMessageResourceType = typeof(Validations), ErrorMessageResourceName = "Required")]
        [Display(ResourceType = typeof(Displays), Name = "Email")]
        public string Email { get; set; }

        #endregion
    }
}