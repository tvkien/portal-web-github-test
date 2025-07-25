﻿using System;
using System.IO;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class ResponseCapture : IDisposable 
    {
        private readonly HttpResponseBase response;
        private readonly TextWriter originalWriter;
        private StringWriter localWriter;
        public ResponseCapture(HttpResponseBase response)
        {
            this.response = response;
            originalWriter = response.Output;
            localWriter = new StringWriter();
            response.Output = localWriter;
        }
        public override string ToString()
        {
            localWriter.Flush();
            return localWriter.ToString();
        }
        public void Dispose()
        {
            if (localWriter != null)
            {
                localWriter.Dispose();
                localWriter = null;
                response.Output = originalWriter;
            }
        }
    }
}