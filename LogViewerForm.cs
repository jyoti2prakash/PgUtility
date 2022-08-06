using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGUtility
{
    public partial class LogViewerForm : Form
    {
        public LogViewerForm()
        {
            InitializeComponent();
        }
        IList<LogModel> LogsList = new List<LogModel>();

        private void LogViewerForm_Load(object sender, EventArgs e)
        {
            splitContainer1.SplitterDistance = 20;
            LoadJson();
        }
        private void LoadJson()
        {
            using (WebClient wc = new WebClient())
            {
                var json = wc.DownloadString("https://a360devanalysis.blob.core.windows.net/a360-mc-errorlog/0_aa2b6997d13541d3988a5d2f9da7fb9b_1.json");


                JsonTextReader reader = new JsonTextReader(new StringReader(json));
                reader.SupportMultipleContent = true;

                while (true)
                {
                    if (!reader.Read())
                    {
                        break;
                    }
                    JsonSerializer serializer = new JsonSerializer();
                    LogModel role = serializer.Deserialize<LogModel>(reader);
                    LogsList.Add(role);
                }



                // DataTable dt = JsonConvert.DeserializeObject<DataTable>(json);
            }
        }
        private void btnSearch_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = LogsList.Where(x=>x.ErrorMessage.Contains(txtSearch.Text, StringComparison.OrdinalIgnoreCase)).ToList();
        }

    }

    public class LogModel
    {
        public string RefId { get; set; }
        public string RefCode { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorDescription { get; set; }
        public string ErrorContext { get; set; }
        public string TotalPageComplete { get; set; }
        public string ClientCode { get; set; }
        public string JobName { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public string SourceId { get; set; }
        public string InputJson { get; set; }
        public string UserEmail { get; set; }
        public string UserLocation { get; set; }
        public string ModuleName { get; set; }
        public string ActionName { get; set; }
        public string PageEnterTime { get; set; }
        public string PageOutTime { get; set; }
        public string OutputJSON { get; set; }
        public string APIURL { get; set; }
        public string Source { get; set; }
        public string PageName { get; set; }
        public string ApplicationId { get; set; }
        public string SecretKey { get; set; }
        public string Logtype { get; set; }
        public string CreatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string Version { get; set; }
        public string Browser { get; set; }
        public string BrowserVersion { get; set; }
        public string EnvironmentType { get; set; }
        public DateTime EventProcessedUtcTime { get; set; }
        public int PartitionId { get; set; }
        public DateTime EventEnqueuedUtcTime { get; set; }
    }

}
