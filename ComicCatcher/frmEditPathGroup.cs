using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ComicModels;
using Helpers;
namespace ComicCatcher
{
    public partial class frmEditPathGroup : Form
    {
        private DataTable myTable;
        private SQLiteDataAdapter myAdapter;

        public frmEditPathGroup(SettingEnum setting)
        {
            if (setting == SettingEnum.IgnoreComic)
            {
                SQLiteHelper.CreateIgnoreComicTableOnFly();
                myAdapter = SQLiteHelper.GetIgnoreComicDataAdapter();
                DataSet ds = new DataSet();
                myAdapter.Fill(ds);
                myTable = ds.Tables[0];
            }
            else
            {
                SQLiteHelper.CreatePathGroupTableOnFly();
                myAdapter = SQLiteHelper.GetPathGroupDataAdapter();
                DataSet ds = new DataSet();
                myAdapter.Fill(ds);
                myTable = ds.Tables[0];
            }
            InitializeComponent();
        }

        private void frmEditPathGroup_Load(object sender, EventArgs e)
        {

            bindingSource1.DataSource = myTable;
            dgvPathGroup.DataSource = myTable;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            myAdapter.Update(myTable);
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
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
