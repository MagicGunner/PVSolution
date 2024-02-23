<Query Kind="Statements">
  <NuGetReference>DocumentFormat.OpenXml</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Excel</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Outlook</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.PowerPoint</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Word</NuGetReference>
  <Namespace>DocumentFormat.OpenXml</Namespace>
  <Namespace>DocumentFormat.OpenXml.Packaging</Namespace>
  <Namespace>DocumentFormat.OpenXml.Spreadsheet</Namespace>
  <Namespace>Excel = Microsoft.Office.Interop.Excel</Namespace>
  <Namespace>Outlook = Microsoft.Office.Interop.Outlook</Namespace>
  <Namespace>PowerPoint = Microsoft.Office.Interop.PowerPoint</Namespace>
  <Namespace>Word = Microsoft.Office.Interop.Word</Namespace>
  <Namespace>LINQPad.FSharpExtensions</Namespace>
</Query>

//string filePath = Path.GetDirectoryName(Util.CurrentQueryPath);

string fileName = @"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\CADToolBox\AutoGA\Template\截面数据.xlsm";
string sheetName = "汇总";
string addressName = "A1";

string? value;
using (SpreadsheetDocument document =
	SpreadsheetDocument.Open(fileName, false))
{
	// Retrieve a reference to the workbook part.
	WorkbookPart wbPart = document.WorkbookPart ?? document.AddWorkbookPart();

	// Find the sheet with the supplied name, and then use that 
	// Sheet object to retrieve a reference to the first worksheet.
	Sheet? theSheet = wbPart.Workbook.Descendants<Sheet>().Where(s => s.Name == sheetName).FirstOrDefault();

	// Throw an exception if there is no sheet.
	if (theSheet is null || theSheet.Id is null)
	{
		throw new ArgumentException("sheetName");
	}

	// Retrieve a reference to the worksheet part.
	WorksheetPart wsPart = (WorksheetPart)wbPart.GetPartById(theSheet.Id!);

	// Use its Worksheet property to get a reference to the cell 
	// whose address matches the address you supplied.
	Cell? theCell = wsPart.Worksheet?.Descendants<Cell>()?.Where(c => c.CellReference == addressName).FirstOrDefault();

	// If the cell does not exist, return an empty string.
	if (theCell is null || theCell.InnerText.Length < 0)
	{
		value = string.Empty;
	}

	value = theCell.InnerText;

	// If the cell represents an integer number, you are done. 
	// For dates, this code returns the serialized value that 
	// represents the date. The code handles strings and 
	// Booleans individually. For shared strings, the code 
	// looks up the corresponding value in the shared string 
	// table. For Booleans, the code converts the value into 
	// the words TRUE or FALSE.
	if (theCell.DataType is not null)
	{
		if (theCell.DataType.Value == CellValues.SharedString)
		{

			// For shared strings, look up the value in the
			// shared strings table.
			var stringTable = wbPart.GetPartsOfType<SharedStringTablePart>().FirstOrDefault();

			// If the shared string table is missing, something 
			// is wrong. Return the index that is in
			// the cell. Otherwise, look up the correct text in 
			// the table.
			if (stringTable is not null)
			{
				value = stringTable.SharedStringTable.ElementAt(int.Parse(value)).InnerText;
			}
		}
		else if (theCell.DataType.Value == CellValues.Boolean)
		{
			switch (value)
			{
				case "0":
					value = "FALSE";
					break;
				default:
					value = "TRUE";
					break;
			}
		}
	}
}

value.Dump();