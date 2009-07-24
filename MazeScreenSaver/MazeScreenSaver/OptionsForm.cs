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
        private string registryKey = "HKEY_CURRENT_USER\\Software\\Dungeoneer";

        public OptionsForm()
        {
            InitializeComponent();
        }

        private void setCheckboxFromRegistry(CheckBox check, string valueName)
        {
            object value = Microsoft.Win32.Registry.GetValue(registryKey, valueName, null);
            if (value != null)
            {
                if ((int)value == 1)
                    check.Checked = true;
                else
                    check.Checked = false;
            }
            else
                check.Checked = false;

        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            if (Microsoft.Win32.Registry.GetValue(registryKey, "enableLogging", null) != null)
                if ((int)Microsoft.Win32.Registry.GetValue(registryKey, "enableLogging", null) == 1)
                    check_enableLogging.Checked = true;
                else
                    check_enableLogging.Checked = false;
            else
                check_enableLogging.Checked = false;

            setCheckboxFromRegistry(check_enableLogging, "enableLogging");
            setCheckboxFromRegistry(checkTrails, "enableTrails");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void setRegistryValueFromCheckbox(CheckBox check, string valueName)
        {
            int value;
            if (check.Checked)
                value = 1;
            else
                value = 0;

            Microsoft.Win32.Registry.SetValue(registryKey, valueName, value);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            setRegistryValueFromCheckbox(check_enableLogging, "enableLogging");
            setRegistryValueFromCheckbox(checkTrails, "enableTrails");
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
