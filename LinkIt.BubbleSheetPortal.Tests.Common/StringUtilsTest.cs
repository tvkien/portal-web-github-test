using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LinkIt.BubbleSheetPortal.Tests.Common
{
    [TestClass()]
    public class StringUtilsTest
    {
        [TestMethod]
        public void ReplaceWeirdCharacters_OK()
        {
            string input = "Ã ";
            string expected = "&#224;";
            string result = input.ReplaceWeirdCharacters();
            Assert.AreEqual(result, expected);
        }
        
    }
}
