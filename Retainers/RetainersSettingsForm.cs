using System;
using System.Windows.Forms;


namespace LlamaLibrary.Retainers
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            propertyGrid1.SelectedObject = RetainerSettings.Instance;
        }
    }
}
