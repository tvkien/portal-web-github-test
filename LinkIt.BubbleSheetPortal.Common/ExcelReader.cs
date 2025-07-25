using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Common
{
    public class ExcelReader
    {
        public static List<T> ReadExcelFile<T>(Stream stream) where T : new()
        {
            var items = new List<T>();

            using (var package = new ExcelPackage(stream))
            {
                var worksheet = package.Workbook.Worksheets.FirstOrDefault();
                var headers = worksheet.Cells[1, 1, 1, worksheet.Dimension.Columns].Select(c => c.Text.Trim()).ToList();

                var propertyMappings = typeof(T).GetProperties()
                    .ToDictionary(p => p.Name, p => headers.IndexOf(p.Name) + 1);

                for (int row = 2; row <= worksheet.Dimension.Rows; row++)
                {
                    var item = new T();
                    foreach (var propertyMapping in propertyMappings)
                    {
                        var colIndex = propertyMapping.Value;
                        if (colIndex > 0)
                        {
                            var cellValue = worksheet.Cells[row, colIndex].Text;
                            typeof(T).GetProperty(propertyMapping.Key)?.SetValue(item, cellValue);
                        }
                    }
                    items.Add(item);
                }
            }

            return items;
        }
    }
}
