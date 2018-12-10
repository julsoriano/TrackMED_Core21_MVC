using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;  // From NuGet Package: Microsoft.AspNet.WebApi.Client 5.2.3. Supplies 'ReadAsAsync' method
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TrackMED.Models;

namespace TrackMED.Services
{
    public class EntityService<T>: IEntityService<T> where T: IEntity
    {
        private readonly Settings _settings;
        private string uri = null;

        // https://docs.asp.net/en/latest/mvc/controllers/dependency-injection.html#accessing-settings-from-a-controller
        public EntityService(IOptions<Settings> optionsAccessor)
        {
            _settings = optionsAccessor.Value; // reads appsettings.json
        }

        public async Task<List<T>> GetEntitiesAsync(string id = null, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = id == null ? getServiceUri(typeof(T).Name) : getServiceUri(typeof(T).Name + "/" + id);

            // HttpClient Class: https://msdn.microsoft.com/en-us/library/system.net.http.httpclient%28v=vs.118%29.aspx?f=255&MSPPError=-2147217396
            using (HttpClient httpClient = new HttpClient())
            {
                // From http://www.asp.net/web-api/overview/advanced/calling-a-web-api-from-a-net-client
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
               
                var dataString = response.Content.ReadAsStringAsync().Result;
                List<T> objList = JsonConvert.DeserializeObject<List<T>>(dataString);

                return objList;
            }
        }

        public async Task<List<T>> GetEntitiesManyAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples" + "/" + ids);
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                List<T> objList = JsonConvert.DeserializeObject<List<T>>(dataString);

                return objList;
            }
        }

        public async Task<List<T>> GetSelectedEntitiesAsync(string tableID, string id, CancellationToken cancelToken = default(CancellationToken))
        {
            // http://www.c-sharpcorner.com/UploadFile/2b481f/pass-multiple-parameter-in-url-in-web-api/
            uri = getServiceUri(typeof(T).Name + "/" + tableID + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                httpClient.Timeout = Timeout.InfiniteTimeSpan;

                HttpResponseMessage response = await httpClient.GetAsync(uri, cancelToken);
                response.EnsureSuccessStatusCode();

                var dataString = response.Content.ReadAsStringAsync().Result;
                List<T> objList = JsonConvert.DeserializeObject<List<T>>(dataString);

                return objList;
            }
        }

        public async Task<T> GetEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<T> GetEntityAsyncByDescription(string Description, CancellationToken cancelToken = default(CancellationToken))
        {
            char[] chars = { '&', '/', '%', '$' };
            int indexSpecial = Description.IndexOfAny(chars);   // String.IndexOfAny Method https://msdn.microsoft.com/en-us/library/11w09h50(v=vs.110).aspx
            if (indexSpecial >= 0)
            {
                // http://stackoverflow.com/questions/24978885/asp-c-special-characters-cant-pass-trough-url-parameter
                Description = Uri.EscapeDataString(Description);
            }

            uri = getServiceUri(typeof(T).Name + "/Desc/" + Description);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<T> GetEntityAsyncByFieldID(string fieldID, string id, string tableID, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + fieldID + "/" + id + "/" + tableID);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);
                //return (await response.Content.ReadAsAsync<T>());

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<bool> DeleteEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(uri, cancelToken);
                return (response.IsSuccessStatusCode);
            }
        }

        public async Task<HttpResponseMessage> EditEntityAsync(T Entity, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name);
            using (HttpClient httpClient = new HttpClient())
            {
                //var response = await httpClient.PutAsJsonAsync(uri, Entity, cancelToken);

                string json = JsonConvert.SerializeObject(Entity, Formatting.Indented);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PutAsync(uri, httpContent, cancelToken);
               
                return response.EnsureSuccessStatusCode();
            }
        }

        /*
        public async Task<HttpResponseMessage> EditEntitiesAsync(List<string> ids, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples/" + ids);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, Entity, cancelToken);
                return response.EnsureSuccessStatusCode();
            }
        }
        */

        public async Task<T> PostEntityAsync( T Entity, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name);
            using (HttpClient httpClient = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(Entity, Formatting.Indented);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(uri, httpContent, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        // public async Task<HttpResponseMessage> PostEntitiesAsync(List<T> Entities, CancellationToken cancelToken = default(CancellationToken))
        public async Task<T> PostEntitiesAsync(List<T> Entities, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/multiples/" + Entities);
            using (HttpClient httpClient = new HttpClient())
            {
                string json = JsonConvert.SerializeObject(Entities, Formatting.Indented);
                HttpContent httpContent = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await httpClient.PostAsync(uri, httpContent, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }

        public async Task<T> VerifyEntityAsync(string id, CancellationToken cancelToken = default(CancellationToken))
        {
            uri = getServiceUri(typeof(T).Name + "/" + id);
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri, cancelToken);

                var dataString = response.Content.ReadAsStringAsync().Result;
                T obj = JsonConvert.DeserializeObject<T>(dataString);
                return obj;
            }
        }
        /* Comment out temporarily
        public async Task<bool> DropDatabaseAsync(CancellationToken cancelToken = default(CancellationToken))
        {
            uri = _settings.TrackMEDApi;
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync(uri, cancelToken);
                return (response.IsSuccessStatusCode);
            }
        }
        */
        public string getServiceUri(string srv)
        {
            return _settings.TrackMEDApi + "api/" + srv;
        }
    }
}