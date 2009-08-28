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
            setCheckboxFromRegistry(check_enableLogging, "enableLogging");
            setCheckboxFromRegistry(checkTrails, "enableTrails");
            int? speed = getRegistryIntValue("speed");
            if (!speed.HasValue)
                speed = 5;
            txtSpeed.Text = speed.ToString();
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

        private void setRegistryIntValue(int value, string valueName)
        {
            Microsoft.Win32.Registry.SetValue(registryKey, valueName, value);
        }

        private int? getRegistryIntValue(string valueName)
        {
            return (int?)Microsoft.Win32.Registry.GetValue(registryKey, valueName, null);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtSpeed.Text.Trim() == "" || Convert.ToInt32(txtSpeed.Text) < 0)
            {
                MessageBox.Show("Please enter a positive integer for the speed.");
                return;
            }
            int speed = Convert.ToInt32(txtSpeed.Text);
            setRegistryValueFromCheckbox(check_enableLogging, "enableLogging");
            setRegistryValueFromCheckbox(checkTrails, "enableTrails");
            setRegistryIntValue(speed, "speed");
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

    }
}
