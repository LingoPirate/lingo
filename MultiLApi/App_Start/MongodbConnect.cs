using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Driver;
using System.Configuration;
using MultiLApi.Models;
using MongoDB.Bson;

namespace MultiLApi.App_Start
{
    public class MongodbConnect
    {
        private IMongoDatabase db;
        public MongodbConnect(string databaseName)
        {
            var client = new MongoClient(string.Format(ConfigurationManager.AppSettings["MongoDBConnection"], databaseName));
            db = client.GetDatabase(databaseName);
        }

        public Application GetRecordByApplicationKey(string id)
        {
            var appCollection = db.GetCollection<ApplicationModel>("ApplicationSetting");
            var langCollection = db.GetCollection<LanguageModel>("LanguageSetting");

            Application appRecords = (from t1 in appCollection.AsQueryable()
                                      where t1.ApiAccessKey == id
                                      select new Application()
                                      {
                                          ApiAccessKey = t1.ApiAccessKey,
                                          ApplicationName = t1.ApplicationName,
                                          ApplicationID = t1.ApplicationID
                                      }).FirstOrDefault();
            if (appRecords != null)
            {
                List<LanguageInfo> langRecords = (from t1 in appCollection.AsQueryable()
                                                  join t2 in langCollection.AsQueryable() on t1.ApplicationID equals t2.ApplicationID
                                                  where t1.ApiAccessKey == id
                                                  select new LanguageInfo()
                                                  {
                                                      LanguageName = t2.LanguageName,
                                                      LanguageCode = t2.LanguageCode
                                                  }).ToList<LanguageInfo>();

                if (langRecords.Count > 0)
                {
                    appRecords.Languages = langRecords;
                }
            }

            return appRecords;
        }

        public LanguageModel ValidateApplicationLanguage(string id, string languageCode)
        {
            var fistJoin = db.GetCollection<ApplicationModel>("ApplicationSetting");
            var secondJoin = db.GetCollection<LanguageModel>("LanguageSetting");

            var lanRecords = (from t1 in fistJoin.AsQueryable()
                              join t2 in secondJoin.AsQueryable() on t1.ApplicationID equals t2.ApplicationID
                              where t2.LanguageCode == languageCode && t1.ApiAccessKey == id
                              select t2).FirstOrDefault();

            return lanRecords;
        }

        public LanguageTextTranslation GetTranslatedText(string id, string languageCode)
        {

            var AppText = db.GetCollection<ApplicationText>("ApplicationText");
            var appmodel = db.GetCollection<ApplicationModel>("ApplicationSetting");

            LanguageTextTranslation appRecords = (from t1 in appmodel.AsQueryable()
                                                  where t1.ApplicationID == id
                                                  select new LanguageTextTranslation()
                                                  {
                                                      ApiAccessKey = t1.ApiAccessKey,
                                                      ApplicationName = t1.ApplicationName
                                                  }).FirstOrDefault();

            List<ApplicationText> textRecords = (from t1 in AppText.AsQueryable()
                                                 where t1.ApplicationID == id
                                                 select t1).ToList();

            if (textRecords.Count > 0)
            {
                Text text = null;
                appRecords.Translation = new List<Text>();
                foreach (ApplicationText lang in textRecords)
                {
                    text = new Text();
                    text.ControlID = lang.ControlID;
                    text.OriginalText = lang.OriginalText;
                    text.Languages = new List<Language>();
                    foreach (Language lng in lang.Languages)
                    {
                        if (languageCode.Trim().Length > 0)
                        {
                            if (lng.languageCode == languageCode)
                            {
                                text.Languages.Add(lng);
                                break;
                            }
                        }
                        else
                        {
                            text.Languages.Add(lng);
                        }
                    }
                    appRecords.Translation.Add(text);
                }
            }

            return appRecords;
        }

        public bool uploadText(string ApplicationID, List<InputText> text, Application appLanguage)
        {
            var AppText = db.GetCollection<ApplicationText>("ApplicationText");
            ApplicationText apptext, updateAppText;
            List<Language> lng;
            List<ApplicationText> InsertAppList = new List<ApplicationText>();
            List<ApplicationText> UpdateAppList = new List<ApplicationText>();
            bool isUpload = false;

            foreach (InputText traslationText in text)
            {
                if (traslationText.ControlID.Length > 0 & traslationText.OriginalText.Length > 0)
                {
                    ApplicationText textRecords = (from t1 in AppText.AsQueryable()
                                                   where t1.ApplicationID == ApplicationID && t1.ControlID == traslationText.ControlID
                                                   select t1).FirstOrDefault();
                    if (textRecords == null)
                    {
                        apptext = new ApplicationText();
                        apptext.ApplicationID = ApplicationID;
                        apptext.ControlID = traslationText.ControlID;
                        //apptext._id = ObjectId.GenerateNewId();
                        apptext.OriginalText = HttpUtility.UrlDecode(traslationText.OriginalText);
                        lng = new List<Language>();
                        foreach(LanguageInfo langinfo in  appLanguage.Languages)
                        {
                            lng.Add(new Language() { languageCode = langinfo.LanguageCode });
                        }
                        apptext.Languages = lng;
                        apptext.AutoTranslate = true;

                        InsertAppList.Add(apptext);
                    }
                    else
                    {
                        if (textRecords.OriginalText != HttpUtility.UrlDecode(traslationText.OriginalText))
                        {
                            updateAppText = textRecords;
                            updateAppText.OriginalText = HttpUtility.UrlDecode(traslationText.OriginalText);
                            updateAppText.AutoTranslate = true;
                            UpdateAppList.Add(updateAppText);
                        }
                    }
                }

            }

            if (InsertAppList.Count > 0)
            {
                //Insert Translation Text.
                AppText.InsertMany(InsertAppList);
                isUpload = true;
            }

            if (UpdateAppList.Count > 0)
            {
                //Update Translation Text.
                foreach (ApplicationText updateitem in UpdateAppList)
                {
                    AppText.ReplaceOne(new BsonDocument("_id", updateitem._id), updateitem);
                }
                isUpload = true;
            }

            if (isUpload)
            {
                return true;
            }
            return false;
        }
    }
}