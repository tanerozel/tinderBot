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


        public string TinderToken;

        public TinderBot(string Token)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.gotinder.com/v2/");
                client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-auth-token", Token);
            _httpClient = client;
        }

        public async Task <string> GetTindePersone()
        {
           
            using (_httpClient)
            {
                var serializerSettings = JsonHelper.GetDefaultJsonSerializerSettings();
                var jsonSerializer = JsonSerializer.Create(serializerSettings);

                var response = await _httpClient.GetAsync("recs/core");                    
                 

                if (!response.IsSuccessStatusCode)
                {
                    return response.ToJsonString() ;
                }

                var data = await response.Content.ReadAsStringAsync();
                var records = JObject.Parse(data);

                var result = (JArray)records["data"]["results"];
                Isele(result);

                return data;

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
            var personImage = (JArray)person["user"]["photos"];
            var personId = (string)person["user"]["_id"];
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

                    using (StreamWriter writer = File.CreateText(filePath + "version.txt"))
                    {
                        writer.WriteLine("1"+ DateTime.Now);

                    }

                }
                else
                {
         

                    string text = System.IO.File.ReadAllText(filePath+"version.txt");

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

       public async Task<string> test()
        {


            //if (!Directory.Exists(filePath))
            //{
            //    Directory.CreateDirectory(filePath);

            //    using (StreamWriter writer = File.CreateText(filePath + "version.txt"))
            //    {
            //        writer.WriteLine("1-" + DateTime.Now);
            //    }

            //}
            //else
            //{
            //    string text = System.IO.File.ReadAllText(filePath + "version.txt");

            //}

            var arr = new JArray();
            string[] dirs = Directory.GetDirectories(@"c:\temp2");
   
            foreach (string dira in dirs)
            {
              //  string id = "5a60d02f39aa4bc32a05f5da";
                string filePath = dira;

                DirectoryInfo dir = new DirectoryInfo(filePath);
                FileInfo[] imageFiles = dir.GetFiles("*.jpg");

                foreach (FileInfo f in imageFiles)
                {
                    var file = new JObject();
                    file["Name"] = f.Name;
                    arr.Add(file);
                }
            }    

            return arr.ToString();


        }

        public void Dispose()
        {
            _httpClient.Dispose();
        }
    }
}