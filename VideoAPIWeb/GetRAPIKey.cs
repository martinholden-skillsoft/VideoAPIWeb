using System;
using System.Web;
using System.Security.Cryptography;
using System.Web.Script.Serialization;
using System.IO;
using System.Configuration;
using System.Collections;
using VideoAPIWeb.Models;

using System.Collections.Generic;
using System.Linq;

namespace VideoAPIWeb
{
    /// <summary>
    /// This handler generates the key used by the Skillsoft Video Restful API
    /// </summary>
    /// <seealso cref="System.Web.IHttpHandler" />
    public class GetRAPIKey : IHttpHandler
    {
        /// <summary>
        /// You will need to configure this handler in the Web.config file of your
        /// web and register it with IIS before being able to use it. For more information
        /// see the following link: https://go.microsoft.com/?linkid=8101007
        /// </summary>
        #region IHttpHandler Members

        public bool IsReusable
        {
            // Return false in case your Managed Handler cannot be reused for another request.
            // Usually this would be false in case you have some state information preserved per request.
            get { return true; }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler" /> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext" /> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            SkillportErrorResponse error = new SkillportErrorResponse();
            SkillportAuthResponse response = null;

            string callback = context.Request.QueryString["callback"];

            var jsonSerializer = new JavaScriptSerializer();
            var jsonString = String.Empty;

            context.Request.InputStream.Position = 0;
            using (var inputStream = new StreamReader(context.Request.InputStream))
            {
                jsonString = inputStream.ReadToEnd();
            }

            var request = jsonSerializer.Deserialize<SkillportAuthRequest>(jsonString);

            //Read the Skillport Sites from the Web.Config
            var skillportSites = ((Hashtable)ConfigurationManager.GetSection("SkillportSites")).Cast<DictionaryEntry>().ToDictionary(d => (string)d.Key, d => (string)d.Value);

            string sharedSecret = null;
            try
            {
                sharedSecret = skillportSites[request.hostname.ToLower()];
            }
            catch (KeyNotFoundException)
            {
                error.messages.Add("unknown hostname");
            }

            if (String.IsNullOrEmpty(request.timestamp))
            {
                error.messages.Add("missing parameter : timestamp");
            }

            if (String.IsNullOrEmpty(request.username))
            {
                error.messages.Add("missing parameter : username");
            }

            if (error.messages.Count == 0)
            {
                MD5 md5 = MD5.Create();

                string sharedsecret = sharedSecret;
                string hashkey = null;
                try
                {
                    hashkey = BitConverter.ToString(md5.ComputeHash(System.Text.Encoding.Default.GetBytes(request.username + "&" + request.timestamp + "&" + sharedsecret))).Replace("-", "");
                    response = new SkillportAuthResponse() { hashkey = hashkey };
                }
                catch (Exception ex)
                {
                    error.messages.Add(ex.Message);
                }
            }

            JavaScriptSerializer TheSerializer = new JavaScriptSerializer();
            string json = null;

            if (error.messages.Count != 0)
            {
                context.Response.StatusCode = 400;
                json = TheSerializer.Serialize(error);
            }
            else
            {
                json = TheSerializer.Serialize(response);
            }

            if (!string.IsNullOrEmpty(callback))
            {
                json = string.Format("{0}({1});", callback, json);
            }

            context.Response.ContentType = "text/json";
            context.Response.Write(json);

        }

        #endregion
    }
}
