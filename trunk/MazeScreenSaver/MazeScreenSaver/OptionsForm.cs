using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MazeScreenSaver
{
    public partial class OptionsForm : Form
    {
        public string[] prgParams;

        public OptionsForm()
        {
            InitializeComponent();
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            if (Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Dungeoneer", "enableLogging", null) != null)
                if ((int)Microsoft.Win32.Registry.GetValue("HKEY_CURRENT_USER\\Software\\Dungeoneer", "enableLogging", null) == 1)
                    check_enableLogging.Checked = true;
                else
                    check_enableLogging.Checked = false;
            else
                check_enableLogging.Checked = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int value;
            if (check_enableLogging.Checked)
                value = 1;
            else
                value = 0;

            Microsoft.Win32.Registry.SetValue("HKEY_CURRENT_USER\\Software\\Dungeoneer", "enableLogging", value);
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
