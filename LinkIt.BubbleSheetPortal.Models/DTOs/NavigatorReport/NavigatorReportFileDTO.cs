using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class NavigatorReportFileDTO
    {
        //
        // Summary:
        //     When overridden in a derived class, gets the size of an uploaded file, in bytes.
        //
        // Returns:
        //     The length of the file, in bytes.
        //
        // Exceptions:
        //   T:System.NotImplementedException:
        //     Always.
        public virtual int ContentLength { get; set; }
        //
        // Summary:
        //     When overridden in a derived class, gets the MIME content type of an uploaded
        //     file.
        //
        // Returns:
        //     The MIME content type of the file.
        //
        // Exceptions:
        //   T:System.NotImplementedException:
        //     Always.
        public virtual string ContentType { get; set; }
        //
        // Summary:
        //     When overridden in a derived class, gets the fully qualified name of the file
        //     on the client.
        //
        // Returns:
        //     The name of the file on the client, which includes the directory path.
        //
        // Exceptions:
        //   T:System.NotImplementedException:
        //     Always.
        public virtual string FileName { get; set; }
        //
        // Summary:
        //     When overridden in a derived class, gets a System.IO.Stream object that points
        //     to an uploaded file to prepare for reading the contents of the file.
        //
        // Returns:
        //     An object for reading a file.
        //
        // Exceptions:
        //   T:System.NotImplementedException:
        //     Always.
        public virtual byte[] FileBinary { get; set; }
    }
}
