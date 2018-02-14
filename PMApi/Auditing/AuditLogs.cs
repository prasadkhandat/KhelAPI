using DataLibrary;
using LogLibrary;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace PMApi.Auditing
{
    /// <summary>
    /// Describe your member here.
    /// </summary>
    public class AuditLogs
    {        
        static string MongoConnectionString = System.Configuration.ConfigurationManager.AppSettings["MongoConnectionString"];
        static string DbName = "Khel";
        /// <summary>
        /// Describe your member here.
        /// </summary>
        public static async Task InsertRequestCall(string access_token, string URL, string RequestHeaders, string RequestParameters, string Response, string ResponseStatus, string ContentSize, string resHeaders, string ReqSentTime, string reqCompletedTime, int resHttpCode)
        {
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(DbName);
                var collection = database.GetCollection<CustomerAccess>("customeraccess");

                APICalls call = new APICalls();
                call.APIUrl = URL;
                call.RequestParameters = RequestParameters;
                call.ResponseContent = Response;
                call.ResponseStatus = ResponseStatus;
                call.ResponseContentSize = ContentSize;
                call.ResponseHeaders = resHeaders;
                call.RequestSentTime = ReqSentTime;
                call.RequestCompletedTime = reqCompletedTime;
                call.ResHttpCode = resHttpCode;
                call.RequestHeaders = RequestHeaders;
                collection.UpdateOne(Builders<CustomerAccess>.Filter.Eq(x => x.access_token, access_token), Builders<CustomerAccess>.Update.AddToSet(x => x.apiCalls, call));

            }
            catch (Exception ex)
            {
                FileLogger.AppendLog("POWebAPI", LogType.Error, "POWebAPI >> AuditLogs >> insertRequestCall >> " + access_token + " >> ", ex.Message);
            }
        }
        /// <summary>
        /// Describe your member here.
        /// </summary>
        public static bool insertCustomerAuthCall(string dbServerID,string name, string URL, string EmailID, string zip, string PhoneNumber, string access_token, string RemoteIP, string twoFactorEnabled, string twoFactorCode)
        {
            bool flag = true;
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(DbName);
                var collection = database.GetCollection<CustomerAccess>("customeraccess");

                CustomerAccess access = new CustomerAccess();                
                access.Name = name;
                access.access_token = access_token;                
                access.URL = URL;                
                access.EmailID = EmailID;                
                access.Zip = zip;
                access.PhoneNumber = PhoneNumber;
                access.RemoteIP = RemoteIP;
                access.TokenGenTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                access.TokenExpTime = DateTime.Now.AddHours(24).ToString("yyyy-MM-dd HH:mm:ss");
                access.DBServerID = dbServerID;
                access.twoFactorVerified = twoFactorEnabled;
                access.twoFactorCode = twoFactorCode;
                access.isValidToken = true;
                access.apiCalls = new List<APICalls>();

                collection.InsertOne(access);
            }
            catch (Exception ex)
            {
                flag = false;
                FileLogger.AppendLog("POWebAPI", LogType.Error, "POWebAPI >> AuditLogs >> insertCustomerAuthCall >> " + access_token + " >>", ex.Message);
            }
            return flag;
        }
        /// <summary>
        /// Describe your member here.
        /// </summary>
        public static DBServerIdentification getIdentificationInfo1(string access_token, string remoteIP, bool skipTwoStep = false)
        {
            DBServerIdentification obj = new DBServerIdentification() { Name = string.Empty, phone = string.Empty, Status = "Error" };
            //LicenseClass sls = new LicenseClass();
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(DbName);
                var collection = database.GetCollection<CustomerAccess>("customeraccess");
                var list = collection.Find(Builders<CustomerAccess>.Filter.Eq(x => x.access_token, access_token)).ToList();

                foreach (var tmpData in list)
                {
                    obj.ObjectID = tmpData.id;
                    obj.Name = tmpData.Name;
                    obj.phone = tmpData.PhoneNumber;
                    obj.access_token = tmpData.access_token;                    
                    obj.EmailID = tmpData.EmailID;                    
                    DateTime fromDate = Convert.ToDateTime(tmpData.TokenGenTime);
                    DateTime toDate = Convert.ToDateTime(tmpData.TokenExpTime);                    
                    if ((toDate - fromDate).TotalMinutes > 0)
                    {
                        if (tmpData.isValidToken && /*tmpData.RemoteIP.ToString().Equals(remoteIP) &&*/ AuditLogs.verifyAllowedIpAccess(tmpData.id.ToString(), remoteIP))
                        {
                            if (tmpData.twoFactorVerified.ToString().ToLower().Equals("true") || skipTwoStep)
                                obj.Status = "Success";
                            else
                                obj.Status = "Please verify two step authentication";
                        }
                        else
                            obj.Status = "Unauthorized access";
                    }
                    else
                    {
                        obj.Status = "TokenExpired";
                        tmpData.isValidToken = false;
                        collection.UpdateOne(Builders<CustomerAccess>.Filter.Eq(x => x.access_token, access_token), Builders<CustomerAccess>.Update.Set(x => x.isValidToken, false));
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.AppendLog("POWebAPI", LogType.Error, "POWebAPI >> AuditLogs >> getIdentificationInfo " + access_token + " >> ", ex.Message);
            }
            return obj;
        }
        /// <summary>
        /// Describe your member here.
        /// </summary>        
        public static bool verifyTwoStepAuthentication(string access_token, string verfication_code)
        {
            bool flag = false;
            try
            {
                var client = new MongoClient(MongoConnectionString);
                var database = client.GetDatabase(DbName);
                var collection = database.GetCollection<CustomerAccess>("customeraccess");

                List<CustomerAccess> data = collection.Find<CustomerAccess>(Builders<CustomerAccess>.Filter.Eq(x => x.access_token, access_token)).ToList();
                if (data.Count == 1)
                {
                    if (data[0].twoFactorCode.Equals(verfication_code))
                    {
                        flag = true;
                        data[0].twoFactorVerified = "True";
                        collection.ReplaceOne(Builders<CustomerAccess>.Filter.Eq(x => x.access_token, access_token), data[0]);
                    }
                }
            }
            catch (Exception ex)
            {
                FileLogger.AppendLog("POWebAPI", LogType.Error, "POWebAPI >> AuditLogs >> verifyTwoStepAuthentication >> " + access_token + " >>", ex.Message);
            }
            return flag;
        }
        /// <summary>
        /// Describe your member here.
        /// </summary>
        public static bool verifyAllowedIpAccess(string MasterCustomerID, string IPAddress)
        {
            bool flag = true;
            try
            {
                //var client = new MongoClient(MongoConnectionString);
                //var database = client.GetDatabase(DbName);
                //var collection = database.GetCollection<IPAddressMapping>("customeripmapping");
                //var ipList = collection.Find<IPAddressMapping>(Builders<IPAddressMapping>.Filter.Eq(x => x.MasterCustomerID, MasterCustomerID)).ToList();

                //if (ipList.Count > 0)
                //{
                //    if ((from x in ipList where x.IpAddress.Equals(IPAddress) select x).Count() == 0)
                //        flag = false;
                //}
            }
            catch (Exception ex)
            {
                flag = false;
                FileLogger.AppendLog(MasterCustomerID, LogType.Error, "POWebAPI >> AuditLogs >> verifyAllowedIpAccess >> ", ex.Message);
            }
            return flag;
        }
    }
    /// <summary>
    /// Describe your member here.
    /// </summary>
    

    /// <summary>
    /// Describe your member here.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class CustomerAccess
    {
        [BsonIgnoreIfDefault]
        public Object id { get; set; }
        public string access_token { get; set; }        
        public string URL { get; set; }        
        public string EmailID { get; set; }                
        public string Zip { get; set; }
        public string PhoneNumber { get; set; }
        public string RemoteIP { get; set; }
        public string TokenGenTime { get; set; }
        public string TokenExpTime { get; set; }        
        public string twoFactorVerified { get; set; }
        public string twoFactorCode { get; set; }
        public bool isValidToken { get; set; }
        public List<APICalls> apiCalls { get; set; }
        public string Name { get; set; }
        public string DBServerID { get; set; }
    }

    /// <summary>
    /// Describe your member here.
    /// </summary>
    [BsonIgnoreExtraElements]
    public class APICalls
    {
        public string APIUrl { get; set; }
        public string RequestParameters { get; set; }
        public string RequestHeaders { get; set; }
        public string ResponseContent { get; set; }
        public string ResponseStatus { get; set; }
        public string ResponseContentSize { get; set; }
        public string ResponseHeaders { get; set; }
        public string RequestSentTime { get; set; }
        public string RequestCompletedTime { get; set; }
        public int ResHttpCode { get; set; }
    }
}