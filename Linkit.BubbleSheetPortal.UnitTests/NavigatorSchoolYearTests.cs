using LinkIt.BubbleSheetPortal.Services;
using NUnit.Framework;
using System;
using System.Linq;

namespace Linkit.BubbleSheetPortal.UnitTests
{
    [TestFixture]
    public class NavigatorSchoolYearTests
    {
        [Test]
        public void Navigator_SchoolYear_TestAllCases()
        {
            NavigatorReportService navigatorReportService = new NavigatorReportService(null, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null);
            var yearList = navigatorReportService.GetYearListByStartMonth("8", new DateTime(2020, 8, 1), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));

            yearList = navigatorReportService.GetYearListByStartMonth("8", new DateTime(2020, 4, 1), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));

            yearList = navigatorReportService.GetYearListByStartMonth("4", new DateTime(2020, 5, 1), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));

            yearList = navigatorReportService.GetYearListByStartMonth("8", new DateTime(2020, 3, 1), 3).ToList();
            Assert.That(!yearList.Exists(c => c.Name == "2020-2021"));

            yearList = navigatorReportService.GetYearListByStartMonth("12", new DateTime(2020, 1, 1), 3).ToList();
            Assert.That(!yearList.Exists(c => c.Name == "2020-2021"));


            yearList = navigatorReportService.GetYearListByStartMonth("8", new DateTime(2020, 4, 1), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));
            Assert.That(yearList.Exists(c => c.Name == "2017-2018"));

            yearList = navigatorReportService.GetYearListByStartMonth("8", new DateTime(2020, 9, 10), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));
            Assert.That(!yearList.Exists(c => c.Name == "2017-2018"));

            yearList = navigatorReportService.GetYearListByStartMonth("8", new DateTime(2020, 8, 10), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));
            Assert.That(!yearList.Exists(c => c.Name == "2017-2018"));


            yearList = navigatorReportService.GetYearListByStartMonth("1", new DateTime(2020, 11, 10), 3).ToList();
            Assert.That(yearList.Exists(c => c.Name == "2021-2022"));
            Assert.That(yearList.Exists(c => c.Name == "2020-2021"));
            Assert.That(yearList.Exists(c => c.Name == "2018-2019"));

        }

    }
}
