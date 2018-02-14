using DataLibrary;
using LogLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLibrary
{
    public class GamesLogic
    {
        DataSQL objSQL;
        public string errorMessage = string.Empty;
        public GamesLogic(DataSQL objSQL)
        {
            this.objSQL = objSQL;
        }

        public bool addNewGame(GamePostModel game)
        {
            bool flag = false;
            errorMessage = string.Empty;
            try
            {
                flag = objSQL.addNewGame(game);
                errorMessage = objSQL.errorMessage;
            }
            catch (Exception ex)
            {
                flag = false;
                errorMessage = ex.Message;
                FileLogger.AppendLog("GamesLogic ", LogType.Error, "addNewGame >> ", ex.Message);
            }
            return flag;
        }

        public bool approveGame(GamesMaster game)
        {
            bool flag = false;
            errorMessage = string.Empty;
            try
            {
                flag = objSQL.approveGame(game);
                errorMessage = objSQL.errorMessage;
            }
            catch (Exception ex)
            {
                flag = false;
                errorMessage = ex.Message;
                FileLogger.AppendLog("GamesLogic ", LogType.Error, "addNewGame >> ", ex.Message);
            }
            return flag;
        }

        public List<GamePostModel> getPendingApprovals()
        {
            List<GamePostModel> data = new List<GamePostModel>();
            errorMessage = string.Empty;
            try
            {
                data = objSQL.getPendingApprovals();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("GamesLogic ", LogType.Error, "getPendingApprovals >> ", ex.Message);
            }
            return data;
        }

        public List<GamePostModel> getGames(string frm = "", string itn = "", string aud = "", string mnp = "", string mxp = "")
        {
            List<GamePostModel> data = new List<GamePostModel>();
            errorMessage = string.Empty;
            try
            {
                data = objSQL.getGames(frm, itn, aud, mnp, mxp);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("GamesLogic ", LogType.Error, "getPendingApprovals >> ", ex.Message);
            }
            return data;
        }

        public GamePostModel getGames(string gameid)
        {
            GamePostModel data = new GamePostModel();
            errorMessage = string.Empty;
            try
            {
                data = objSQL.getGames(gameid);
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("GamesLogic ", LogType.Error, "getPendingApprovals >> ", ex.Message);
            }
            return data;
        }
    }
}
