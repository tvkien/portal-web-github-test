using LinkIt.BubbleSheetPortal.Common;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.UnitTest.Common
{
    [TestFixture]
    public class StringExtension_Rationalize_Tests
    {

        [Test]
        [TestCase(arguments: new object[] { "abc\"<>|:*?\\/_aaa", "abc_aaa" })]
        [TestCase(arguments: new object[] { "abc/aaa  ", "abc_aaa_" })]
        [TestCase(arguments: new object[] { "2020-2021_1182_Clark Intermediate_Benchmark Teacher Slides_ _Form A", "2020_2021_1182_Clark_Intermediate_Benchmark_Teacher_Slides_Form_A" })]
        public void FileNameContainsSpecialCaseShouldBeRationalize(string rawFileName, string expectation)
        {
            var resFileName = rawFileName.RationalizeFileName(Constanst.NAVIGATOR_RATIONALIZEFILENAME_REPLACE_BY);
            Assert.That(resFileName, Is.EqualTo(expectation));
        }

    }
}
