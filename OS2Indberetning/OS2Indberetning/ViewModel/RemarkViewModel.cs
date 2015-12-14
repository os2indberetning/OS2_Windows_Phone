using System;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    public class RemarkViewModel : XLabs.Forms.Mvvm.ViewModel, IDisposable
    {

        private string remark;
        public RemarkViewModel()
        {
            Remark = Definitions.Report.ManualEntryRemark;
            Subscribe();
        }

        public void Dispose()
        {
            Unsubscribe();
        }

        public void Subscribe()
        {
            MessagingCenter.Subscribe<RemarkPage>(this, "Back", (sender) => { HandleBackMessage(); });
            MessagingCenter.Subscribe<RemarkPage>(this, "Save", (sender) => { HandleSaveMessage(); });
        }

        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<RemarkPage>(this, "Back");
            MessagingCenter.Unsubscribe<RemarkPage>(this, "Save");
        }

        #region Message Handlers
        private void HandleSaveMessage()
        {
            Definitions.Report.ManualEntryRemark = remark;
            Dispose();
            HandleBackMessage();
        }
        private void HandleBackMessage()
        {
            Dispose();
            App.Navigation.PopToRootAsync();
        }
        #endregion

        #region Properties
        public const string RemarkProperty = "Remark";
        public string Remark
        {
            get
            {
                return remark;
            }
            set
            {
                remark = value;
            }
        }
        #endregion
    }
}
