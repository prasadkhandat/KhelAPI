using LogLibrary;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class DataSQL
    {
        private static string MongoConnectionString = System.Configuration.ConfigurationManager.AppSettings["MongoConnectionString"];
        string dbServerID = "Khel";
        public string errorMessage = string.Empty;
        DBServerIdentification objIdentification;
        public DataSQL(DBServerIdentification objIdentification)
        {
            this.objIdentification = objIdentification;
        }
        public bool addNewGame(GamePostModel game)
        {
            bool flag = false;
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(dbServerID);
                var collection = database.GetCollection<GamePostModel>("GamesMaster");
                game.is_validated = false;
                game.submited_by_name = objIdentification.Name;
                game.submited_by_user_id = objIdentification.ObjectID.ToString();
                game.submited_date = DateTime.Now;
                collection.InsertOne(game);
                flag = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("Rx ", LogType.Error, "getRx >> ", ex.Message);
            }
            return flag;
        }
        public bool approveGame(GamesMaster game)
        {
            bool flag = false;
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(dbServerID);
                var collection = database.GetCollection<GamesMaster>("GamesMaster");
                game.is_validated = true;
                game.validated_by_name = objIdentification.Name;
                game.validated_user_id = objIdentification.ObjectID.ToString();
                game.validated_date = DateTime.Now;
                var update = Builders<GamesMaster>.Update
                             .Set(x => x.is_validated, true)
                             .Set(x => x.validated_by_name, objIdentification.Name)
                             .Set(x => x.validated_user_id, objIdentification.ObjectID.ToString())
                             .Set(x => x.validated_date, DateTime.Now);
                collection.UpdateOne(Builders<GamesMaster>.Filter.Eq(x => x.id, ObjectId.Parse(game.id.ToString())), update);

                flag = true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("Rx ", LogType.Error, "getRx >> ", ex.Message);
            }
            return flag;
        }
        public List<GamePostModel> getPendingApprovals()
        {
            List<GamePostModel> data = new List<GamePostModel>();
            errorMessage = string.Empty;
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(dbServerID);
                var collection = database.GetCollection<GamePostModel>("GamesMaster");
                data = collection.Find(Builders<GamePostModel>.Filter.Eq(x => x.is_validated, false)).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("DataSQL", LogType.Error, "getAllReports >> ", ex.Message);
            }
            return data;
        }
        public List<GamePostModel> getGames(string frm = "", string itn = "", string aud = "", string mnp = "", string mxp = "")
        {
            List<GamePostModel> data = new List<GamePostModel>();
            errorMessage = string.Empty;
            try
            {                
                List<string> filtersFrm = new List<string>();
                if (frm.Length > 0)
                {
                    filtersFrm = frm.Split(',').ToList();                    
                }
                List<string> filtersitn = new List<string>();
                if (itn.Length > 0)
                {
                    filtersitn = itn.Split(',').ToList();
                }
                List<string> filtersaud = new List<string>();
                if (aud.Length > 0)
                {
                    filtersaud = aud.Split(',').ToList();
                }
                List<string> filtersmnp = new List<string>();
                if (mnp.Length > 0)
                {
                    filtersmnp = mnp.Split(',').ToList();
                }
                List<string> filtersmxp = new List<string>();
                if (mxp.Length > 0)
                {
                    filtersmxp = mxp.Split(',').ToList();
                }

                var filter = Builders<GamePostModel>.Filter.Eq(x => x.is_validated, true)
                             & (filtersFrm.Count > 0 ? Builders<GamePostModel>.Filter.In(x => x.formation, filtersFrm) : Builders<GamePostModel>.Filter.Eq(x => x.is_validated, true))
                &(filtersitn.Count > 0 ? Builders<GamePostModel>.Filter.In(x => x.intensity, filtersitn) : Builders<GamePostModel>.Filter.Eq(x => x.is_validated, true))
                &(filtersaud.Count > 0 ? Builders<GamePostModel>.Filter.In(x => x.audiance, filtersaud) : Builders<GamePostModel>.Filter.Eq(x => x.is_validated, true))
                &(filtersmnp.Count > 0 ? Builders<GamePostModel>.Filter.In(x => x.min_participants, filtersmnp) : Builders<GamePostModel>.Filter.Eq(x => x.is_validated, true))
                & (filtersmxp.Count > 0 ? Builders<GamePostModel>.Filter.In(x => x.max_participants, filtersmxp) : Builders<GamePostModel>.Filter.Eq(x => x.is_validated, true));


                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(dbServerID);
                var collection = database.GetCollection<GamePostModel>("GamesMaster");
                data = collection.Find(filter).ToList();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("DataSQL", LogType.Error, "getAllReports >> ", ex.Message);
            }
            return data;
        }

        public GamePostModel getGames(string gameid)
        {
            GamePostModel data = new GamePostModel();
            errorMessage = string.Empty;
            try
            {


                var filter = Builders<GamePostModel>.Filter.Eq(x => x.id, ObjectId.Parse(gameid));

                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(dbServerID);
                var collection = database.GetCollection<GamePostModel>("GamesMaster");
                data = collection.Find(filter).First();
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                FileLogger.AppendLog("DataSQL", LogType.Error, "getAllReports >> ", ex.Message);
            }
            return data;
        }
    }
}
