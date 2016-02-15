/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OS2Indberetning.Model;
using OS2WP8._0.Model;

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
        public static async Task<ReturnUserModel> Couple(string url, string username, string password)
        {
            var model = new ReturnUserModel();
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                _httpClient = new HttpClient(handler);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, url + "/auth");

                var sendthis = new LoginModel();
                sendthis.Username = username;
                sendthis.Password = password;
                var json = JsonConvert.SerializeObject(sendthis);

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
                        Message = "Netværksfejl"
                    };
                    model.User = null;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    // Deserialize string to object
                    Error error = JsonConvert.DeserializeObject<Error>(jsonString);
                    if (String.IsNullOrEmpty(error.Message))
                        error.Message = error.ErrorCode;
                    if (String.IsNullOrEmpty(error.ErrorMessage))
                        error.ErrorMessage = error.Message;
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
                    Message = e.Message,
                };
                return model;
            }
        }

        /// <summary>
        /// Fetches the UserInfoModel belonging to the token.
        /// </summary>
        /// <param name="authorization">the token belonging to the user.</param>
        /// <param name="mun">the Municipality the user belongs to.</param>
        /// <returns>ReturnUserModel</returns>
        public static async Task<ReturnUserModel> RefreshModel(Authorization authorization, Municipality mun)
        {
            var model = new ReturnUserModel();
            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                _httpClient = new HttpClient(handler);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, mun.APIUrl + "/userInfo");

                var json = JsonConvert.SerializeObject(authorization);

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;

                // Send request
                HttpResponseMessage response = await _httpClient.SendAsync(request);
                
                // Read response
                string jsonString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    // Deserialize string to object
                    UserInfoModel user = JsonConvert.DeserializeObject<UserInfoModel>(jsonString);
                    user = RemoveTrailer(user);
                    model.User = user;
                    model.Error = new Error(); // tom
                }
                else if (string.IsNullOrEmpty(jsonString))
                {
                    model.Error = new Error
                    {
                        Message = "Netværksfejl",
                        ErrorCode = "404",
                    };
                    model.User = null;
                }
                else if (!response.IsSuccessStatusCode)
                {
                    // Deserialize string to object
                    Error error = JsonConvert.DeserializeObject<Error>(jsonString);
                    if (String.IsNullOrEmpty(error.Message))
                        error.Message = error.ErrorCode;
                    if (String.IsNullOrEmpty(error.ErrorMessage))
                        error.ErrorMessage = error.Message;

                    model.Error = error;
                    model.User = null;
                }

                //return model;
                return model;
                
                
            }
            catch (Exception e)
            {
                model.User = null;
                model.Error = new Error
                {
                    Message = e.Message,
                };
                return model;
            }
        }

        /// <summary>
        /// Used to submit drivereport after finished drive.
        /// </summary>
        /// <param name="report">the report of the drive.</param>
        /// <param name="authorization">the token belonging to the user.</param>
        /// <param name="munUrl">the municipalicy url to be called</param>
        /// <returns>ReturnUserModel</returns>
        public static async Task<ReturnUserModel> SubmitDrive(DriveReport report, Authorization authorization, string munUrl)
        {
            var model = new ReturnUserModel();
            try
            {
                var sendthis = new DriveSubmit();
                sendthis.Authorization = authorization;
                sendthis.DriveReport = report;
                var json = JsonConvert.SerializeObject(sendthis);

                HttpClientHandler handler = new HttpClientHandler();
                _httpClient = new HttpClient(handler);

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, munUrl + "/report");

                var stringContent = new StringContent(json, Encoding.UTF8, "application/json");
                request.Content = stringContent;
                // Send request
                HttpResponseMessage response = await _httpClient.SendAsync(request);
               
                // Read response
                string jsonString = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model.Error = null;
                }
                else if (string.IsNullOrEmpty(jsonString))
                {
                    model.Error = new Error
                    {
                        Message = "Netværksfejl",
                        ErrorCode = "404",
                    };
                    model.User = null;
                }
                else
                {
                    // Deserialize string to object
                    Error error = JsonConvert.DeserializeObject<Error>(jsonString);
                    if (String.IsNullOrEmpty(error.Message))
                        error.Message = error.ErrorCode;
                    if (String.IsNullOrEmpty(error.ErrorMessage))
                        error.ErrorMessage = error.Message;
                    model.Error = error;
                    model.User = null;
                }

                //return model;
                return model;
            }
            catch (Exception e)
            {
                model.Error = new Error { ErrorCode = "Exception", Message = "Der skete en uhåndteret fejl. Kontakt venligst Support" };
                return model;
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
        