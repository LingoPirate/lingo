using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Configuration;
using MultiLApi.App_Start;
using MultiLApi.Models;
using Newtonsoft.Json;
using System.Text;

namespace MultiLApi.Controllers
{
    public class ApplicationKeyValidatorController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage ValidateApplicationkey(string id) // id refers to Application api access key      
        {
            HttpResponseMessage response;
            Response res = new Response();
            MongodbConnect mongo = new MongodbConnect(ConfigurationManager.AppSettings["MongoDBName"].ToString());
            Application appRecord = mongo.GetRecordByApplicationKey(id);

            // check whether the application key is valid or not
            if (appRecord == null)
            {
                response = Request.CreateErrorResponse(HttpStatusCode.Unauthorized, "Application Key is unauthorized");
                res.Status = "Failed";
                res.Message = "Application Key is unauthorized";
                res.Result = null;
            }
            else
            {
                response = Request.CreateResponse(HttpStatusCode.OK);
                res.Status = "Success";
                res.Message = "Authorized api access key";
                res.Result = appRecord;
            }
            response.Content = new StringContent(JsonConvert.SerializeObject(res, Formatting.Indented), Encoding.UTF8, "application/json");
            return response;
        }
    }
}