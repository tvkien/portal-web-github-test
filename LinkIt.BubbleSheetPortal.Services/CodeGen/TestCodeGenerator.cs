using System;

namespace LinkIt.BubbleSheetPortal.Services.CodeGen
{
    public class TestCodeGenerator
    {

        private readonly ITestCodeValidator _validator;

        public TestCodeGenerator(ITestCodeValidator validator)
        {
            _validator = validator;
        }

        public string GenerateTestCode(int iTestCodeLength, string prefix)
        {
            if (prefix == null) prefix = string.Empty;
            prefix = prefix.Trim();

            var strNumber = "123456789";
            var strAlpha = "QWERTYUIOPLKJHGFDSAZXCVBNM";
            //var strAlphaAndNumber = "123456789QWERTYUIOPLKJHGFDSAZXCVBNM";
            var strOutPut = string.Empty;

            var r = new Random();
            int indexLetterRequire = r.Next(0, iTestCodeLength - 1);
            string strLetterRequire = strAlpha.Substring(r.Next(0, strAlpha.Length), 1);
            do
            {
                strOutPut = prefix;
                for (int j = 0; j < iTestCodeLength; j++)
                {
                    if (j == indexLetterRequire)
                    {
                        strOutPut += strLetterRequire;
                    }
                    else
                    {
                        int numberOrLetter = r.Next(1, 10);
                        if (numberOrLetter < 6)//TODO: number
                        {
                            strOutPut += strNumber.Substring(r.Next(0, strNumber.Length), 1);
                        }
                        else//TODO: letter
                        {
                            strOutPut += strAlpha.Substring(r.Next(0, strAlpha.Length), 1);
                        }
                    }
                }
            } while (!_validator.Validate(strOutPut));
            return strOutPut;
        }

        public string GenerateTestCodebk(int iTestCodeLength, string prefix)
        {
            if (prefix == null) prefix = string.Empty;
            prefix = prefix.Trim();

            var strNumber = "123456789";
            var strAlpha = "QWERTYUIOPLKJHGFDSAZXCVBNM";
            var strOutPut = string.Empty;
            var iAmountNumber = iTestCodeLength / 2;
            var iAmountAlpha = iTestCodeLength - iAmountNumber;
            var r = new Random();
            do
            {
                strOutPut = prefix;
                for (int i = 0; i < iAmountNumber; i++)
                {
                    strOutPut += strNumber.Substring(r.Next(0, strNumber.Length), 1);
                }
                for (int j = 0; j < iAmountAlpha; j++)
                {
                    strOutPut += strAlpha.Substring(r.Next(0, strAlpha.Length), 1);
                }
            } while (!_validator.Validate(strOutPut));
            return strOutPut;
        }
    }
}
