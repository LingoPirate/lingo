using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MultiLApi.Models
{
    public class ApiInput
    {
        public string ApiAccessKey { get; set; }
        public List<InputText> InputText { get; set; }
    }

    public class InputText
    {
        public String ControlID { get; set; }
        public string OriginalText { get; set; }
    }

    public class Application
    {
        [IgnoreDataMember]
        public string ApplicationID { get; set; }
        public string ApiAccessKey { get; set; }
        public string ApplicationName { get; set; }
        public List<LanguageInfo> Languages { get; set; }
    }

    public class Language
    {
        public string languageCode { get; set; }
        public string Text { get; set; }
    }

    public class LanguageTextTranslation
    {
        public string ApiAccessKey { get; set; }
        public string ApplicationName { get; set; }
        public List<Text> Translation { get; set; }
    }

    public class Text
    {
        public String ControlID { get; set; }
        public string OriginalText { get; set; }
        public List<Language> Languages { get; set; }
    }

    public class TranslationInput
    {
        public string id { get; set; }
        public string languagecode { get; set; }
    }

    public class LanguageInfo
    {
        public String LanguageName { get; set; }
        public string LanguageCode { get; set; }
    }

    public class Response
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public object Result { get; set; }
    }
}