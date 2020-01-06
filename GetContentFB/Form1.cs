using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GetContentFB
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void importUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.InitialDirectory = ConfigurationManager.AppSettings["DefaultFolder"];
                openFileDialog.Filter = "Excel files (*.xls)|*.xls*|All files (*.*)|*.*";
                openFileDialog.FilterIndex = 2;
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Title = "Load one file";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Get the path of specified file
                    string filePath = openFileDialog.FileName;

                    DataTable excelDataFile = ParseExcelFile(filePath);
                }
                else
                {
                    return;
                }
            }
        }

        private DataTable ParseExcelFile(string fileName)
        {
            XSSFWorkbook wb;
            XSSFSheet sh;
            String Sheet_name;

            using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                wb = new XSSFWorkbook(fs);

                Sheet_name = wb.GetSheetAt(0).SheetName;  //get first sheet name
            }
            DataTable DT = new DataTable();
            DT.Rows.Clear();
            DT.Columns.Clear();

            // get sheet
            sh = (XSSFSheet)wb.GetSheet(Sheet_name);

            int i = 0;
            while (sh.GetRow(i) != null)
            {
                // add neccessary columns
                if (DT.Columns.Count < sh.GetRow(i).Cells.Count)
                {
                    for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                    {
                        DT.Columns.Add("", typeof(string));
                    }
                }

                // add row
                DT.Rows.Add();

                // write row value
                for (int j = 0; j < sh.GetRow(i).Cells.Count; j++)
                {
                    var cell = sh.GetRow(i).GetCell(j);

                    if (cell != null)
                    {
                        // TODO: you can add more cell types capatibility, e. g. formula
                        switch (cell.CellType)
                        {
                            case NPOI.SS.UserModel.CellType.Numeric:
                                DT.Rows[i][j] = sh.GetRow(i).GetCell(j).NumericCellValue;
                                //dataGridView1[j, i].Value = sh.GetRow(i).GetCell(j).NumericCellValue;

                                break;
                            case NPOI.SS.UserModel.CellType.String:
                                DT.Rows[i][j] = sh.GetRow(i).GetCell(j).StringCellValue;

                                break;
                        }
                    }
                }

                i++;
            }

            return DT;
        }

        private void WriteLog(string log)
        {
            string fileName = Path.Combine(ConfigurationManager.AppSettings.Get("DefaultFolder")
                , ConfigurationManager.AppSettings.Get("LogFile"));
            try
            {
                using (StreamWriter w = File.AppendText(fileName))
                {
                    log = DateTime.Now.ToLongDateString() + " " + DateTime.Now.ToLongTimeString() + ": " + log;
                    w.WriteLine(log);
                }
            }
            catch
            { }
        }
    }
}
