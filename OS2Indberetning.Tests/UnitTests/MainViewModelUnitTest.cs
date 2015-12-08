using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using OS2Indberetning.Model;
using OS2Indberetning.ViewModel;


namespace OS2Indberetning.UnitTests
{
    [TestClass]
    public class MainViewModelUnitTest
    {
        private MainViewModel vm;
        [TestInitialize]public void Setup()
        {
            vm = new MainViewModel();
        }

        [TestMethod]
        public void HomeCheckProperty_SetTrue_AssertIsTrue()
        {
            vm.HomeCheck = true;

            Assert.AreEqual(vm.HomeCheck, true);
        }

        [TestMethod]
        public void HomeCheckProperty_SetFalse_AssertIsFalse()
        {
            vm.HomeCheck = false;

            Assert.AreEqual(vm.HomeCheck, false);
        }

        [TestMethod]
        public void HomeCheckProperty_SetFalse_AssertIsNotTrue()
        {
            vm.HomeCheck = false;
            Assert.AreNotEqual(vm.HomeCheck, true);
        }

        [TestMethod]
        public void HomeCheckProperty_SetTrue_AssertIsNotFalse()
        {
            vm.HomeCheck = true;
            Assert.AreNotEqual(vm.HomeCheck, false);
        }

        [TestMethod]
        public void DriveReportList_SetNull_AssertIsNull()
        {

            vm.DriveReportList = null;
            Assert.AreEqual(vm.DriveReportList, null);
        }

        [TestMethod]
        public void DriveReportList_SetToListWith2Items_AssertCountIs2()
        {
            var list = new ObservableCollection<DriveReportCellModel>();
            list.Add(new DriveReportCellModel());
            list.Add(new DriveReportCellModel());
            vm.DriveReportList = list;
            Assert.AreEqual(vm.DriveReportList.Count, 2);
        }
    }
}
