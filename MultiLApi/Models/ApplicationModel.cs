using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MultiLApi.Models
{
    public class ApplicationModel
    {  
        [BsonId]
        public ObjectId ID { get; set; }
        public string ApplicationID { get; set; }
        public string UserID { get; set; }
        public String ApplicationName { get; set; }
        public string ApplicationDetails { get; set; }
        public string ApiAccessKey { get; set; }
        public DateTime CacheDate { get; set; }
    }
}