using System;
using System.Windows.Forms;

namespace LlamaLibrary
{
    public partial class AutoFollowSettingsFrm : Form
    {
        public AutoFollowSettingsFrm()
        {
            InitializeComponent();
        }


        private void AutoFollowSettingsFrm_Load(object sender, EventArgs e)
        {
            //bsFollowTarget.DataSource = AutoFollowSettings.Instance.IsPaused;

            checkBox1.Checked = AutoFollowSettings.Instance.IsPaused;
            if (AutoFollowSettings.Instance.FollowLeader)
                radioButton1.Checked = true;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            AutoFollowSettings.Instance.IsPaused = checkBox1.Checked;
        }
    }
}