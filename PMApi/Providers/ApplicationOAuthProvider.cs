using LicenseLibrary;
using LogLibrary;
using Microsoft.Owin.Security.OAuth;
using PMApi.Auditing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;

namespace PMApi.Providers
{
    /// <summary>
    /// Describe your member here.
    /// </summary>
    public class ApplicationOAuthProvider : OAuthAuthorizationServerProvider
    {
        //LoginDetails details = null;
        LoginDetails ld = null;
        string AuthenticationType = string.Empty;
        /// <summary>
        /// Describe your member here.
        /// </summary>
        public ApplicationOAuthProvider() { }
        /// <summary>
        /// Describe your member here.
        /// </summary>
        public ApplicationOAuthProvider(string AuthType) { AuthenticationType = AuthType; }

        /// <summary>
        /// Describe your member here.
        /// </summary>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            context.Validated();
        }
        /// <summary>
        /// Describe your member here.
        /// </summary>

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { "*" });

            LicenseClass lc = new LicenseLibrary.LicenseClass();
            ld = lc.Authenticate(context.UserName, context.Password, HttpContext.Current.Request.UserHostAddress);

            if (ld.is_success)
            {
                //if (AuditLogs.verifyAllowedIpAccess(ld.MasterCustomerID, ld.UserIPAddress))
                //{
                    ClaimsIdentity oAuthIdentity = new ClaimsIdentity(context.Options.AuthenticationType);
                    oAuthIdentity.AddClaim(new Claim(ClaimTypes.Name, context.UserName));
                    oAuthIdentity.AddClaim(new Claim(ClaimTypes.NameIdentifier, "user"));
                    context.Validated(oAuthIdentity);
                //}
                //else
                //{
                //    context.Rejected();
                //}
            }
            else
            {
                context.Rejected();
                context.SetError("server error", ld.login_status);
            }

        }

        /// <summary>
        /// Describe your member here.
        /// </summary>
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            context.AdditionalResponseParameters.Add("email", ld.email_address);
            context.AdditionalResponseParameters.Add("first_name", ld.first_name);
            context.AdditionalResponseParameters.Add("last_name", ld.last_name);
            //context.AdditionalResponseParameters.Add("mastercustomerid", ld.MasterCustomerID);
            //context.AdditionalResponseParameters.Add("user_id", ld.UserID);
            context.AdditionalResponseParameters.Add("role", ld.role);
            //context.AdditionalResponseParameters.Add("speciality", ld.Speciality);
            context.AdditionalResponseParameters.Add("zip", ld.zip);
            context.AdditionalResponseParameters.Add("phone_number", ld.phone_number);
            context.AdditionalResponseParameters.Add("country ", ld.country);

            if (!ld.is_two_step_enabled)
            {
                context.AdditionalResponseParameters.Add("status", "success");
            }
            else
            {
                context.AdditionalResponseParameters.Add("status", "TFAC");
            }
            return Task.FromResult<object>(null);
        }

        /// <summary>
        /// Describe your member here.
        /// </summary>
        public override Task TokenEndpointResponse(OAuthTokenEndpointResponseContext context)
        {
            bool flag = false;
            try
            {
                DateTime expiry = DateTime.Now.AddMinutes(context.Options.AccessTokenExpireTimeSpan.TotalMinutes);
                context.AdditionalResponseParameters.Add("expiry", expiry.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'"));
                flag = AuditLogs.insertCustomerAuthCall(ld.id.ToString(), ld.first_name + " " + ld.last_name, context.Request.Host.Value, ld.email_address, ld.zip, ld.phone_number, context.AccessToken, HttpContext.Current.Request.UserHostAddress, (!ld.is_two_step_enabled).ToString(), ld.verificationCode);
                //flag = AuditLogs.insertCustomerAuthCall(ld.last_name + " " + ld.first_name, ld.UserID, context.Request.Host.Value, ld.DBServerID, ld.EmailID, ld.MasterCustomerID, ld.Speciality, ld.zip, ld.PhoneNumber, context.AccessToken, ld.UserIPAddress, (!ld.IS2wayEnabled).ToString(), ld.verificationCode, ld.User_Type);
            }
            catch (Exception ex)
            {
                FileLogger.AppendLog("POWebAPI", LogType.Error, "POWebAPI >> ApplicationOAuthProvider >> GrantResourceOwnerCredentials >> ", ex.Message);
            }
            if (flag)
                return base.TokenEndpointResponse(context);
            else
            {
                return null;
            }
        }
    }
}