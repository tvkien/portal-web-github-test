using Envoc.Core.Shared.Data;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            //NavigatorReportService navigatorReportService = new NavigatorReportService(new FSchoolRepos());
            //string filePath = @"D:\Devblock\linkitportal-vina\TestConsole\bin\Debug\4th Grade.pdf";


            ////using (FileStream fs = new FileStream(filePath, FileMode.Open))
            ////{
            ////    var res = navigatorReportService.SaveFileToLocalThenSlpitThem(fs, Path.GetFileName(filePath), "Fingertip", Environment.CurrentDirectory);
            ////}
            //filePath = @"D:\Devblock\linkitportal-vina\TestConsole\bin\Debug\G6 ELA.pdf";
            //using (FileStream fs = new FileStream(filePath, FileMode.Open))
            //{
            //    var res = navigatorReportService.ProcessUploadFiles(new NavigatorReportUploadFileFormDataDTO()
            //    {
            //        ReportType = "TeacherSlide",
            //        School = 11,
            //        District = 22
            //    }, new List<NavigatorReportFileDTO>()
            //    {
            //        new NavigatorReportFileDTO(){FileName = Path.GetFileName(filePath),InputStream = fs }
            //    });
            //}
            //Console.ReadKey();
        }
    }
    public class FSchoolRepos : IRepository<School>
    {
        public void Delete(School item)
        {
            throw new NotImplementedException();
        }

        public void Save(School item)
        {
            throw new NotImplementedException();
        }

        public IQueryable<School> Select()
        {
            return new List<School>() { new School() { Id = 11, Name = "SchooNameX" } }.AsQueryable();
        }
    }

}
