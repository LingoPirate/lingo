using MultiLApi.App_Start;
using MultiLApi.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace MultiLApi.Controllers
{
    public class UploadTextController : ApiController
    {
        [HttpPost]
        [Route("api/UploadText")]

        public HttpResponseMessage Post([FromBodyAttribute] ApiInput input) // id refers to Application Key
        {
            bool isTranslationUpload = false;
            HttpResponseMessage response = new HttpResponseMessage();
            Response res = new Response();
            try
            {
                if (input != null)
                {
                    MongodbConnect mongo = new MongodbConnect(ConfigurationManager.AppSettings["MongoDBName"].ToString());

                    // Validate Application key
                    var validApplication = mongo.GetRecordByApplicationKey(input.ApiAccessKey);

                    if (validApplication != null)
                    {
                        // Upload Text if not Exist
                        if (input.InputText != null)
                        {

                            isTranslationUpload = mongo.uploadText(validApplication.ApplicationID, input.InputText,mongo.GetRecordByApplicationKey(input.ApiAccessKey));
                            response = Request.CreateResponse(HttpStatusCode.OK);
                            if (isTranslationUpload)
                            {
                                res.Message = "Translation uploaded";
                            }
                            else
                            {
                                res.Message = "No Translation uploaded";
                            }
                            res.Status = "Sucess";
                            res.Result = null;
                        }
                        else
                        {
                            response = Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Translation input text is missing in request");
                            res.Status = "Failed";
                            res.Message = "Translation input text is missing in request";
                            res.Result = null;
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
            catch (Exception ex)
            {
                Console.Error.Write(ex.Message);
                response= Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Unexpected Error");
                res.Status = "Failed";
                res.Message = "Unexpected Error";
                res.Result = null;
            }

            response.Content = new StringContent(JsonConvert.SerializeObject(res, Formatting.Indented), Encoding.UTF8, "application/json");
            return response;
        }
    }
}