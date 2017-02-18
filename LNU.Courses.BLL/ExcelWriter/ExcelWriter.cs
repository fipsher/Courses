using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using Bytescout.Spreadsheet;
using Bytescout.Spreadsheet.Constants;
using Bytescout.Spreadsheet.Structures;
using Entities;

namespace LNU.Courses.BLL.ExcelWriter
{
    public class ExcelWriter<T>
    {
        private readonly Spreadsheet _document;
        private readonly Worksheet _worksheet;
        private readonly string _sheetName;

        public ExcelWriter(string sheetName)
        {
            _document = new Spreadsheet();
            _worksheet = _document.Workbook.Worksheets.Add(sheetName);
            _sheetName = sheetName;
        }

        public byte[] WriteToExcel(IEnumerable<T> dataList)
        {
            var headers = typeof(T).GetProperties();
            for (int i = 0; i < headers.Length; i++)
            {
                _worksheet.Cell(0, i).Value = headers[i].Name;
            }

            _worksheet.Rows[0].Height = 50;
            _worksheet.Rows[0].AlignmentVertical = AlignmentVertical.Centered;
            _worksheet.Rows[0].Font = new Font("Arial", 18, FontStyle.Bold | FontStyle.Italic);

            var r = 1;

            foreach (var data in dataList)
            {
                var c = 0;
                var maxLines = 1;
                foreach (var prop in data.GetType().GetProperties())
                {
                    var currLine = r;
                    //var dataTemp = new Users() 
                    if (prop.Name != "StudentsInGroups" && prop.Name != "Users")
                    {
                        var cellValue = prop.GetValue(data, null);

                        if (cellValue != null)
                        {
                            if (!cellValue.GetType().IsGenericType)
                            {
                                _worksheet.Cell(currLine, c).Value = cellValue;
                            }
                        }
                        c++;
                    }
                   
                }
                r = r + maxLines;
            }

            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Name.ToLower().Contains("photo"))
                {
                    _worksheet.Columns[i].Width = 150;
                }
                else
                {
                    _worksheet.Columns[i].AutoFit();
                    _worksheet.Columns[i].AlignmentVertical = AlignmentVertical.Centered;
                }
            }

            byte[] result = _document.GetAsBytesArrayXLSX();

            return result;
        }

        private Image GetImageByUrl(string url)
        {
            WebClient wc = new WebClient();

            byte[] bytes = wc.DownloadData(url);
            MemoryStream ms = new MemoryStream(bytes);

            Image image = Image.FromStream(ms);

            return image;
        }
    }
}
