/*
 * Copyright (c) 2017 Aspose Pty Ltd. All Rights Reserved.
 *
 * Licensed under the MIT (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *       https://github.com/aspose-omr/Aspose.OMR-for-Cloud/blob/master/LICENSE
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Web;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using Newtonsoft.Json;
using Com.Aspose.OMR.Model;

namespace Com.Aspose.OMR
{

    public struct MimeFileInfo
    {
        public string Name;
        public string MimeType;
        public byte[] file;
    }

    public class ApiInvoker
    {
        private static readonly ApiInvoker _instance = new ApiInvoker();
        private Dictionary<String, String> defaultHeaderMap = new Dictionary<String, String>();
        private string appSid;
        private string apiKey;

        public string AppSid
        {
            set { this.appSid = value; }
            get { return this.appSid; }
        }

        public string ApiKey
        {
            set { this.apiKey = value; }
            get { return this.apiKey; }
        }

        public static ApiInvoker GetInstance()
        {
            return _instance;
        }

        public void addDefaultHeader(string key, string value)
        {
            if (!defaultHeaderMap.ContainsKey(key))
            {
                defaultHeaderMap.Add(key, value);
            }
        }

        public string escapeString(string str)
        {
            return str;
        }

        public static object deserialize(string json, Type type)
        {
            try
            {
                if (json.StartsWith("{") || json.StartsWith("["))
                    return JsonConvert.DeserializeObject(json, type);
                else
                {
                    System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                    xmlDoc.LoadXml(json);
                    return JsonConvert.SerializeXmlNode(xmlDoc);
                }

            }
            catch (IOException e)
            {
                throw new ApiException(500, e.Message);
            }
            catch (JsonSerializationException jse)
            {
                throw new ApiException(500, jse.Message);
            }
            catch (System.Xml.XmlException xmle)
            {
                throw new ApiException(500, xmle.Message);
            }
        }

        public static object deserialize(byte[] BinaryData, Type type)
        {
            try
            {
                return new ResponseMessage(BinaryData, 200, "Ok");
            }
            catch (IOException e)
            {
                throw new ApiException(500, e.Message);
            }

        }

        private static string Sign(string url, string appKey)
        {
            UriBuilder uriBuilder = new UriBuilder(url);

            // Remove final slash here as it can be added automatically.
            uriBuilder.Path = uriBuilder.Path.TrimEnd('/');

            // Compute the hash.
            byte[] privateKey = Encoding.UTF8.GetBytes(appKey);
            HMACSHA1 algorithm = new HMACSHA1(privateKey);

            byte[] sequence = ASCIIEncoding.ASCII.GetBytes(uriBuilder.Uri.AbsoluteUri);
            byte[] hash = algorithm.ComputeHash(sequence);
            string signature = Convert.ToBase64String(hash);

            // Remove invalid symbols.
            signature = signature.TrimEnd('=');
            signature = HttpUtility.UrlEncode(signature);

            // Convert codes to upper case as they can be updated automatically.
            signature = Regex.Replace(signature, "%[0-9a-f]{2}", Evaluator);
            //signature = Regex.Replace(signature, "%[0-9a-f]{2}", e => e.Value.ToUpper());

            // Add the signature to query string.
            return string.Format("{0}&signature={1}", uriBuilder.Uri.AbsoluteUri, signature);
        }

        private static string Evaluator(Match match)
        {
            return match.Value.ToUpper();
        }

        public static string serialize(object obj)
        {
            try
            {
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;

                return obj != null
                    ? JsonConvert.SerializeObject(obj, Formatting.Indented, settings)
                    : null;
            }
            catch (Exception e)
            {
                throw new ApiException(500, e.Message);
            }
        }

        public string invokeAPI(string host, string path, string method, Dictionary<String, String> queryParams,
            object body, Dictionary<String, String> headerParams, Dictionary<String, object> formParams)
        {
            return invokeAPIInternal(host, path, method, false, queryParams, body, headerParams, formParams) as string;
        }

        public byte[] invokeBinaryAPI(string host, string path, string method, Dictionary<String, String> queryParams,
            object body, Dictionary<String, String> headerParams, Dictionary<String, object> formParams)
        {
            return invokeAPIInternal(host, path, method, true, queryParams, body, headerParams, formParams) as byte[];
        }

        public static void CopyTo(Stream source, Stream destination, int bufferSize)
        {
            byte[] array = new byte[bufferSize];
            int count;
            while ((count = source.Read(array, 0, array.Length)) != 0)
            {
                destination.Write(array, 0, count);
            }
        }

        private object invokeAPIInternal(string host, string path, string method, bool binaryResponse,
            Dictionary<String, String> queryParams, object body, Dictionary<String, String> headerParams,
            Dictionary<String, object> formParams)
        {

            path = path.Replace("{appSid}", this.AppSid);

            path = Regex.Replace(path, @"{.+?}", "");


            host = host.EndsWith("/") ? host.Substring(0, host.Length - 1) : host;

            path = Sign(host + path, this.ApiKey);

            WebRequest client = WebRequest.Create(path);
            client.Method = method;

            byte[] formData = null;
            if (formParams.Count > 0)
            {
                if (formParams.Count > 1)
                {
                    string formDataBoundary = String.Format("Somthing");
                    client.ContentType = "multipart/form-data; boundary=" + formDataBoundary;
                    formData = GetMultipartFormData(formParams, formDataBoundary);
                }
                else
                {
                    client.ContentType = "multipart/form-data";
                    formData = GetMultipartFormData(formParams, "");

                }
                client.ContentLength = formData.Length;

            }
            else
            {
                client.ContentType = "application/json";
            }

            foreach (KeyValuePair<string, string> headerParamsItem in headerParams)
            {
                client.Headers.Add(headerParamsItem.Key, headerParamsItem.Value);
            }
            foreach (KeyValuePair<string, string> defaultHeaderMapItem in defaultHeaderMap)
            {
                if (!headerParams.ContainsKey(defaultHeaderMapItem.Key))
                {
                    client.Headers.Add(defaultHeaderMapItem.Key, defaultHeaderMapItem.Value);
                }
            }

            switch (method)
            {
                case "GET":
                    break;
                case "POST":
                case "PUT":
                case "DELETE":
                    using (Stream requestStream = client.GetRequestStream())
                    {
                        if (formData != null)
                        {
                            requestStream.Write(formData, 0, formData.Length);
                        }
                        if (body != null)
                        {
                            StreamWriter swRequestWriter = new StreamWriter(requestStream);
                            swRequestWriter.Write(serialize(body));
                            swRequestWriter.Close();
                        }
                    }
                    break;
                default:
                    throw new ApiException(500, "unknown method type " + method);
            }

            try
            {
                HttpWebResponse webResponse = (HttpWebResponse) client.GetResponse();
                if (webResponse.StatusCode != HttpStatusCode.OK)
                {
                    webResponse.Close();
                    throw new ApiException((int) webResponse.StatusCode, webResponse.StatusDescription);
                }

                if (binaryResponse)
                {
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        CopyTo(webResponse.GetResponseStream(), memoryStream, 81920);
                        return memoryStream.ToArray();
                    }
                }
                else
                {
                    using (StreamReader responseReader = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string responseData = responseReader.ReadToEnd();
                        return responseData;
                    }
                }
            }
            catch (WebException ex)
            {
                HttpWebResponse response = ex.Response as HttpWebResponse;
                int statusCode = 0;
                if (response != null)
                {
                    statusCode = (int) response.StatusCode;
                    response.Close();
                }
                throw new ApiException(statusCode, ex.Message);
            }
        }

        private static byte[] GetMultipartFormData(Dictionary<string, object> postParameters, string boundary)
        {
            Stream formDataStream = new System.IO.MemoryStream();
            bool needsCLRF = false;

            if (postParameters.Count > 1)
            {

                foreach (KeyValuePair<string, object> param in postParameters)
                {
                    // Thanks to feedback from commenters, add a CRLF to allow multiple parameters to be added.
                    // Skip it on the first parameter, add it to subsequent parameters.
                    if (needsCLRF)
                        formDataStream.Write(Encoding.UTF8.GetBytes("\r\n"), 0, Encoding.UTF8.GetByteCount("\r\n"));

                    needsCLRF = true;
                    MimeFileInfo fileInfo = (MimeFileInfo) param.Value;
                    if (param.Value is MimeFileInfo)
                    {

                        string postData = string.Format(
                            "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n",
                            boundary,
                            param.Key,
                            fileInfo.MimeType);
                        formDataStream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));

                        // Write the file data directly to the Stream, rather than serializing it to a string.
                        formDataStream.Write((fileInfo.file as byte[]), 0, (fileInfo.file as byte[]).Length);
                    }
                    else
                    {
                        string postData = string.Format(
                            "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}",
                            boundary,
                            param.Key,
                            fileInfo.file);
                        formDataStream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
                    }
                }
                // Add the end of the request.  Start with a newline
                string footer = "\r\n--" + boundary + "--\r\n";
                formDataStream.Write(Encoding.UTF8.GetBytes(footer), 0, Encoding.UTF8.GetByteCount(footer));
            }
            else
            {
                foreach (KeyValuePair<string, object> param in postParameters)
                {
                    MimeFileInfo fileInfo = (MimeFileInfo) param.Value;
                    if (param.Value is MimeFileInfo)
                    {
                        // Write the file data directly to the Stream, rather than serializing it to a string.
                        formDataStream.Write((fileInfo.file as byte[]), 0, (fileInfo.file as byte[]).Length);
                    }
                    else
                    {
                        string postData = (string) param.Value;
                        formDataStream.Write(Encoding.UTF8.GetBytes(postData), 0, Encoding.UTF8.GetByteCount(postData));
                    }
                }
            }

            // Dump the Stream into a byte[]
            formDataStream.Position = 0;
            byte[] formData = new byte[formDataStream.Length];
            formDataStream.Read(formData, 0, formData.Length);
            formDataStream.Close();

            return formData;
        }

        /**
         * Overloaded method for returning the path value
         * For a string value an empty value is returned if the value is null
         * @param value
         * @return
         */
        public String ToPathValue(String value)
        {
            return (value == null) ? "" : value;
        }

        public String ToPathValue(int value)
        {
            return value.ToString();
        }

        public String ToPathValue(int? value)
        {
            return value.ToString();
        }

        public String ToPathValue(float value)
        {
            return value.ToString();
        }

        public String ToPathValue(float? value)
        {
            return value.ToString();
        }

        public String ToPathValue(long value)
        {
            return value.ToString();
        }

        public String ToPathValue(long? value)
        {
            return value.ToString();
        }

        public String ToPathValue(bool value)
        {
            return value.ToString();
        }

        public String ToPathValue(bool? value)
        {
            return value.ToString();
        }

        public String ToPathValue(double value)
        {
            return value.ToString();
        }

        public String ToPathValue(double? value)
        {
            return value.ToString();
        }
    }
}