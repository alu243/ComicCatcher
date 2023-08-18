using ComicCatcherLib.DbModel;
using ComicCatcherLib.Models;
using System;
using System.Data;
using System.Windows.Forms;

namespace ComicCatcher
{
    public partial class ComicSettingsGroup : Form
    {
        private DataTable myTable;
        private SettingEnum setting = SettingEnum.IgnoreComic;
        public ComicSettingsGroup(SettingEnum setting)
        {
            this.setting = setting;
            if (this.setting == SettingEnum.IgnoreComic)
            {
                IgnoreComicDao.CreateTableOnFly().Wait();
                myTable = IgnoreComicDao.GetTable().Result;
            }
            else //if(this.setting == SettingEnum.PathGroup)
            {
                PathGroupDao.CreateTableOnFly().Wait();
                myTable = PathGroupDao.GetTable().Result;
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
            if (setting == SettingEnum.IgnoreComic)
            {
                IgnoreComicDao.ApplyChangesToDatabase(this.myTable);
            }
            else
            {
                PathGroupDao.ApplyChangesToDatabase(this.myTable);
            }
        }

        private void TxtFilter_TextChanged(object sender, EventArgs e)
        {
            if (this.setting == SettingEnum.PathGroup)
            {
                if (String.IsNullOrEmpty(txtFilter.Text))
                {
                    bindingSource1.Filter = String.Empty;
                }
                else
                {
                    bindingSource1.Filter = "GroupName like '%" + txtFilter.Text.Trim() + "%'";
                }
            }
        }
    }
}
