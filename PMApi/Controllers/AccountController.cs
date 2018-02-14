using LicenseLibrary;
using LogLibrary;
using System;
using System.Web;
using System.Web.Http;

namespace PMApi.Controllers
{
    public class AccountController : ApiController
    {
        public IHttpActionResult Post([FromBody]RegistrationModel value)
        {
            string message = string.Empty;
            try
            {
                LicenseClass lc = new LicenseClass();
                message = lc.Register(value, HttpContext.Current.Request.UserHostAddress);
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.AppendLog("ACCOUNT", LogType.Error, "POST", ex.Message);
            }
            if (message.ToLower().Equals("success"))
                return Ok(message);
            else
                return BadRequest(message);

        }
    }
}
