using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using System.IO;
//using System.Configuration;
using System.Net;
//using System.Xml.XPath;
//using CryptoRankingConsoleApp;
//using HtmlAgilityPack;
//using Microsoft.Extensions.Configuration;

namespace Rest
{
    public class RestClient
    {
       
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
           
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public string GetFromRestService(string url)
        {
            string auth = "testy:mcTestFace";

            var pwd = Base64Encode(auth);

            try
            {
                var apiUrl = url;
                WebRequest request = WebRequest.Create(apiUrl);
                // Set the Method property of the request to POST.
                request.Method = "GET";
                
                request.Headers.Add("Accepts", "application/json");
                request.Headers.Add("Authorization", "Basic " + pwd);

                request.ContentType = "application/json";
                // Get the response.
                WebResponse response = request.GetResponse();
                // Get the stream containing content returned by the server.
                var dataStream = response.GetResponseStream();
                // Open the stream using a StreamReader for easy access.
                StreamReader reader = new StreamReader(dataStream);
                // Read the content.
                string responseFromServer = reader.ReadToEnd();
                // Display the content.
                Console.WriteLine(responseFromServer);
                // Clean up the streams.
                reader.Close();
                response.Close();

                return responseFromServer;
            }
            catch (Exception ex)
            {
                //logger
                Console.WriteLine(ex.ToString());
            }
            return "";
        }
    }
}
