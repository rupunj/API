using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using API.Const;
using API.Models.Responses;

namespace API.Models.Base
{
    internal class RestClient
    {
        public int StatusCode { get; set; }

        internal async Task<(bool, string)> PostAPIInvoke(string baseUrl, string endPoint, List<KeyValuePair<string, string>> paramCollection, string token, string authType = Config.AUTHORIZATION_TYPE_BEARER)
        {
            HttpClientHandler handler = new()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            };

            handler.ServerCertificateCustomValidationCallback += SSLCertValidate;

            using var httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri(baseUrl)
            };
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Config.CONTENT_TYPE_APPLICATION_URLENCODED));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType, token);
            httpClient.DefaultRequestHeaders.ConnectionClose = false;

            using var content = new FormUrlEncodedContent(paramCollection);
            content.Headers.ContentType = new MediaTypeWithQualityHeaderValue(Config.CONTENT_TYPE_APPLICATION_URLENCODED);

            HttpResponseMessage response = await httpClient.PostAsync(endPoint, content);

            StatusCode = (int)response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                return (true, response.Content.ReadAsStringAsync().Result);
            }
            else
            {
                string strResult = response.Content.ReadAsStringAsync().Result;
                return (false, strResult.Equals("") ? response.StatusCode.ToString() : strResult);
            }
        }

        internal async Task<(bool, string)> PostAPIInvoke(string baseUrl, string endPoint, string requestContent, string token, string authType = Config.AUTHORIZATION_TYPE_BEARER)
        {
            HttpClientHandler handler = new()
            {
                SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            };

            handler.ServerCertificateCustomValidationCallback += SSLCertValidate;

            using HttpClient httpClient = new(handler);

            httpClient.Timeout = TimeSpan.FromSeconds(200);
            httpClient.BaseAddress = new Uri(baseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(Config.CONTENT_TYPE_APPLICATION_JSON));
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(authType, token);
            httpClient.DefaultRequestHeaders.ConnectionClose = false;

            using (StringContent httpContent = new(requestContent, Encoding.UTF8, "application/json"))
            {
                using HttpResponseMessage response = httpClient.PostAsync(endPoint, httpContent).Result;
                string result = response.Content.ReadAsStringAsync().Result;

                this.StatusCode = (int)response.StatusCode;

                if (result != null && result.Length > 0)
                {
                    if (response.IsSuccessStatusCode)
                        return (true, result);
                    
                    else
                    {
                        try
                        {
                            if (JsonConvert.DeserializeObject<ErrorResponse>(result).ErrorID != null && JsonConvert.DeserializeObject<ErrorResponse>(result).ErrorID.Length > 0)
                                return (false, result);
                            else
                                return (false, JsonConvert.SerializeObject(new ErrorResponse() { Error = result }));
                        }
                        catch (Exception ex)
                        {
                            if (result.Length > 0)
                                return (false, JsonConvert.SerializeObject(new ErrorResponse() { Error = JsonConvert.DeserializeObject<string>(result) }));
                            else
                                throw new Exception(ex.Message + " - " + response.StatusCode.ToString());
                        }
                    }
                }

                if (!response.IsSuccessStatusCode)
                    return (false, "");
                else
                    return (true, "");
            };
        }

        private static bool SSLCertValidate(object sender, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {

            HttpRequestMessage req = (HttpRequestMessage)sender;
            if (req.RequestUri.Authority.ToString().Contains("localhost"))
                return true;

            if (certificate == null)
                throw new Exception(ErrorStatus.STATUS_MSG_CERT_NOT_FOUND);

            X509Certificate2 cert = certificate;

            if (!cert.Verify())
                throw new Exception(string.Concat(ErrorStatus.STATUS_MSG_CERT_CHAIN_FAIL));

            return true;
        }

        private class FormBodyEncodedContent : ByteArrayContent
        {
            public FormBodyEncodedContent(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
                : base(FormBodyEncodedContent.GetContentByteArray(nameValueCollection))
            {
                base.Headers.ContentType = new MediaTypeHeaderValue(Config.CONTENT_TYPE_APPLICATION_URLENCODED);
            }
            private static byte[] GetContentByteArray(IEnumerable<KeyValuePair<string, string>> nameValueCollection)
            {
                if (nameValueCollection == null)
                {
                    throw new ArgumentNullException(nameof(nameValueCollection));
                }

                StringBuilder stringBuilder = new StringBuilder();
                foreach (KeyValuePair<string, string> current in nameValueCollection)
                {
                    if (stringBuilder.Length > 0)
                    {
                        stringBuilder.Append('&');
                    }

                    stringBuilder.Append(FormBodyEncodedContent.Encode(current.Key));
                    stringBuilder.Append('=');
                    stringBuilder.Append(FormBodyEncodedContent.Encode(current.Value));
                }
                return Encoding.ASCII.GetBytes(stringBuilder.ToString());
            }
            private static string Encode(string data)
            {
                if (string.IsNullOrEmpty(data))
                {
                    return string.Empty;
                }
                return System.Net.WebUtility.UrlEncode(data).Replace("%20", "+");
            }
        }
    }
}
