using System;
using System.Drawing;
using System.Windows.Forms;

namespace AC_ONTI_AppCert
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void Form4_Load(object sender, EventArgs e)
        {
            Globals.pKeyPass = "";
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
                textBox1.PasswordChar = '*';
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                textBox1.Enabled = false;
                textBox1.Text = "";
            }
            else
            {
                textBox1.Enabled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("") && !checkBox1.Checked)
            {
                MessageBox.Show("Debe ingresar una contraseña");
            }
            else
            {
                Globals.pKeyPass= textBox1.Text;
                this.Close();
            }
        }

        private void Form4_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (textBox1.Text.Equals("") && !checkBox1.Checked)
            {
                Globals.pKeyPass = "";
            }
        }
    }
}
