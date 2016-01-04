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
        /// <returns>ReturnUserModel</returns>
        public static async Task<ReturnUserModel> Couple(string url, string token)
        {
            var model = new ReturnUserModel();
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

                if (string.IsNullOrEmpty(jsonString))
                {
                    model.Error = new Error
                    {
                        ErrorMessage = "Netværksfejl"
                    };
                    model.User = null;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    // Deserialize string to object
                    Error error = JsonConvert.DeserializeObject<Error>(jsonString);
                    model.Error = error;
                    model.User = null;
                }
                else
                {
                    // Deserialize string to object
                    UserInfoModel user = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);
                    user = RemoveTrailer(user);
                    model.User = user;
                    model.Error = new Error(); // tom
                }
                //return model;
                return model;
            }
            catch (Exception e)
            {
                model.User = null;
                model.Error = new Error
                {
                    ErrorMessage = e.Message,
                };
                return model;
            }
        }

        /// <summary>
        /// Fetches the UserInfoModel belonging to the token.
        /// </summary>
        /// <param name="token">the token belonging to the user.</param>
        /// <param name="mun">the Municipality the user belongs to.</param>
        /// <returns>ReturnUserModel</returns>
        public static async Task<ReturnUserModel> RefreshModel(Token token, Municipality mun)
        {
            var model = new ReturnUserModel();
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
                
                // Read response
                string jsonString = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrEmpty(jsonString))
                {
                    model.Error = new Error
                    {
                        ErrorMessage = "Netværksfejl",
                        ErrorCode = "404",
                    };
                    model.User = null;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    // Deserialize string to object
                    Error error = JsonConvert.DeserializeObject<Error>(jsonString);
                    model.Error = error;
                    model.User = null;
                }
                else
                {
                    // Deserialize string to object
                    UserInfoModel user = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);
                    user = RemoveTrailer(user);
                    model.User = user;
                    model.Error = new Error(); // tom
                }

                //return model;
                return model;
                
                
            }
            catch (Exception e)
            {
                model.User = null;
                model.Error = new Error
                {
                    ErrorMessage = e.Message,
                };
                return model;
            }
        }

        /// <summary>
        /// Used to submit drivereport after finished drive.
        /// </summary>
        /// <param name="report">the report of the drive.</param>
        /// <param name="token">the token belonging to the user.</param>
        /// <param name="munUrl">the municipalicy url to be called</param>
        /// <returns>ReturnUserModel</returns>
        public static async Task<ReturnUserModel> SubmitDrive(DriveReport report, Token token, string munUrl)
        {
            var model = new ReturnUserModel();
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

                if (string.IsNullOrEmpty(jsonString))
                {
                    model.Error = new Error
                    {
                        ErrorMessage = "Netværksfejl",
                        ErrorCode = "404",
                    };
                    model.User = null;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    // Deserialize string to object
                    Error error = JsonConvert.DeserializeObject<Error>(jsonString);
                    model.Error = error;
                    model.User = null;
                }
                else
                {
                    // Deserialize string to object
                    UserInfoModel user = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);
                    user = RemoveTrailer(user);
                    model.User = user;
                    model.Error = new Error(); // tom
                }
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
        