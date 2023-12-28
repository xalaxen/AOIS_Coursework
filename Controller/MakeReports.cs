using Microsoft.Office.Interop.Excel;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Xml.Linq;
using ApplicationExcel = Microsoft.Office.Interop.Excel.Application;
using ApplicationWord = Microsoft.Office.Interop.Word.Application;
using excel = Microsoft.Office.Interop.Excel;
using word = Microsoft.Office.Interop.Word;

namespace AOIS.Controller
{
    public class MakeReports
    {
        private ApplicationWord wordapp = new word.Application();
        private Documents worddocuments;
        private Document worddocument;

        private ApplicationExcel excelapp = new excel.Application();
        private Workbook workbook;
        private Worksheet worksheet;

        private Dictionary<int, int> chartData;
        private string chartTitle;

        public MakeReports(string template)
        {
            wordapp.Visible = false;

            Object newTemplate = false;
            Object documentType = word.WdNewDocumentType.wdNewBlankDocument;
            Object visible = false;

            wordapp.Documents.Add(template, newTemplate, ref documentType, ref visible);

            worddocuments = wordapp.Documents;
            worddocument = worddocuments.get_Item(1);
            worddocument.Activate();
        }

        public excel.Chart GenerateFilmCountByYearChart(Dictionary<int, int> filmsByYear)
        {
            // Создаем новый экземпляр Excel
            excelapp.Visible = false;

            // Создаем новую книгу Excel
            Workbook workbook = excelapp.Workbooks.Add();
            Worksheet worksheet = workbook.Worksheets[1];

            // Сортируем данные по годам
            var sortedFilmsByYear = filmsByYear.OrderBy(entry => entry.Key).ToDictionary(entry => entry.Key, entry => entry.Value);

            // Заполняем данные в Excel
            int row = 2;
            foreach (var entry in sortedFilmsByYear)
            {
                worksheet.Cells[row, 1] = entry.Key; // Год
                worksheet.Cells[row, 2] = entry.Value; // Количество фильмов
                row++;
            }

            // Добавляем диаграмму в Excel
            ChartObjects chartObjects = (ChartObjects)worksheet.ChartObjects();
            ChartObject chartObject = chartObjects.Add(100, 80, 300, 200);
            excel.Chart chart = chartObject.Chart;
            excel.Range chartRange = worksheet.Range["A1", "B" + (row - 1)];
            chart.SetSourceData(chartRange);
            chart.ChartType = XlChartType.xlColumnClustered;

            // Переворачиваем ориентацию графика
            chart.PlotBy = excel.XlRowCol.xlColumns;

            return chart;
        }

        public void SaveChartImage(string imagePath, excel.Chart chart)
        {
            // Сохраняем изображение графика
            chart.Export(imagePath, "PNG");
        }

        public void InsertChartIntoWord(string chartImagePath)
        {
            // Вставляем изображение графика в Word
            InlineShape inlineShape = worddocument.InlineShapes.AddPicture(chartImagePath);

            // Перемещаем график в конец документа
            word.Range endRange = worddocument.Range();
            endRange.Collapse(WdCollapseDirection.wdCollapseEnd);
            inlineShape.Range.Cut();
            endRange.Paste();
            File.Delete(chartImagePath);
        }

        public void Replace(string wordr, string replacement)
        {
            word.Range range = worddocument.StoryRanges[word.WdStoryType.wdMainTextStory];
            range.Find.ClearFormatting();

            range.Find.Execute(FindText: wordr, ReplaceWith: replacement);

            TrySave();
        }

        public void ReplaceBookmark(string bookmark, string replacement)
        {
            worddocument.Bookmarks[bookmark].Range.Text = replacement;
            TrySave();
        }

        public void TrySave()
        {
            try
            {
                string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), DateTime.Now.ToString("yyyyMMdd HHmmss") + " New Report.doc");
                worddocument.SaveAs2(savePath, word.WdSaveFormat.wdFormatDocument97);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void Close()
        {
            Object saveChanges = word.WdSaveOptions.wdPromptToSaveChanges;
            Object originalFormat = word.WdOriginalFormat.wdWordDocument;
            Object routeDocument = Type.Missing;
            wordapp.Quit(ref saveChanges, ref originalFormat, ref routeDocument);
            if(excelapp.ActiveWorkbook != null)
            {
                excelapp.ActiveWorkbook.Saved = true;
                excelapp.Quit();
                Marshal.ReleaseComObject(excelapp);
                System.Runtime.InteropServices.Marshal.FinalReleaseComObject(excelapp);
            }
        }
    }
}
