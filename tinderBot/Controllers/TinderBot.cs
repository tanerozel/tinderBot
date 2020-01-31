using Hangfire;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Azure.ServiceBus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using tinderBot.Helpers;

namespace tinderBot
{
    public class TinderBot : IDisposable
    {
        private HttpClient _httpClient;
        
        public TinderBot()
        {
        }

        public TinderBot(string apiBaseUrl, string tinderToken)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));         
            client.DefaultRequestHeaders.Add("x-auth-token", tinderToken.ToString());     
            _httpClient = client;
        }

        public async Task <JObject> GetTindePersone()
        {
           
            using (_httpClient)
            {
                var serializerSettings = JsonHelper.GetDefaultJsonSerializerSettings();
                var jsonSerializer = JsonSerializer.Create(serializerSettings);

                var response = await _httpClient.GetAsync("/recs/core");                    
                 

                if (!response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == HttpStatusCode.Unauthorized)
                        throw new Exception("Login Olunamadı");

                    var errorData = await response.Content.ReadAsStringAsync();

                   // throw new Exception("RecordFind method gets '" + response.StatusCode + "' error.\nTenantId: " +  + "\nRequest: " + postBody + "\nResponse: " + errorData);
                }

                var data = await response.Content.ReadAsStringAsync();
                var records = JObject.Parse(data);

                var result = (JArray)records["results"];
                Isele(result);

                return (JObject)records;

            }
          
        }

        public void Isele(JArray records)
        {
            foreach (JObject person in records)
            {

                PersoneProcces(person);

            }

        }

        public void PersoneProcces(JObject person)
        {
            var personImage = (JArray)person["photos"];
            var personId = (string)person["_id"];
            SavePersoneImage(personImage, personId);
        }
        public static void SavePersoneImage(JArray images,string id)
        {
            foreach (JObject image in images)
            {
                string filePath = @"c:\temp\" + id+ @"\" ;

                if (!Directory.Exists(filePath))
                {
                    Directory.CreateDirectory(filePath);
                }
            

                DownLoadFileAndSave((string)image["url"], filePath + image["id"] + ".jpg");                
            }
        }

        public static void DownLoadFileAndSave(string Address, string FileName)
        {
            WebClient client = new WebClient();
            Uri uri = new Uri(Address);           
            client.DownloadFileAsync(uri, FileName);
           
        }
        public void Dispose()
        {

        }
    }
}