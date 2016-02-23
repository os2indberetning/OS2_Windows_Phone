/* 
 * Copyright (c) OS2 2016.
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */

using System;
using OS2Indberetning.Model;
using Xamarin.Forms;

namespace OS2Indberetning
{
    public static class Definitions
    {
        #region User choices and drivereport

        public static UserInfoModel User;
        public static DriveReport Report = new DriveReport();

        #endregion
        
        #region Drive choices
        public static string Purpose = "Vælg Formål";
        public static string Remark = "Indtast Bemærkning";
        public static Employment Organization;
        public static Rate Rate;
        public static string DateToView;
        public static string DateToApi;
        public static bool StartAtHome = false;
        public static bool EndsAtHome = false;
        public static Route Route = new Route();
        #endregion

        #region GPS properties

        public static double Accuracy = 10;
        public static uint MinInterval = 8000;
        public static int MinIntervalTimeout = 10000;

        public static int NoGpsSignalTimer = 30;
        public static int GpsCountDown = 5;

        public static bool GpsIsActive;
        #endregion

        #region Loading message

        public static string LoadingMessage = "\nVent venligst\n";

        #endregion

        #region Text

        public static string FontFamily = "HelveticaNeue-Medium";

        public static int LoginLabelText = 25;
        public static int MainListTextSize = 23;
        public static int InformationFontSize = 21;
        public static int DriveFinishedTextSize = 20;

        #endregion

        #region File System

        public static string PurposeFileName = "purpose.txt";
        public static string PurposeFolderName = "purpose";

        public static string ReportsFileName = "reports.txt";
        public static string ReportsFolderName = "reports";

        public static string OrganizationFileName = "organization.txt";
        public static string OrganizationFolder = "organization";

        public static string TaxeFileName = "taxe.txt";
        public static string TaxeFolder = "taxe";

        #endregion

        #region Storage keys
        public static string AuthKey = "login_key";
        public static string UserDataKey = "userKey";
        public static string MunKey = "userToken";
        #endregion

        #region GpsPage
        public static int AccuracyTextSize = 25;
        public static int DrivenTextSize = 50;
        public static int LastupdateTextSize = 25;
        public static int PopupTextSize = 23;

        public static double GpsButtonHeight = 85;
        #endregion

        #region Header
        public static int HeaderFontSize = 40;
        public static int HeaderHeight = 80;
        #endregion

        #region Buttons
        public static int ButtonHeight = 70;
        public static int ButtonFontSize = 35;
        #endregion

        #region Colors
        public static string PrimaryColor = "#d3d3d3";
        public static string SecondaryColor = "#d3d3d3";
        public static string TextColor = "#000000";
        public static string BackgroundColor = "#ffffff";

        // Default - Used when logging out
        public static string DefaultPrimaryColor = "#d3d3d3";
        public static string DefaultSecondaryColor = "#d3d3d3";
        public static string DefaultTextColor = "#000000";
        public static string DefaultBackgroundColor = "#ffffff";
        #endregion

        #region Municipality icon and url

        public static UriImageSource MunIcon;
        public static string MunUrl;

        #endregion

        #region Padding and Screen width/height

        public static double Padding;


        public static double Scale;
        public static double ScreenWidth;
        public static double ScreenHeight;

        #endregion
        
        #region MainPage

        public static bool HasAppeared = false;
        public static int storedReportsCount;
        public static bool RefreshMainView = false;

        #endregion
    }
}
