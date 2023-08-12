using ComicCatcher.App_Code.DbModel;
using ComicModels;
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
                IgnoreComicDao.CreateTableOnFly();
                myTable = IgnoreComicDao.GetTable();
            }
            else //if(this.setting == SettingEnum.PathGroup)
            {
                PathGroupDao.CreateTableOnFly();
                myTable = PathGroupDao.GetTable();
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
