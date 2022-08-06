using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PGUtility
{
    public partial class ScriptMergeForm : Form
    {
        public ScriptMergeForm()
        {
            InitializeComponent();
        }

        private void ScriptMergeForm_Load(object sender, EventArgs e)
        {
            string folderPath = @"D:\A360\droisys_cloud\DroisysCloud\APIServiceFoundation\DatabaseScript\Sprint-17\Dev\Function";

            StringBuilder stringBuilder = new StringBuilder();
            foreach (string file in Directory.EnumerateFiles(folderPath, "*.sql"))
            {
                string contents = File.ReadAllText(file);
                stringBuilder.Append("-- ===============================================================================").Append(Environment.NewLine);
                stringBuilder.Append("-- ======").Append(file).Append(Environment.NewLine);
                stringBuilder.Append("-- ===============================================================================").Append(Environment.NewLine);
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(Environment.NewLine);
                stringBuilder.Append(contents).Append(Environment.NewLine);
                stringBuilder.Append(Environment.NewLine);

            }
            textEditorControl1.Text = stringBuilder.ToString();
        }
    }
}
