using DataLibrary;
using PMApi.Auditing;
using PMApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace PMApi.MessageHandlers
{
    public class APIPipeline : DelegatingHandler
    {

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            DateTime dtProcessStartTime = DateTime.Now;
            string access_token = string.Empty;
            string UserEmail = string.Empty;
            string ReqData = string.Empty;
            var response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            try
            {
                ReqData = await request.Content.ReadAsStringAsync();
            }
            catch { }
            /****************************** Authorization header check *************************************/

            if (request.Headers.Authorization != null && request.Headers.Authorization.Scheme.ToLower().Equals("bearer"))
            {
                bool Authenticated = false;
                try
                {
                    access_token = request.Headers.Authorization.Parameter;
                    DBServerIdentification objData = new DBServerIdentification();

                    objData = AuditLogs.getIdentificationInfo1(access_token, HttpContext.Current.Request.UserHostAddress, request.RequestUri.AbsolutePath.ToString().ToLower().EndsWith("/verify") ? true : false);
                    if (objData.Status == "Success")
                    {
                        Authenticated = true;
                        request.Properties.Add(Constants.IdentificationInfo, objData);
                    }
                    else
                    {
                        response = new HttpResponseMessage(HttpStatusCode.Unauthorized);
                    }

                }
                catch (Exception ex)
                {
                    response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
                }

                if (Authenticated)
                {
                    response = await base.SendAsync(request, cancellationToken);
                }
            }
            else
            /************************************************/
            {
                response = await base.SendAsync(request, cancellationToken);
            }
            if (!(request.RequestUri.LocalPath.ToString().Equals(@"/") || request.RequestUri.LocalPath.ToString().Length == 0))
            {
                DateTime dtProcessEndTime = DateTime.Now;
                long? bodylength = 0;
                long? headerlength = 0;
                if (response.Content != null)
                {
                    await response.Content.LoadIntoBufferAsync();
                    bodylength = response.Content.Headers.ContentLength;
                    headerlength = response.Headers.ToString().Length;
                }
                try
                {
                    string resContaint = string.Empty;
                    try
                    {
                        resContaint = await response.Content.ReadAsStringAsync();
                    }
                    catch { }
                    await AuditLogs.InsertRequestCall(access_token == null ? "" : access_token, request.RequestUri.AbsoluteUri == null ? "" : request.RequestUri.AbsoluteUri, request.Headers.ToString(), ReqData == null ? "" : ReqData, resContaint, response.StatusCode.ToString(), bodylength == null ? "" : bodylength.ToString(), response.Headers == null ? "" : response.Headers.ToString(), dtProcessStartTime.ToString("yyyy-MM-dd HH:mm:ss"), dtProcessEndTime.ToString("yyyy-MM-dd HH:mm:ss"), response.StatusCode.GetHashCode());
                }
                catch { }
            }
            return response;
        }
    }
}