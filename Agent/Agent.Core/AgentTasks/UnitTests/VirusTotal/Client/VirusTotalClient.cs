using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace Zidium.Agent.AgentTasks.UnitTests.VirusTotal.Client
{
    public class VirusTotalClient : IVirusTotalClient
    {
        private string rootUrl = "https://www.virustotal.com";
        private string lastJson = null;

        private string GetUrl(string path)
        {
            return rootUrl + path;
        }

        private TResponse ParseJson<TResponse>(string json)
        {
            lastJson = json;
            if (string.IsNullOrEmpty(json))
            {
                throw new Exception("Empty json");
            }
            return JsonConvert.DeserializeObject<TResponse>(json);
        }

        private void ValidateResponse(HttpWebResponse response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                string message = response.Headers.Get("X-Api-Message");
                if (message==null)
                {
                    message = "Неизвестная ошибка";
                }
                throw new VirusTotalResponseException(response.StatusCode, message);
            }
        }

        private TResponse ExecutePost<TResponse>(string path, NameValueCollection parameters)
            where TResponse:class
        {

            // request
            var url = GetUrl(path);
            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            string postBodyText = parameters.ToString();
            byte[] postData = Encoding.UTF8.GetBytes(postBodyText);
            request.ContentLength = postData.Length;
            using (var dataStream = request.GetRequestStream())
            {
                dataStream.Write(postData, 0, postData.Length);
                dataStream.Close();
            }

            // response
            var response = (HttpWebResponse)request.GetResponse();
            ValidateResponse(response);
            var json = string.Empty;
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                json = streamReader.ReadToEnd();
            }
            return ParseJson<TResponse>(json);
        }

        private TResponse ExecuteGet<TResponse>(string path, NameValueCollection parameters)
            where TResponse : class
        {

            // request
            var url = GetUrl(path);
            var builder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(builder.Query);
            foreach (string key in parameters.Keys)
            {
                string value = parameters[key];
                query[key] = value;
            }
            builder.Query = query.ToString();
            url = builder.ToString();
            var request = (HttpWebRequest)WebRequest.Create(url);

            // response
            var response = (HttpWebResponse)request.GetResponse();
            ValidateResponse(response);
            var json = string.Empty;
            var responseStream = response.GetResponseStream();
            if (responseStream != null)
            {
                var streamReader = new StreamReader(responseStream, Encoding.UTF8);
                json = streamReader.ReadToEnd();
            }
            return ParseJson<TResponse>(json);
        }

        public ScanResponse Scan(ScanRequest request)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString("");
            parameters.Add("apikey", request.Apikey);
            parameters.Add("url", request.Url);
            return ExecutePost<ScanResponse>("/vtapi/v2/url/scan", parameters);
        }

        public ReportResponse Report(ReportRequest request)
        {
            NameValueCollection parameters = HttpUtility.ParseQueryString("");
            parameters.Add("apikey", request.Apikey);
            parameters.Add("resource", request.Resource);
            parameters.Add("scan_id", request.ScanId);
            return ExecuteGet<ReportResponse>("/vtapi/v2/url/report", parameters);
        }
    }
}
