using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using tinderBot.Helpers;

namespace tinderBot
{
    public class TinderBot : IDisposable
    {
        private HttpClient _httpClient;
        private MemoryCacheHelper _cacheHelper;
        public string TinderToken;
       

        public TinderBot(string Token, IMemoryCache memoryCache)
        {

            var client = new HttpClient();
            client.BaseAddress = new Uri("https://api.gotinder.com/v2/");
                client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("x-auth-token", Token);
            _httpClient = client;

            if (memoryCache != null)
                _cacheHelper = new MemoryCacheHelper(memoryCache);
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
           // Like(personId);
             var personCache  =  _cacheHelper.Get<JObject>(personId.ToString());
            if (personCache.IsNullOrEmpty())
            {
                _cacheHelper.Set("personId", person);
                SavePersoneImage(personImage, personId);
            }
            else
            {
                SavePersoneImage(personImage, personId);
            }

          
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

        public async Task Like(string userId)
        {
           await _httpClient.GetAsync("recs/core");
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

        private Task<TResponse> Get<TResponse>(string requestUri)
        {
            return Send<TResponse>(new HttpRequestMessage(HttpMethod.Get, requestUri));
        }

        private Task<TResponse> Post<TResponse>(string requestUri)
        {
            return Send<TResponse>(new HttpRequestMessage(HttpMethod.Post, requestUri));
        }

        private Task<TResponse> Post<TRequest, TResponse>(string requestUri, TRequest payload)
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, requestUri);
            var jsonPayload = System.Text.Json.JsonSerializer.Serialize(payload);
            msg.Content = new StringContent(jsonPayload);
            return Send<TResponse>(msg);
        }

        private Task<TResponse> Delete<TResponse>(string requestUri)
        {
            return Send<TResponse>(new HttpRequestMessage(HttpMethod.Delete, requestUri));
        }

        private async Task<TResponse> Send<TResponse>(HttpRequestMessage msg)
        {
            var res = await _httpClient.SendAsync(msg);
            var json = await res.Content.ReadAsStringAsync();

            if (!res.IsSuccessStatusCode)
            {
                if (res.StatusCode == HttpStatusCode.Unauthorized)
                {
                    //throw new TinderAuthenticationException("Invalid or expired token");
                }

               // throw new TinderException(json);
            }

            return Deserialize<TResponse>(json);
        }

        private T Deserialize<T>(string json)
        {
            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<T>(json);
            }
            catch (Exception e)
            {
                throw new Exception($"Couldn't deserialize response: ${json}", e);
            }
        }

    }
}