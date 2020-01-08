using System;
using System.Windows.Forms;
using ff14bot.Managers;
using LlamaLibrary.Reduce;

namespace LlamaLibrary.Reduce
{
    public partial class Settings : Form
    {
        public Settings()
        {
            InitializeComponent();
        }
        
        private void DEindexCheck_CheckedChanged(object sender, EventArgs e)
        {
            ReduceSettings.Instance.IncludeDE10000 = DEindexCheck.Checked;
        }
        
        private void ArmoryCheck_CheckedChanged(object sender, EventArgs e)
        {
            ReduceSettings.Instance.IncludeArmory = ArmoryCheck.Checked;
        }

        private void Settings_Load(object sender, EventArgs e)
        {
            ArmoryCheck.Checked = ReduceSettings.Instance.IncludeArmory;
            DEindexCheck.Checked = ReduceSettings.Instance.IncludeDE10000;
            RunCheck.Checked = ReduceSettings.Instance.StayRunning;
            checkBox1.Checked = ReduceSettings.Instance.AEZoneCheck; //yeah i'll rename it later
            textZone.Text = ReduceSettings.Instance.AEZone.ToString();
            checkBox2.Checked = ReduceSettings.Instance.OpenCoffers;//yeah i'll rename it later...this one too
        }

        private void RunCheck_CheckedChanged(object sender, EventArgs e)
        {
            ReduceSettings.Instance.StayRunning = RunCheck.Checked;
        }

        private void setZoneBtn_Click(object sender, EventArgs e)
        {
            if (textZone.Text != null);
            {
                int _zone;
                if (int.TryParse(textZone.Text, out _zone))
                {
                    ReduceSettings.Instance.AEZone = _zone;
                }
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            ReduceSettings.Instance.AEZoneCheck = checkBox1.Checked;
        }

        private void setCurrentZoneBtn_Click(object sender, EventArgs e)
        {
            ReduceSettings.Instance.AEZone = WorldManager.ZoneId;
            textZone.Text = ReduceSettings.Instance.AEZone.ToString();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            ReduceSettings.Instance.OpenCoffers = checkBox2.Checked;
            ReduceSettings.Instance.Save();
        }
    }
}