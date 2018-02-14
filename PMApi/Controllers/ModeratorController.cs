using BusinessLibrary;
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
    public class ModeratorController : ApiController
    {
        
        public IHttpActionResult Get()
        {
            string message = "";
            bool flag = false;
            List<GamePostModel> data = new List<GamePostModel>();
            try
            {
                object infoObject;
                Request.Properties.TryGetValue(Constants.IdentificationInfo, out infoObject);
                DBServerIdentification identificationInfo = (DBServerIdentification)infoObject;
                DataSQL objSQL = new DataSQL(identificationInfo);
                GamesLogic gl = new GamesLogic(objSQL);

                data = gl.getPendingApprovals();
                message = gl.errorMessage;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.AppendLog("ModeratorController >> ", LogType.Error, "GET", ex.Message);
            }
            if (message.Length == 0)
                return Ok(data);
            else
                return BadRequest(message);
        }

        public IHttpActionResult Post([FromBody]GamesMaster approveGame)
        {
            string message = "approved";
            bool flag = false;
            try
            {
                object infoObject;
                Request.Properties.TryGetValue(Constants.IdentificationInfo, out infoObject);
                DBServerIdentification identificationInfo = (DBServerIdentification)infoObject;
                DataSQL objSQL = new DataSQL(identificationInfo);
                GamesLogic gl = new GamesLogic(objSQL);
                flag = gl.approveGame(approveGame);
                message = gl.errorMessage;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.AppendLog("GamesController >> ", LogType.Error, "POST", ex.Message);
            }
            if (flag)
                return Ok(message);
            else
                return BadRequest(message);
        }
    }
}
