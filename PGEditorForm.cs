using Npgsql;
using PGUtility.Repository;
using System;
using System.Collections;
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
    public partial class PGEditorForm : Form
    {
        List<Tuple<string, NpgsqlConnection>> connections = new List<Tuple<string, NpgsqlConnection>>();
        public PGEditorForm()
        {
            InitializeComponent();
        }

        private void PGEditorForm_Load(object sender, EventArgs e)
        {
            toolStripComboBoxConnection.ComboBox.DataSource = SqlHelper.Connections;
            toolStripComboBoxConnection.ComboBox.DisplayMember = "ConnectionName";
            //  toolStripComboBoxConnection.Items.AddRange(SqlHelper.Connections.Select(X => X.ConnectionName).ToArray());

            toolStripComboBoxConnection.SelectedIndex = 0;
            BindTreeViewDBInfo();
        }
        private async void BindTreeViewDBInfo()
        {

            foreach (var item in SqlHelper.Connections)
            {
                TreeNode treeNodeDb = new TreeNode(item.ConnectionName);
                treeNodeDb.Tag = item;
                treeNodeDb.Name = "Connection";
                treeView1.Nodes.Add(treeNodeDb);
                treeNodeDb.Nodes.Add("Loading..");
            }
            //  BindTreeViewDataBase();
        }
        private async void BindTreeViewConnection()
        {
            List<string> lstDatabases = new List<string>();
            lstDatabases = await PgDbRepository.ExecuteListAsync<string>("SELECT datname FROM pg_database WHERE datistemplate = FALSE;", CommandType.Text, null);
            foreach (var item in lstDatabases)
            {
                TreeNode treeNodeDb = new TreeNode(item);

                treeView1.Nodes.Add(treeNodeDb);
                treeNodeDb.Name = "DataBase";
                treeNodeDb.Tag = "DataBase";
                treeNodeDb.Nodes.Add("Loading..");
            }
        }

        private async void BindTreeViewDataBase(TreeNode node)
        {
            List<string> lstDatabases = new List<string>();
            SqlConnection connection = (SqlConnection)node.Tag;
            lstDatabases = SqlHelper.ExecuteList<string>(connection, "SELECT datname FROM pg_database WHERE datistemplate = FALSE;");
            node.Nodes.Clear();

            if (lstDatabases.Count > 0)
            {
                foreach (var item in lstDatabases)
                {
                    SqlConnection connectionDb = connection.Copy(item);
                    TreeNode treeNodeDb = new TreeNode(item);
                    node.Nodes.Add(treeNodeDb);
                    treeNodeDb.Name = "DataBase";
                    treeNodeDb.Tag = connectionDb;
                    treeNodeDb.Nodes.Add("Loading..");
                }
            }

        }
        private async void BindTreeViewSchema(TreeNode node)
        {
            string dbname = node.Text;
            List<string> lstSchema = new List<string>();
            SqlConnection connection = (SqlConnection)node.Tag;
            lstSchema = SqlHelper.ExecuteList<string>(connection, "SELECT schema_name FROM information_schema.schemata WHERE catalog_name = '" + dbname + "' and schema_name not like 'pg_t%'; ");
            node.Nodes.Clear();

            if (lstSchema.Count > 0)
            {
                foreach (var item in lstSchema)
                {
                    TreeNode treeNodeDb = new TreeNode(item);
                    treeNodeDb.Tag = connection;
                    treeNodeDb.Name = "Schema";
                    node.Nodes.Add(treeNodeDb);

                    TreeNode treeNodeTable = new TreeNode("Tables");
                    treeNodeTable.Name = "Tables";
                    treeNodeTable.Tag = connection;
                    treeNodeTable.Nodes.Add("Loading..");
                    treeNodeDb.Nodes.Add(treeNodeTable);

                    TreeNode treeNodeFunction = new TreeNode("Functions");
                    treeNodeFunction.Name = "Functions";
                    treeNodeFunction.Tag = connection;
                    treeNodeFunction.Nodes.Add("Loading..");
                    treeNodeDb.Nodes.Add(treeNodeFunction);


                    TreeNode treeNodeViews = new TreeNode("Views");
                    treeNodeViews.Name = "Views";
                    treeNodeViews.Tag = connection;
                    treeNodeViews.Nodes.Add("Loading..");
                    treeNodeDb.Nodes.Add(treeNodeViews);
                }
            }
        }

        private async void BindTreeViewTable(TreeNode node)
        {
            string dbname = node.Text;
            List<string> lstSchema = new List<string>();
            SqlConnection connection = (SqlConnection)node.Tag;
            lstSchema = SqlHelper.ExecuteList<string>(connection, "SELECT schema_name FROM information_schema.schemata WHERE catalog_name = '" + dbname + "'; ");
            node.Nodes.Clear();

            if (lstSchema.Count > 0)
            {
                foreach (var item in lstSchema)
                {
                    TreeNode treeNode = new TreeNode(item);
                    treeNode.Tag = connection;
                    treeNode.Name = "Schema";
                    treeNode.Nodes.Add("Loading..");
                    node.Nodes.Add(treeNode);

                }
            }
        }
        private async void BindTreeViewView(TreeNode node)
        {
            string dbname = node.Text;
            List<string> lstSchema = new List<string>();
            SqlConnection connection = (SqlConnection)node.Tag;
            lstSchema = SqlHelper.ExecuteList<string>(connection, "SELECT schema_name FROM information_schema.schemata WHERE catalog_name = '" + dbname + "'; ");
            node.Nodes.Clear();

            if (lstSchema.Count > 0)
            {
                foreach (var item in lstSchema)
                {
                    TreeNode treeNode = new TreeNode(item);
                    treeNode.Tag = connection;
                    treeNode.Name = "Schema";
                    treeNode.Nodes.Add("Loading..");
                    node.Nodes.Add(treeNode);

                }
            }
        }
        private async void BindTreeViewFunction(TreeNode node)
        {
            string dbname = node.Text;
            List<string> lstFunctions = new List<string>();
            SqlConnection connection = (SqlConnection)node.Tag;
            string schemaName = node.Parent.Text;
            lstFunctions = SqlHelper.ExecuteList<string>(connection, "select p.proname as FunctionName from pg_proc p left join pg_namespace n on p.pronamespace = n.oid where n.nspname not in ('pg_catalog', 'information_schema') and n.nspname = '" + schemaName + "' order by  FunctionName; ");

            node.Nodes.Clear();

            if (lstFunctions.Count > 0)
            {
                foreach (var item in lstFunctions)
                {
                    TreeNode treeNode = new TreeNode(item);
                    treeNode.Tag = connection;
                    treeNode.Name = "Function";

                    node.Nodes.Add(treeNode);

                    treeNode.ContextMenuStrip = new ContextMenuStrip();
                    ToolStripMenuItem toolStripMenuItemCopy = new ToolStripMenuItem("Copy Body");
                    toolStripMenuItemCopy.Click += (sender, e) => ToolStripMenuItem_FunctionCopyBody_Click(treeNode, e); // ToolStripMenuItem_FunctionCopyBody_Click ;
                    treeNode.ContextMenuStrip.Items.Add(toolStripMenuItemCopy);

                    ToolStripMenuItem toolStripMenuItemSaveonFile = new ToolStripMenuItem("Save on File");
                    toolStripMenuItemSaveonFile.Click += (sender, e) => ToolStripMenuItem_FunctionSaveOnFile_Click(treeNode, e); // ToolStripMenuItem_FunctionCopyBody_Click ;
                    treeNode.ContextMenuStrip.Items.Add(toolStripMenuItemSaveonFile);
                }
            }
        }

        private void ToolStripMenuItem_FunctionCopyBody_Click(object sender, EventArgs e)
        {
            var node = ((TreeNode)sender);
            string functionName = node.Text;
            string schemanName = node.Parent.Parent.Text;
            SqlConnection connection = (SqlConnection)node.Tag;
            string functionBody = SqlHelper.ExecuteFirstOrDefault<string>(connection, "select case when l.lanname = 'internal' then p.prosrc  else pg_get_functiondef(p.oid) end as definition from pg_proc p left join pg_namespace n on p.pronamespace = n.oid  left join pg_language l on p.prolang = l.oid  left join pg_type t on t.oid = p.prorettype  where n.nspname = '" + schemanName + "' AND  p.proname ='" + functionName + "'; ");
            functionBody = $"DROP FUNCTION IF EXISTS \"{schemanName}\".{functionName};\n {functionBody}";
            textEditorControl1.Text = functionBody;
        }

        private void ToolStripMenuItem_FunctionSaveOnFile_Click(object sender, EventArgs e)
        {
            var node = ((TreeNode)sender);
            string functionName = node.Text;
            string schemanName = node.Parent.Parent.Text;
            SqlConnection connection = (SqlConnection)node.Tag;
            string functionBody = SqlHelper.ExecuteFirstOrDefault<string>(connection, "select case when l.lanname = 'internal' then p.prosrc  else pg_get_functiondef(p.oid) end as definition from pg_proc p left join pg_namespace n on p.pronamespace = n.oid  left join pg_language l on p.prolang = l.oid  left join pg_type t on t.oid = p.prorettype  where n.nspname = '" + schemanName + "' AND  p.proname ='" + functionName + "'; ");

            functionBody = $"DROP FUNCTION IF EXISTS \"{schemanName}\".{functionName};\n {functionBody}";
            //  textEditorControl1.Text = functionBody;

            string basepath = $"D:\\A360\\droisys_cloud\\DroisysCloud\\APIServiceFoundation\\DatabaseScript\\Aug Release 2022/{schemanName}/Functions/";
            string fileNmae = schemanName + "." + functionName + ".sql";

            // textEditorControl1.SaveFile(basepath + fileNmae);

            File.WriteAllText(basepath + fileNmae, functionBody);
            // FileInfo fileInfo = new FileInfo(basepath + fileNmae);
            MessageBox.Show("Function Saved Succssfully.");
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            TreeNode node = e.Node;
            switch (node.Name)
            {
                case "Connection":
                    BindTreeViewDataBase(node);
                    break;
                case "DataBase":
                    BindTreeViewSchema(node);
                    break;
                case "Tables":
                    BindTreeViewTable(node);
                    break;
                case "Functions":
                    BindTreeViewFunction(node);
                    break;
                case "Views":
                    BindTreeViewView(node);
                    break;
                default:
                    break;
            }
        }

        private void toolStripComboBoxConnection_SelectedIndexChanged(object sender, EventArgs e)
        {
            List<string> lstDatabases = new List<string>();
            SqlConnection connection = (SqlConnection)toolStripComboBoxConnection.ComboBox.SelectedValue; ;
            lstDatabases = SqlHelper.ExecuteList<string>(connection, "SELECT datname FROM pg_database WHERE datistemplate = FALSE;");
            List<SqlConnection> lstSqlDatabases = new List<SqlConnection>();
            foreach (var item in lstDatabases)
            {
                SqlConnection sqlConnection = connection.Copy(item);
                lstSqlDatabases.Add(sqlConnection);
            }
            toolStripComboBoxDatabase.ComboBox.DataSource = lstSqlDatabases;
            toolStripComboBoxDatabase.ComboBox.DisplayMember = "Database";
        }

        private void toolStripButtonPlay_Click(object sender, EventArgs e)
        {
            try
            {
                SqlConnection connection = (SqlConnection)toolStripComboBoxDatabase.ComboBox.SelectedValue;
                string query = textEditorControl1.Text;

                DataSet ds = SqlHelper.ExecuteDataSet(connection, query);
                dataGridView1.DataSource = ds.Tables[0];

                //foreach (var item in ds.Tables)
                //{
                //    // TabPage tabPage = new TabPage("Result 2");
                //    tabResult.Controls.Add(this.tabPageResult);
                //    tabResult.TabPages.Add("Tab 2");

                //}
                //var cc = tabResult.TabPages;

            }
            catch (Exception ex)
            {

            }


        }
        bool nodefound = false;
        private void toolStripTextBoxSearch_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {

                if (treeView1.Nodes.Count > 0)
                {

                    TreeNodeCollection nodes = treeView1.Nodes;
                    // for connection
                    foreach (TreeNode dbNode in nodes)
                    {

                        if (dbNode.IsExpanded)
                        {
                            //for Database
                            foreach (TreeNode schemaNode in dbNode.Nodes)
                            {


                                if (schemaNode.IsExpanded)
                                {
                                    // for table schema
                                    foreach (TreeNode tableFunctionNode in schemaNode.Nodes)
                                    {
                                        if (tableFunctionNode.IsExpanded)
                                        {
                                            // for table function view 
                                            foreach (TreeNode tfvNodes in tableFunctionNode.Nodes)
                                            {
                                                if (tfvNodes.IsExpanded)
                                                {

                                                    // for table function view 
                                                    foreach (TreeNode itemNode in tfvNodes.Nodes)
                                                    {
                                                        if (itemNode.Text.ToUpper().Contains(toolStripTextBoxSearch.Text.ToUpper().ToString()))
                                                        {  
                                                            var node = itemNode;
                                                            //node.IsVisible = true;
                                                            node.ForeColor = Color.DarkBlue;
                                                            
                                                           // itemNode.Visible = true;
                                                          // treeView1.Nodes.
                                                        }
                                                        else
                                                        {
                                                            if (toolStripTextBoxSearch.Text.Length > 0)
                                                                itemNode.ForeColor = Color.Gray;
                                                            else
                                                                itemNode.ForeColor = Color.Black;
                                                        }
                                                    }

                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

            }
        }
        private void toolStripTextBoxSearch_KeyUp_old(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                if (toolStripTextBoxSearch.Text.Trim() != "")
                {
                    if (treeView1.Nodes.Count > 0)
                    {
                        nodefound = false;

                        TreeNodeCollection nodes = treeView1.Nodes;
                        foreach (TreeNode n in nodes)
                        {

                            if (!nodefound)
                                PrintRecursive(n);
                            else
                                return;
                        }
                    }
                }
            }
        }

        private void PrintRecursive(TreeNode treeNode)
        {
            if (treeNode.Text.ToUpper().Contains(toolStripTextBoxSearch.Text.ToUpper().ToString()))
            {
                treeView1.SelectedNode = treeNode;
                treeView1.SelectedNode.Expand();
                treeView1.Focus();
           
                nodefound = true;
                return;
            }

            foreach (TreeNode tn in treeNode.Nodes)
            {
                PrintRecursive(tn);
            }
        }

        private void logViewerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogViewerForm logViewerForm = new LogViewerForm();
            logViewerForm.Show();
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptMergeForm scriptMergeForm = new ScriptMergeForm();
            scriptMergeForm.Show();
        }
    }
}
