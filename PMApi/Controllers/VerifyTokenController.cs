using DataLibrary;
using LogLibrary;
using PMApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PMApi.Controllers
{
    [Authorize]
    public class VerifyTokenController : ApiController
    {
        public IHttpActionResult Get()
        {
            return Ok();
        }
    }
}
