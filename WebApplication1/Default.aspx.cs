using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.IO;
using System.Text;
using DatePickerControl;

namespace WebApplication1
{
    public partial class _Default : System.Web.UI.Page
    {
        private DatePicker StartPicker;
        private DatePicker EndPicker;
        private ContentPlaceHolder Body;
        public Boolean HasData = false;
        public List<TableRow> TableRows = new List<TableRow>();
        //Fields to use to show what we are displaying
        public String[] TableColumnNames;
        
        protected void Page_Load(object sender, EventArgs e)
        {
            StartPicker = (DatePicker)Body.FindControl("StartPicker");
            EndPicker = (DatePicker)Body.FindControl("EndPicker");
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            Body = (ContentPlaceHolder)this.Master.FindControl("MainContent");
        }

        /// <summary>
        /// This is called when the user clicks the button, finds the stock data for the specified time intervals and stock symbol and puts the data 
        /// into a list. 
        /// </summary>
        protected void FindStockData(object sender, EventArgs e)
        {
            string ticker = TickerSymbol.Text;
            DateTime startDate = StartPicker.CalendarDate;
            DateTime endDate = EndPicker.CalendarDate;
            if (string.IsNullOrEmpty(ticker) || !(ticker.Split(' ').Length == 1) || ticker.ToLower().Equals("ticker symbol"))
            {
                HasData = false;
                TableRows.Clear();
                return;
            }
            DownloadDataFromWeb(ticker, startDate, endDate);
        }

        public void DownloadDataFromWeb(string symbol, DateTime startDate, DateTime endDate)
        {
            string baseURL = "http://ichart.finance.yahoo.com/table.csv?";
            DateTime first = new DateTime(1, 1, 1);
            DateTime end = endDate == first ? DateTime.Today : endDate;
            DateTime start = startDate == first ? end.AddYears(-1) : startDate;
            TimeSpan diff = end - start;
            
            
            string queryText = BuildHistoricalDataRequest(symbol, start, end);
            string url = string.Format("{0}{1}", baseURL, queryText);

            //Get page showing the table with the chosen indices
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            StreamReader stReader = null;

            //csv content
            string docText = string.Empty;
            string csvLine = null;
            //Table stockTable = (Table)this.Body.FindControl("StockTable");
            
            try
            {
                request = (HttpWebRequest)WebRequest.CreateDefault(new Uri(url));
                request.Timeout = 300000;

                response = (HttpWebResponse)request.GetResponse();

                stReader = new StreamReader(response.GetResponseStream(), true);

                stReader.ReadLine();//skip the first (header row)
                TableColumnNames = new String[]{"StartDate", "Open", "High", "Low", "Close", "Volume", "Adjusted Close"};
                //stockTable.Rows.Add(CreateTableRow(BuildList("StartDate", "Open", "High", "Low", "Close", "Volume", "Adjusted Close")));
                while ((csvLine = stReader.ReadLine()) != null)
                {
                    string[] sa = csvLine.Split(new char[] { ',' });

                    DateTime date = DateTime.Parse(sa[0].Trim('"'));
                    Double open = double.Parse(sa[1]);
                    Double high = double.Parse(sa[2]);
                    Double low = double.Parse(sa[3]);
                    Double close = double.Parse(sa[4]);
                    Double volume = double.Parse(sa[5]);
                    Double adjClose = double.Parse(sa[6]);
                    
                    TableRow row = CreateTableRow(BuildList(date.Month + "-" + date.Day + "-" + date.Year, Convert.ToString(open), Convert.ToString(high), Convert.ToString(low), 
                        Convert.ToString(close), Convert.ToString(volume), Convert.ToString(adjClose)));
                    TableRows.Add(row);

                    TableCellCollection cells = row.Cells;
                    for (int i = 0; i < cells.Count; i++)
                    {
                        TableCell cell = cells[i];
                        string text = cell.Text;
                    }
                    //stockTable.Rows.Add(row);
                    
                    // Process the data (e.g. insert into DB)
                    HasData = true;
                }
            }
            catch (Exception e)
            {
                Label label = new Label();
                label.Text = "Stock not found";
                //Body.Controls.Add(label);
            }
        }

        public Boolean DownloadChart(DateTime start, DateTime end)
        {
            return false;
        }
        public string BuildHistoricalDataRequest(string symbol, DateTime startDate, DateTime endDate)
        {
            // We're subtracting 1 from the month because yahoo
            // counts the months from 0 to 11 not from 1 to 12.
            StringBuilder request = new StringBuilder();
            request.AppendFormat("s={0}", symbol);
            request.AppendFormat("&a={0}", startDate.Month - 1);
            request.AppendFormat("&b={0}", startDate.Day);
            request.AppendFormat("&c={0}", startDate.Year);
            request.AppendFormat("&d={0}", endDate.Month - 1);
            request.AppendFormat("&e={0}", endDate.Day);
            request.AppendFormat("&f={0}", endDate.Year);
            request.AppendFormat("&g={0}", "d"); //daily

            return request.ToString();
        }

        public List<string> BuildList(string startDate, string open, string high, string low, string close, string volume, string adjClose)
        {
            List<string> result = new List<string>();
            result.Add(startDate);
            result.Add(open);
            result.Add(high);
            result.Add(low);
            result.Add(close);
            result.Add(volume);
            result.Add(adjClose);
            return result;
        }
        public TableCell CreateTableCell(string celltext)
        {
            TableCell tableCell = new TableCell();
            tableCell.Text = celltext;
            return tableCell;
        }

        public TableRow CreateTableRow(IEnumerable<string> rowText)
        {
            TableRow row = new TableRow();
            foreach (string text in rowText)
            {
                row.Cells.Add(CreateTableCell(text));
            }
            return row;
        }

    }
}
