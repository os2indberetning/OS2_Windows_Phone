﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.Model;

namespace OS2Indberetning.BuisnessLogic
{
    /// <summary>
    /// APICaller is responsible for handling API calling to the backend.
    /// </summary>
    public static class APICaller
    {
        private static readonly string AppInfoUrl = "https://ework.favrskov.dk/FavrskovMobilityAPI/api/AppInfo";
        private static HttpClient _httpClient;
        static APICaller()
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// Fetches all possible Municipalitys
        /// </summary>
        /// <returns>List of Municipality</returns>
        public static async Task<List<Municipality>> GetMunicipalityList()
        {
            List<Municipality> list = new List<Municipality>();
            var T = await _httpClient.GetStringAsync(AppInfoUrl);
            list = JsonConvert.DeserializeObject<List<Municipality>>(T);
            return list;
        }

        /// <summary>
        /// Couples phone and user through a token id.
        /// </summary>
        /// <param name="url">Url to post too.</param>
        /// <param name="token">token id belonging to the user</param>
        /// <returns>UserInfoModel</returns>
        public static async Task<UserInfoModel> Couple(string url, string token)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                _httpClient = new HttpClient(handler);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url + "/SyncWithToken");
                request.Content = new FormUrlEncodedContent( new[] 
                {
                    new KeyValuePair<string, string>("TokenString", token )
                });

                if (handler.SupportsTransferEncodingChunked())
                {
                    request.Headers.TransferEncodingChunked = true;
                }
                
                // Send request
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                // Read response
                string jsonString = await response.Content.ReadAsStringAsync();
                // Deserialize string to object
                UserInfoModel model = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);

                model = RemoveTrailer(model);
                //return model;
                return model;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Fetches the UserInfoModel belonging to the token.
        /// </summary>
        /// <param name="token">the token belonging to the user.</param>
        /// <param name="mun">the Municipality the user belongs to.</param>
        /// <returns>UserInfoModel if successful. returns null on failure</returns>
        public static async Task<UserInfoModel> RefreshModel(Token token, Municipality mun)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                _httpClient = new HttpClient(handler);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, mun.APIUrl + "/UserData");
                request.Content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("GUID", token.GuId)
                });

                if (handler.SupportsTransferEncodingChunked())
                {
                    request.Headers.TransferEncodingChunked = true;
                }

                // Send request
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                // NO INTERNET ERROR
                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new ArgumentException("NotFound");

                // If model deserialisation goes wrong. null is returned
                try
                {
                    // Read response
                    string jsonString = await response.Content.ReadAsStringAsync();
                    // Deserialize string to object
                    UserInfoModel model = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);

                    model = RemoveTrailer(model);
                    //return model;
                    return model;
                }
                catch
                {
                    return null;
                }
                
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Used to submit drivereport after finished drive.
        /// </summary>
        /// <param name="report">the report of the drive.</param>
        /// <param name="token">the token belonging to the user.</param>
        /// <param name="munUrl">the municipalicy url to be called</param>
        /// <returns>UserInfoModel if successful. returns null on failure</returns>
        public static async Task<UserInfoModel> SubmitDrive(DriveReport report, Token token, string munUrl)
        {
            try
            {
                var sendthis = new DriveSubmit();
                sendthis.Token = token;
                sendthis.DriveReport = report;
                var json = JsonConvert.SerializeObject(sendthis);

                HttpClientHandler handler = new HttpClientHandler();
                _httpClient = new HttpClient(handler);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, munUrl + "SubmitDrive");

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                // Send request
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                // Read response
                string jsonString = await response.Content.ReadAsStringAsync();
                // Deserialize string to object
                UserInfoModel model = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);

                model = RemoveTrailer(model);
                //return model;
                return model;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        /// <summary>
        /// Removes Anhænger rate from UserInfoModel.Rates
        /// </summary>
        /// <param name="model">the UserInfoModel which needs trailer rate removed</param>
        /// <returns>UserInfoModel</returns>
        private static UserInfoModel RemoveTrailer(UserInfoModel model)
        {
            var temp = model.Rates.FirstOrDefault(x => x.Description == "Anhænger");

            // If item was found remove it from collection.
            if (temp != null)
            {
                model.Rates.Remove(temp);
            }
            
            return model;
        }

    }
}
        