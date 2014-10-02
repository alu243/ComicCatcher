using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using Helpers;
namespace ComicCatcher
{
    public partial class frmEditPathGroup : Form
    {
        private DataTable myTable;
        private SQLiteDataAdapter myAdapter;

        public frmEditPathGroup()
        {
            InitializeComponent();
        }

        private void frmEditPathGroup_Load(object sender, EventArgs e)
        {
            SQLiteHelper.CreatePathGroupTableOnFly();

            myAdapter = SQLiteHelper.GetPathGroupAdapter();
            DataSet ds = new DataSet();
            myAdapter.Fill(ds);
            myTable = ds.Tables[0];

            bindingSource1.DataSource = myTable;
            dgvPathGroup.DataSource = myTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var adapter = SQLiteHelper.GetPathGroupAdapter();
            myAdapter.Update(myTable);
        }

        private void txtFilter_TextChanged(object sender, EventArgs e)
        {
            if (false == String.IsNullOrEmpty(txtFilter.Text))
            {
                bindingSource1.Filter = "GroupName like '%" + txtFilter.Text.Trim() + "%'";
            }
            else
            {
                bindingSource1.Filter = String.Empty;
            }
        }
    }
}
