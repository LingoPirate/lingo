using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiLApi.Models
{
    public class LanguageModel
    {
        [BsonId]
        public ObjectId ID { get; set; }
        public string LanguageID { get; set; }
        public string ApplicationID { get; set; }
        public String LanguageName { get; set; }
        public string LanguageCode { get; set; }
    }
}