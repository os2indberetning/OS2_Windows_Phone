using System;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    public class RemarkViewModel : XLabs.Forms.Mvvm.ViewModel
    {

        private string remark;
        public RemarkViewModel()
        {
            remark = Definitions.Report.ManualEntryRemark = remark;
            Subscribe();
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
            Unsubscribe();
            HandleBackMessage();
        }
        private void HandleBackMessage()
        {
            try
            {
                Navigation.PopAsync();
            }
            catch (Exception e)
            {
                // Catching exception from double pop
                // Dont know how to fix it in a proper way
            }
        }
        #endregion

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
    }
}
