using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Nameless.Core;
using Nameless.Framework.Security;
using Nameless.Framework.Security.Providers;
using Nameless.Framework.Web.WebApi.Results;
using Nameless.BalanceSheet.WebApplication.Models.Account;

namespace Nameless.BalanceSheet.WebApplication.Controllers {
    [Authorize]
    [RoutePrefix("api/Account")]
    public class AccountController : WebApiController {
        #region Private Constants

        private const string LOCAL_LOGIN_PROVIDER = "Local";

        #endregion

        #region Private Fields

        private ApplicationUserManager _userManager;
        private ISecureDataFormat<AuthenticationTicket> _accessTokenFormat;

        #endregion

        #region Private Properties

        private IAuthenticationManager Authentication {
            get { return Request.GetOwinContext().Authentication; }
        }

        #endregion

        #region Public Constructors

        public AccountController(ApplicationUserManager userManager, ISecureDataFormat<AuthenticationTicket> accessTokenFormat) {
            Ensure.ParameterNotNull(userManager, "userManager");
            Ensure.ParameterNotNull(accessTokenFormat, "accessTokenFormat");

            _userManager = userManager;
            _accessTokenFormat = accessTokenFormat;
        }

        #endregion

        #region Public Methods

        [HttpGet /* GET api/Account/UserInfo */]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public IHttpActionResult GetUserInfo() {
            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return Ok(new UserInfoViewModel {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            });
        }

        [HttpPost /* POST api/Account/Logout */]
        [Route("Logout")]
        public IHttpActionResult Logout() {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);

            return Ok();
        }

        [HttpGet /* GET api/Account/ManageInfo?returnUrl=%2F&generateState=true */]
        [Route("ManageInfo")]
        public async Task<IHttpActionResult> GetManageInfo(string returnUrl, bool generateState = false) {
            var user = await _userManager.FindByIdAsync(User.Identity.GetUserId());

            if (user == null) {
                return null;
            }

            var logins = new List<UserLoginInfoViewModel>();

            foreach (var linkedAccount in user.Logins) {
                logins.Add(new UserLoginInfoViewModel {
                    LoginProvider = linkedAccount.LoginProvider,
                    ProviderKey = linkedAccount.ProviderKey
                });
            }

            if (user.PasswordHash != null) {
                logins.Add(new UserLoginInfoViewModel {
                    LoginProvider = LOCAL_LOGIN_PROVIDER,
                    ProviderKey = user.UserName,
                });
            }
            return Ok(new ManageInfoViewModel {
                LocalLoginProvider = LOCAL_LOGIN_PROVIDER,
                Email = user.UserName,
                Logins = logins,
                ExternalLoginProviders = GetExternalLogins(returnUrl, generateState)
            });
        }

        [HttpPost /* POST api/Account/ChangePassword */]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _userManager.ChangePasswordAsync(User.Identity.GetUserId()
                , model.OldPassword
                , model.NewPassword);

            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost /* POST api/Account/SetPassword */]
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var result = await _userManager.AddPasswordAsync(User.Identity.GetUserId()
                , model.NewPassword);

            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost /* POST api/Account/AddExternalLogin */]
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            var ticket = _accessTokenFormat.Unprotect(model.ExternalAccessToken);
            if (ticket == null || ticket.Identity == null || (ticket.Properties != null && ticket.Properties.ExpiresUtc.HasValue && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow)) {
                return BadRequest(T("External login failure.").Text);
            }

            var externalData = ExternalLoginData.FromIdentity(ticket.Identity);
            if (externalData == null) {
                return BadRequest(T("The external login is already associated with an account.").Text);
            }

            var result = await _userManager.AddLoginAsync(User.Identity.GetUserId()
                , new UserLoginInfo(externalData.LoginProvider
                    , externalData.ProviderKey));
            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost /* POST api/Account/RemoveLogin */]
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LOCAL_LOGIN_PROVIDER) {
                result = await _userManager.RemovePasswordAsync(User.Identity.GetUserId());
            } else {
                result = await _userManager.RemoveLoginAsync(User.Identity.GetUserId()
                    , new UserLoginInfo(model.LoginProvider
                        , model.ProviderKey));
            }

            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpGet /* GET api/Account/ExternalLogin */]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null) {
            if (error != null) {
                return Redirect(string.Concat(Url.Content("~/"), "#error=", Uri.EscapeDataString(error)));
            }

            if (!User.Identity.IsAuthenticated) {
                return new ChallengeResult(provider, this);
            }

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);
            if (externalLogin == null) {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider) {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user = await _userManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider
                , externalLogin.ProviderKey));
            var hasRegistered = user != null;
            if (hasRegistered) {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var oAuthIdentity = await user.GenerateUserIdentityAsync(_userManager
                    , OAuthDefaults.AuthenticationType);
                var cookieIdentity = await user.GenerateUserIdentityAsync(_userManager
                    , CookieAuthenticationDefaults.AuthenticationType);

                var properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            } else {
                var claims = externalLogin.GetClaims();
                var identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }

        [HttpGet /* GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true */]
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false) {
            var descriptions = Authentication.GetExternalAuthenticationTypes();
            var logins = new List<ExternalLoginViewModel>();

            string state = null;

            if (generateState) {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }

            foreach (var description in descriptions) {
                var login = new ExternalLoginViewModel {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = StartUp.PublicClientID,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }

        [HttpPost /* POST api/Account/Register */]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            return Ok();
        }

        [HttpPost /* POST api/Account/RegisterExternal */]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model) {
            if (!ModelState.IsValid) {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null) {
                return InternalServerError();
            }

            var user = new ApplicationUser {
                UserName = model.Email,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            result = await _userManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded) {
                return GetErrorResult(result);
            }

            return Ok();
        }

        #endregion

        #region Protected Override Methods

        protected override void Dispose(bool disposing) {
            if (disposing && _userManager != null) {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #endregion

        #region Private Methods

        private IHttpActionResult GetErrorResult(IdentityResult result) {
            if (result == null) {
                return InternalServerError();
            }

            if (!result.Succeeded) {
                if (result.Errors != null) {
                    foreach (var error in result.Errors) {
                        ModelState.AddModelError(string.Empty, error);
                    }
                }

                if (ModelState.IsValid) {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        #endregion

        #region Private Inner Class

        private class ExternalLoginData {
            #region Public Properties

            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            #endregion

            #region Public Methods

            public IList<Claim> GetClaims() {
                var claims = new List<Claim> {
                    new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider)
                };

                if (UserName != null) {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            #endregion

            #region Public Static Methods

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity) {
                if (identity == null) {
                    return null;
                }

                var providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || string.IsNullOrEmpty(providerKeyClaim.Issuer) || string.IsNullOrEmpty(providerKeyClaim.Value)) {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer) {
                    return null;
                }

                return new ExternalLoginData {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }

            #endregion
        }

        #endregion

        #region Private Static Inner Class

        private static class RandomOAuthStateGenerator {
            #region Private Static Read-Only Fields

            private static readonly RandomNumberGenerator Random = new RNGCryptoServiceProvider();

            #endregion

            #region Public Static Methods

            public static string Generate(int strengthInBits) {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0) {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                var strengthInBytes = strengthInBits / bitsPerByte;
                var data = new byte[strengthInBytes];

                Random.GetBytes(data);

                return HttpServerUtility.UrlTokenEncode(data);
            }

            #endregion
        }

        #endregion
    }
}
