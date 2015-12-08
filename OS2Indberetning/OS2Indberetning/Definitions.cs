using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS2Indberetning.Model;
using Xamarin.Forms;

namespace OS2Indberetning
{
    public static class Definitions
    {
        public static UserInfoModel User;
        public static DriveReport Report = new DriveReport();
        public static Token Token;

        public static bool HasAppeared = false;

        #region drive choices
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
        public static double Accuracy = 10.00;
        public static uint MinInterval = 2000;
        #endregion

        public static string LoadingMessage = "\nVent venligst\n";
        
        public static string FontFamily = "HelveticaNeue-Medium";

        public static int LoginLabelText = 25;
        public static int MainListTextSize = 23;
        public static int InformationFontSize = 21;
        public static int DriveFinishedTextSize = 20;

        public static string PurposeFileName = "purpose.txt";
        public static string PurposeFolderName = "purpose";

        public static string ReportsFileName = "reports.txt";
        public static string ReportsFolderName = "reports";

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

        public static UriImageSource MunIcon;
        public static string MunUrl;

        public static int Padding = 15;

    }
}
