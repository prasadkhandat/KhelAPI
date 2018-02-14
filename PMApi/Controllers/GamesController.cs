using BusinessLibrary;
using DataLibrary;
using LogLibrary;
using PMApi.Auditing;
using PMApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PMApi.Controllers
{
    public class GamesController : ApiController
    {
        [Authorize]
        public IHttpActionResult Post([FromBody]GamePostModel value)
        {
            string message = "New game added";
            bool flag = false;
            try
            {
                object infoObject;
                Request.Properties.TryGetValue(Constants.IdentificationInfo, out infoObject);
                DBServerIdentification identificationInfo = (DBServerIdentification)infoObject;
                DataSQL objSQL = new DataSQL(identificationInfo);
                GamesLogic gl = new GamesLogic(objSQL);
                flag = gl.addNewGame(value);
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
        
        public IHttpActionResult Get(string frm="", string itn="", string aud="",string mnp="",string mxp="")
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

                data = gl.getGames(frm, itn, aud, mnp, mxp);
                message = gl.errorMessage;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.AppendLog("GamesController >> ", LogType.Error, "GET", ex.Message);
            }
            if (message.Length == 0)
                return Ok(data);
            else
                return BadRequest(message);
        }
        
        [ApiExplorerSettings(IgnoreApi =true)]
        public IHttpActionResult Get(string gameid)
        {
            string message = "";
            bool flag = false;
            GamePostModel data = new GamePostModel();
            try
            {
                object infoObject;
                Request.Properties.TryGetValue(Constants.IdentificationInfo, out infoObject);
                DBServerIdentification identificationInfo = (DBServerIdentification)infoObject;
                DataSQL objSQL = new DataSQL(identificationInfo);
                GamesLogic gl = new GamesLogic(objSQL);

                data = gl.getGames(gameid);
                message = gl.errorMessage;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                FileLogger.AppendLog("GamesController >> ", LogType.Error, "GET", ex.Message);
            }
            if (message.Length == 0)
                return Ok(data);
            else
                return BadRequest(message);
        }

    }
}
