/****************************************************************************************************
	Copyright (C) 2010 RapidWebDev Organization (http://rapidwebdev.org)
	Author: Tim, Legal Name: Long Yi, Email: tim.long.yi@RapidWebDev.org

	The GNU Library General Public License (LGPL) used in RapidWebDev is 
	intended to guarantee your freedom to share and change free software - to 
	make sure the software is free for all its users.

	This program is distributed in the hope that it will be useful,
	but WITHOUT ANY WARRANTY; without even the implied warranty of
	MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
	GNU Library General Public License (LGPL) for more details.

	You should have received a copy of the GNU Library General Public License (LGPL)
	along with this program.  
	If not, see http://www.rapidwebdev.org/Content/ByUniqueKey/OpenSourceLicense
 ****************************************************************************************************/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Configuration;

namespace RapidWebDev.RestfulServices.Test
{
    /// <summary>
    /// Util Class help test case
    /// </summary>
    public class TestServicesHelper
    {
        public const string userName = "admin";
        public const string password = "password1";

		/// <summary>
		/// Base Uri for all testing restful web services.
		/// </summary>
		public static string ServiceBaseUri
		{
			get
			{
				return ConfigurationManager.AppSettings["ServiceBaseUri"] ?? "http://localhost:50682/Services";
			}
		}

        /// <summary>
        /// Generate Json string
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static string GenerateJsonByType(object item)
        {
            try
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(item.GetType());

                using (MemoryStream ms = new MemoryStream())
                {

                    serializer.WriteObject(ms, item);

                    StringBuilder sb = new StringBuilder();

                    sb.Append(Encoding.UTF8.GetString(ms.ToArray()));

                    return sb.ToString();

                }
            }
            catch (Exception exp)
            {
                throw exp;
            }

        }
        /// <summary>
        /// Generate Object by Json string
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="content"></param>
        /// <returns></returns>
        public static T GenerateObjectByJson<T>(string content)
        {
            try
            {
                DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));

                MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(content));

                T jsonObject = (T)ser.ReadObject(ms);

                ms.Close();

                return jsonObject;
            }
            catch (Exception exp)
            {
                throw exp;
            }

        }

        /// <summary>
        /// Get the response from the web request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static string GetResponse<T>(string serviceUrl, string userName, string password, Func<HttpWebRequest, T> callback)
        {
            HttpWebRequest request = HttpWebRequest.Create(ServiceBaseUri + serviceUrl) as HttpWebRequest;
            request.Timeout = Int32.MaxValue;
            string credential = String.Format("{0}:{1}", userName, password);
            byte[] bytes = Encoding.UTF8.GetBytes(credential);
            string base64 = Convert.ToBase64String(bytes);
            request.Headers.Add("Authorization", "basic " + base64);
            request.PreAuthenticate = true;
            if (callback != null)
            {
                callback(request);
            }
            using (Stream responseStream = request.GetResponse().GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }
        /// <summary>
        ///  Get the response from the web request
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serviceUrl"></param>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <param name="callback"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static string GetResponse<T>(string serviceUrl, string userName, string password, Func<HttpWebRequest, string,IDictionary<string,string> ,T> callback,string content,IDictionary<string,string> parameters)
        {
            HttpWebRequest request = HttpWebRequest.Create(ServiceBaseUri + serviceUrl) as HttpWebRequest;
            request.Timeout = Int32.MaxValue;
            string credential = String.Format("{0}:{1}", userName, password);
            byte[] bytes = Encoding.UTF8.GetBytes(credential);
            string base64 = Convert.ToBase64String(bytes);
            request.Headers.Add("Authorization", "basic " + base64);
            request.PreAuthenticate = true;
            if (callback != null)
            {
                callback(request, content,parameters);
            }

            using (Stream responseStream = request.GetResponse().GetResponseStream())
            {
                using (StreamReader streamReader = new StreamReader(responseStream))
                {
                    return streamReader.ReadToEnd();
                }
            }
        }

        /// <summary>
        /// Set the request 
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public static HttpWebRequest GetDataByJson(HttpWebRequest request)
        {
            request.ContentType = "application/json";
            request.Method = "GET";
            return request;
        }

        public static HttpWebRequest PostDataByJson(HttpWebRequest request)
        {
            request.ContentType = "application/json";
            request.Method = "POST";
            return request;
        }

        /// <summary>
        /// Attach the post data
        /// </summary>
        /// <param name="request"></param>
        /// <param name="_object"></param>
        /// <returns></returns>
        public static HttpWebRequest PostDataByJsonWithContent(HttpWebRequest request, string _object,IDictionary<string,string> parameters)
        {
            request.ContentType = "application/json";
            request.Method = "POST";
            if (parameters != null)
            {
                if (parameters.Count > 0)
                {
                    foreach (KeyValuePair<string, string> temp in parameters)
                    {
                        request.Headers.Add(temp.Key, temp.Value);
                    }
                }
            }
            string content = _object;

            byte[] dataArray = System.Text.Encoding.Default.GetBytes(content);

            Stream stream = request.GetRequestStream();
            stream.Write(dataArray, 0, dataArray.Length);
            stream.Close();


            return request;
        }


    }
}
