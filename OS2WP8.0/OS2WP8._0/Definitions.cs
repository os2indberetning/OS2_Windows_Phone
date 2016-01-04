﻿using OS2Indberetning.Model;
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
        public static Employment Organization = new Employment { EmploymentPosition = "Vælg"};
        public static Rate Taxe = new Rate{Description = "Vælg"};
        public static string Date;
        public static bool StartAtHome = false;
        public static bool EndsAtHome = false;
        public static Route Route = new Route();
        #endregion

        #region GPS properties

        public static double Accuracy = 0;
        public static uint MinInterval = 3000;

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

        #endregion

        #region Storage keys
        public static string TokenKey = "login_key";
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
        public static string DefaultTextColor = "#000000";
        public static string BackgroundColor = "#ffffff";
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

        #endregion
    }
}