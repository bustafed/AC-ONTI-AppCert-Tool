using System;
using System.Drawing;
using System.Runtime.ConstrainedExecution;
using System.Windows.Forms;
using static AC_ONTI_AppCert.Form1;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace AC_ONTI_AppCert
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(textBox2.Text))
            {
                passDiagComplete = true;
                pKeyPass = textBox1.Text;
                this.Hide();
            }
            else
            {
                if (textBox2.Text.Equals("Confirmar contraseña"))
                {
                    MessageBox.Show("Debe ingresar una contraseña");
                }
                else
                {
                    MessageBox.Show("Las contraseñas no coinciden");
                }
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("Ingresar contraseña"))
            {
                textBox1.Text = "";
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
                textBox1.PasswordChar = '*';
            }
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                textBox1.PasswordChar = '\0';
                textBox1.Font = new Font(textBox1.Font, FontStyle.Italic);
                textBox1.Text = "Ingresar contraseña";
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals("Confirmar contraseña"))
            {
                textBox2.Text = "";
                textBox2.Font = new Font(textBox2.Font, FontStyle.Regular);
                textBox2.PasswordChar = '*';
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals(""))
            {
                textBox2.PasswordChar = '\0';
                textBox2.Font = new Font(textBox2.Font, FontStyle.Italic);
                textBox2.Text = "Confirmar contraseña";
            }
        }

    }
}
