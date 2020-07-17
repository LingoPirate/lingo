using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MultiLApi.Models
{
    public class ApplicationText
    {
        [BsonId]
        public ObjectId _id { get; set; }
        public string ApplicationID { get; set; }
        public String ControlID { get; set; }
        public string OriginalText { get; set; }
        public List<Language> Languages { get; set; }
        public bool AutoTranslate { get; set; }
    }

}