using System;
using System.Linq;
using System.Threading.Tasks;

using Inventory.UwpApp.Core.Services;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Inventory.UwpApp.Core.Tests.MSTest
{
    // TODO: Add appropriate unit tests.
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void TestMethod1()
        {
        }

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [TestMethod]
        public async Task EnsureSampleDataServiceReturnsContentGridDataAsync()
        {
            var actual = await SampleDataService.GetContentGridDataAsync();

            Assert.AreNotEqual(0, actual.Count());
        }

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [TestMethod]
        public async Task EnsureSampleDataServiceReturnsGridDataAsync()
        {
            var actual = await SampleDataService.GetGridDataAsync();

            Assert.AreNotEqual(0, actual.Count());
        }

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [TestMethod]
        public async Task EnsureSampleDataServiceReturnsListDetailsDataAsync()
        {
            var actual = await SampleDataService.GetListDetailsDataAsync();

            Assert.AreNotEqual(0, actual.Count());
        }

        // Remove or update this once your app is using real data and not the SampleDataService.
        // This test serves only as a demonstration of testing functionality in the Core library.
        [TestMethod]
        public async Task EnsureSampleDataServiceReturnsTreeViewDataAsync()
        {
            var actual = await SampleDataService.GetTreeViewDataAsync();

            Assert.AreNotEqual(0, actual.Count());
        }
    }
}
