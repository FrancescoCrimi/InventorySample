using System;

using Inventory.UwpApp.ViewModels;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventory.UwpApp.Tests.MSTest
{
    // TODO: Add appropriate tests
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        // TODO: Add tests for functionality you add to ContentGridViewModel.
        [TestMethod]
        public void TestContentGridViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new ContentGridViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to DataGridViewModel.
        [TestMethod]
        public void TestDataGridViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new DataGridViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to ListDetailsViewModel.
        [TestMethod]
        public void TestListDetailsViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new ListDetailsViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to MainViewModel.
        [TestMethod]
        public void TestMainViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new MainViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to SettingsViewModel.
        [TestMethod]
        public void TestSettingsViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new SettingsViewModel();
            Assert.IsNotNull(vm);
        }

        //// TODO: Add tests for functionality you add to TabbedPivotViewModel.
        //[TestMethod]
        //public void TestTabbedPivotViewModelCreation()
        //{
        //    // This test is trivial. Add your own tests for the logic you add to the ViewModel.
        //    var vm = new TabbedPivotViewModel();
        //    Assert.IsNotNull(vm);
        //}

        //// TODO: Add tests for functionality you add to TabViewViewModel.
        //[TestMethod]
        //public void TestTabViewViewModelCreation()
        //{
        //    // This test is trivial. Add your own tests for the logic you add to the ViewModel.
        //    var vm = new TabViewViewModel();
        //    Assert.IsNotNull(vm);
        //}

        // TODO: Add tests for functionality you add to TreeViewViewModel.
        [TestMethod]
        public void TestTreeViewViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new TreeViewViewModel();
            Assert.IsNotNull(vm);
        }

        // TODO: Add tests for functionality you add to TwoPaneViewViewModel.
        [TestMethod]
        public void TestTwoPaneViewViewModelCreation()
        {
            // This test is trivial. Add your own tests for the logic you add to the ViewModel.
            var vm = new TwoPaneViewViewModel();
            Assert.IsNotNull(vm);
        }
    }
}
