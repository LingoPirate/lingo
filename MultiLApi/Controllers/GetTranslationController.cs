using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using MongoDB.Driver;
using MultiLApi.App_Start;
using MultiLApi.Models;
using Newtonsoft.Json;

namespace MultiLApi.Controllers
{
    public class GetTranslationController : ApiController
    { 
        [HttpPost]
        [Route("api/GetTranslation")]
        public HttpResponseMessage Post([FromBodyAttribute] TranslationInput input) // id refers to Application Key
        {
            LanguageTextTranslation translatedText = null;
            HttpResponseMessage response = new HttpResponseMessage();
            Response res = new Response();
            try
            {
                if (input != null)
                {
                    MongodbConnect mongo = new MongodbConnect(ConfigurationManager.AppSettings["MongoDBName"].ToString());

                    // Validate Application key
                    var validApplication = mongo.GetRecordByApplicationKey(input.id);

                    if (validApplication != null)
                    {
                        // Validate Application has language or not

                        if (input.languagecode != null && input.languagecode.Length > 0)
                        {
                            if (mongo.ValidateApplicationLanguage(input.id, input.languagecode) != null)
                            {
                                translatedText = mongo.GetTranslatedText(validApplication.ApplicationID, input.languagecode);
                            }
                            else
                            {
                                response = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Translation Language is not supported");
                                res.Status = "Failed";
                                res.Message = "Translation Language is not supported";
                                res.Result = null;
                            }
                        }
                        else
                        {
                            translatedText = mongo.GetTranslatedText(validApplication.ApplicationID, string.Empty);
                        }

                        if (translatedText != null)
                        {
                            response = Request.CreateResponse(HttpStatusCode.OK);
                            if (translatedText.Translation != null)
                            {
                                res.Status = "Success";
                                res.Message = "Translation Found";
                            }
                            else
                            {
                                res.Status = "Success";
                                res.Message = "Translation Not Found";
                            }

                            res.Result = translatedText;
                        }
                    }
                    else
                    {
                        response = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Application Key is unauthorized");
                        res.Status = "Failed";
                        res.Message = "Application Key is unauthorized";
                        res.Result = null;
                    }
                }
                else
                {
                    response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "HTTP BedRequest. Check your request body and header.");
                    res.Status = "Failed";
                    res.Message = "HTTP BedRequest. Check your request body and header.";
                    res.Result = null;
                }
                               
            }
            catch(Exception ex)
            {
                Console.Error.Write(ex.Message);
                response = Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unexpected Error");
                res.Status = "Failed";
                res.Message = "Unexpected Error";
                res.Result = null;
            }

            response.Content = new StringContent(JsonConvert.SerializeObject(res, Formatting.Indented), Encoding.UTF8, "application/json");
            return response;
        }
    }
}