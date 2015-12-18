using System;
using OS2Indberetning.Pages;
using Xamarin.Forms;

namespace OS2Indberetning.ViewModel
{
    /// <summary>
    /// Viewmodel of the Remarks page. Handles all view logic
    /// </summary>
    public class RemarkViewModel : XLabs.Forms.Mvvm.ViewModel, IDisposable
    {
        private string _remark;

        /// <summary>
        /// Constructor that handles initialization of the viewmodel
        /// </summary>
        public RemarkViewModel()
        {
            Remark = Definitions.Report.ManualEntryRemark;
            Subscribe();
        }

        /// <summary>
        /// Method that handles cleanup of the viewmodel
        /// </summary>
        public void Dispose()
        {
            Unsubscribe();
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        public void Subscribe()
        {
            MessagingCenter.Subscribe<RemarkPage>(this, "Back", (sender) => { HandleBackMessage(); });
            MessagingCenter.Subscribe<RemarkPage>(this, "Save", (sender) => { HandleSaveMessage(); });
        }

        /// <summary>
        /// Method that handles subscribing to the needed messages
        /// </summary>
        public void Unsubscribe()
        {
            MessagingCenter.Unsubscribe<RemarkPage>(this, "Back");
            MessagingCenter.Unsubscribe<RemarkPage>(this, "Save");
        }

        #region Message Handlers

        /// <summary>
        /// Method that handles the Save message
        /// </summary>
        private void HandleSaveMessage()
        {
            Definitions.Report.ManualEntryRemark = _remark;
            Dispose();
            HandleBackMessage();
        }

        /// <summary>
        /// Method that handles the Back message
        /// </summary>
        private void HandleBackMessage()
        {
            Dispose();
            Navigation.PopModalAsync();
        }

        #endregion

        #region Properties
        public const string RemarkProperty = "Remark";
        public string Remark
        {
            get
            {
                return _remark;
            }
            set
            {
                _remark = value;
            }
        }
        #endregion
    }
}
