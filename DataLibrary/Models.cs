using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{

    public class DBServerIdentification
    {
        public Object ObjectID { get; set; }
        public string access_token { get; set; }
        public string EmailID { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string phone { get; set; }
        public string User_Type { get; set; }
    }

    [BsonIgnoreExtraElements]
    public class GamesMaster
    {
        [BsonIgnoreIfDefault]
        public Object id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string formation{ get; set; }
        public string intensity { get; set; }
        public string audiance { get; set; }
        public string min_participants { get; set; }
        public string max_participants { get; set; }
        public string video { get; set; }                
        public double avg_rating { get; set; }        
        public bool is_validated { get; set; }        
        public string validated_by_name { get; set; }        
        public string validated_user_id { get; set; }
        public DateTime validated_date { get; set; }
        public string submited_by_name { get; set; }
        public string submited_by_user_id { get; set; }
        public List<comments> user_comments { get; set; }
        public DateTime submited_date { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class GamePostModel
    {
        [BsonIgnoreIfDefault]
        public Object id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public string formation { get; set; }
        public string intensity { get; set; }
        public string audiance { get; set; }
        public string min_participants { get; set; }
        public string max_participants { get; set; }
        public string video { get; set; }
        [JsonIgnore]
        public double avg_rating { get; set; }
        [JsonIgnore]
        public bool is_validated { get; set; }
        [JsonIgnore]
        public string validated_by_name { get; set; }
        [JsonIgnore]
        public string validated_user_id { get; set; }
        [JsonIgnore]
        public string submited_by_name { get; set; }
        [JsonIgnore]
        public string submited_by_user_id { get; set; }
        [JsonIgnore]
        public List<comments> user_comments { get; set; }
        public DateTime submited_date { get; set; }
    }    

    [BsonIgnoreExtraElements]
    public class comments
    {
        public string user_name { get; set; }
        public DateTime comment_dt { get; set; }
        public string comment { get; set; }
        public double rating { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserType
    {
        user,
        moderator
    }
}
