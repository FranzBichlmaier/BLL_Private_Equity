using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Windows.Documents.Spreadsheet.Model;
using Telerik.Windows.Documents.Spreadsheet.PropertySystem;

namespace BLL_Private_Equity.Berechnungen
{
    public class ExcelHelperFunctions
    {
        Worksheet sheet;

        public int RowCount { get; set; }

        public int ColumnCount { get; set; }

        public ExcelHelperFunctions(Worksheet _sheet)
        {
            sheet = _sheet;
            CellRange result = sheet.GetUsedCellRange(new IPropertyDefinition[] { CellPropertyDefinitions.ValueProperty });

            RowCount = result.RowCount;
            ColumnCount = result.ColumnCount;
        }

        public CellSelection FindTextStartsWith(string searchText)
        {
            for (int i = 0; i < RowCount; i++)
            {
                CellIndex cellIndex = new CellIndex(i,0);
                CellSelection selection = sheet.Cells[cellIndex];

                ICellValue cellValue = selection.GetValue().Value;
                if (cellValue.ValueType == CellValueType.Text)
                {
                    string content = cellValue.RawValue;
                    content = content.Trim();
                    if (content.Contains(searchText))
                    {
                        return selection;
                    }
                }
            }
            return null;
        }

        public int FindTextInHeadline(int row, string searchText)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                CellIndex cellIndex = new CellIndex(row, i);
                CellSelection selection = sheet.Cells[cellIndex];

                ICellValue cellValue = selection.GetValue().Value;
                if (cellValue.ValueType == CellValueType.Text)
                {
                    string content = cellValue.RawValue;
                    content = content.Trim();
                    if (content.StartsWith(searchText))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int FindTextInHeadline(int row, string searchText1, string searchText2)
        {
            for (int i = 0; i < ColumnCount; i++)
            {
                CellIndex cellIndex = new CellIndex(row, i);
                CellSelection selection = sheet.Cells[cellIndex];

                ICellValue cellValue = selection.GetValue().Value;
                if (cellValue.ValueType == CellValueType.Text)
                {
                    string content = cellValue.RawValue;
                    content = content.Trim();
                    if (content.Contains(searchText1) && content.Contains(searchText2))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }

        public int FindRowforTextStartsWith(string searchText)
        {
            for (int i = 0; i < RowCount; i++)
            {
                CellIndex cellIndex = new CellIndex(i, 0);
                CellSelection selection = sheet.Cells[cellIndex];

                ICellValue cellValue = selection.GetValue().Value;
                if (cellValue.ValueType == CellValueType.Text)
                {
                    string content = cellValue.RawValue;
                    content = content.Replace(" ", "");                   
                    if (content.Contains(searchText))
                    {
                        return i;
                    }
                }
            }
            return -1;
        }
        /// <summary>
        /// returns a string array which contains a distinct list of content
        /// </summary>
        /// <param name="row">defines the column for the search</param>
        /// <returns></returns>
        public string[] GetDistinctListOfText(int column)
        {
            Dictionary<string, string> contents = new Dictionary<string,string>();

            for(int row =1; row <RowCount; row ++)
            {
                string content = GetText(row, column);
                if (content.Length == 0) continue;
                string existingContent; 
                if (contents.TryGetValue(content, out existingContent)) continue;
                contents.Add(content, content);
            }
            int length = contents.Count();
            string[] results = new string[length];
            length=0;
            foreach(KeyValuePair<string, string> item in contents)
            {
                results[length] = item.Key;
                length++;
            }
            return results;
        }

        public double GetAmount(int row, int column)
        {
            CellIndex cellIndex = new CellIndex(row, column);
            CellSelection selection = sheet.Cells[cellIndex];
            ICellValue cellValue = selection.GetValue().Value;
            // in case of DBNULL the cell contains a text with content 'NULL'
            // return '0'
            if (cellValue.ValueType == CellValueType.Text)
            {
                if (cellValue.RawValue == "NULL") return 0;
                // An Exception will be thrown
            }

            if (cellValue.ValueType == CellValueType.Empty)
                return 0;
            if (cellValue.ValueType != CellValueType.Number)
            {
                throw new Exception("Value is not a number");
            }
            return double.Parse(cellValue.RawValue);
        }

        public string GetText(int row, int column)
        {
            CellIndex cellIndex = new CellIndex(row, column);
            CellSelection selection = sheet.Cells[cellIndex];
            ICellValue cellValue = selection.GetValue().Value;
            if (cellValue.ValueType == CellValueType.Empty)
                return string.Empty;
            if (cellValue.ValueType != CellValueType.Text)
            {
                if (cellValue.ValueType == CellValueType.Number)
                {
                    return cellValue.RawValue.ToString();
                }
                throw new Exception("Value ist not a text");
            }
            return cellValue.RawValue;
        }

        public DateTime GetDate(int row, int column)
        {
            int serialDate = 0;
            CellIndex cellIndex = new CellIndex(row, column);
            CellSelection selection = sheet.Cells[cellIndex];
            ICellValue cellValue = selection.GetValue().Value;
            if (cellValue.ValueType == CellValueType.Empty)
                return new DateTime(2000,1,1);
            if (cellValue.ValueType != CellValueType.Number)
            {
                // try datetime.tryparse
                string[] formats = { "dd.MM.yyyy" };
                DateTime d = DateTime.Now;
                if (DateTime.TryParseExact(cellValue.RawValue, formats,
                    System.Globalization.CultureInfo.CreateSpecificCulture("de-De"), System.Globalization.DateTimeStyles.None, out d))
                {
                    return d;
                }
          
                throw new Exception("Value is not a date");
            }
            if (int.TryParse(cellValue.RawValue, out serialDate))
            {
                return FromExcelSerialDate((int)serialDate);
            }
            return DateTime.MinValue;
        }

        public bool IsNumber(int row, int column)
        {
            CellIndex cellIndex = new CellIndex(row, column);
            CellSelection selection = sheet.Cells[cellIndex];
            ICellValue cellValue = selection.GetValue().Value;
           
            if (cellValue.ValueType == CellValueType.Number)
            {
                return true;
            }
            return false;
        }

        public bool IsText(int row, int column)
        {
            CellIndex cellIndex = new CellIndex(row, column);
            CellSelection selection = sheet.Cells[cellIndex];
            ICellValue cellValue = selection.GetValue().Value;

            if (cellValue.ValueType == CellValueType.Text)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// this routine starts in row of column and looks downwards for a date. 
        /// if it finds one, it is returned, if not Datetime.minvalue is returned
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public DateTime FindDate(int column, int row)
        {
            int serialDate = 0;
            for (int i = row; i < RowCount; i++)
            {
                CellIndex cellIndex = new CellIndex(i, column);
                CellSelection selection = sheet.Cells[cellIndex];

                ICellValue cellValue = selection.GetValue().Value;
                if (cellValue.ValueType == CellValueType.Number)
                {
                    if (int.TryParse(cellValue.RawValue, out serialDate))
                    {
                        return FromExcelSerialDate((int)serialDate);
                    }
                    break;                   
                }
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// this routine starts in row of column and looks downwards for a string.
        /// It returns the found string or string.empty in case there was no string
        /// </summary>
        /// <param name="column"></param>
        /// <param name="row"></param>
        /// <returns></returns>
        public string FindString(int column, int row)
        {
            for (int i = row; i < RowCount; i++)
            {
                CellIndex cellIndex = new CellIndex(i, column);
                CellSelection selection = sheet.Cells[cellIndex];

                ICellValue cellValue = selection.GetValue().Value;
                if (cellValue.ValueType == CellValueType.Text)
                {
                    return cellValue.RawValue;
                }
            }
            return string.Empty;
        }

        private static DateTime FromExcelSerialDate(int serialDate)
        {
            if (serialDate > 59)
                serialDate -= 1; //Excel/Lotus 2/29/1900 bug   
            return new DateTime(1899, 12, 31).AddDays(serialDate);
        }
    }
}