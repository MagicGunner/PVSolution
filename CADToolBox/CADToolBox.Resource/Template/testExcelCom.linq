<Query Kind="Statements">
  <NuGetReference>Microsoft.Office.Interop.Excel</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Outlook</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.PowerPoint</NuGetReference>
  <NuGetReference>Microsoft.Office.Interop.Word</NuGetReference>
  <Namespace>Excel = Microsoft.Office.Interop.Excel</Namespace>
  <Namespace>Outlook = Microsoft.Office.Interop.Outlook</Namespace>
  <Namespace>PowerPoint = Microsoft.Office.Interop.PowerPoint</Namespace>
  <Namespace>Word = Microsoft.Office.Interop.Word</Namespace>
</Query>

// 创建Excel应用程序对象
Excel.Application excelApp = new Excel.Application();

// 打开Excel工作簿
Excel.Workbook workbook = excelApp.Workbooks.Open(@"E:\BaiduSyncdisk\Works\00-Research_and_development\PVSolution\CADToolBox\AutoGA\Template\截面数据.xlsm");

// 获取第一个工作表
Excel.Worksheet worksheet = workbook.Worksheets["汇总"];

Excel.Range r = worksheet.Range["A1"];


