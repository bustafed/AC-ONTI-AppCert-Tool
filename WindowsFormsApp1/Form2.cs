using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Windows.Forms;
using Org.BouncyCastle.OpenSsl;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Drawing;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();
        OpenFileDialog openFileDialog1 = new OpenFileDialog();
        string pathToCert = "";
        string pathToKey = "";
        string pfxName = "";
        bool dllCheck = false;

        public Form2()
        {
            InitializeComponent();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (validator())
            {
                if (textBox4.Text.Equals(textBox5.Text))
                {
                    if (createPFX())
                    {
                        MessageBox.Show("PFX creado correctamente, resguárdelo y no lo distribuya.");
                    }
                }
                else
                {
                    if (textBox5.Text.Equals("Confirmar contraseña"))
                    {
                        MessageBox.Show("Debe ingresar una contraseña");
                    }
                    else
                    {
                        MessageBox.Show("Las contraseñas no coinciden");
                    }
                }
            }
            else
            {
                if (dllCheck)
                {
                    MessageBox.Show("Por favor revise todos los campos");
                }
            }
        }

        private bool createPFX()
        {
            using (X509Certificate2 cert = new X509Certificate2(pathToCert))
            {
                String base64 = File.ReadAllText(pathToKey);

                using (RSA key = RSA.Create())
                {
                    try
                    {
                        using (X509Certificate2 certWithKey = cert.CopyWithPrivateKey(ImportPrivateKey(base64)))
                        {
                            byte[] pkcs12 = certWithKey.Export(X509ContentType.Pfx, textBox4.Text);
                            File.WriteAllBytes(folderBrowserDialog1.SelectedPath + "/" + pfxName + ".pfx", pkcs12);
                        }
                    } catch (Exception e){ 
                        if (e.Message.Equals("The provided key does not match the public key for this certificate.\r\nParameter name: privateKey"))
                        {
                            MessageBox.Show("La clave privada no corresponde al certificado seleccionado.\n" +
                                "Asegúrese de elegir la clave privada que fue generada junto al CSR.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public static RSACryptoServiceProvider ImportPrivateKey(string pem)
        {
            PemReader pr = new PemReader(new StringReader(pem));
            AsymmetricCipherKeyPair KeyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
            RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)KeyPair.Private);

            RSACryptoServiceProvider csp = new RSACryptoServiceProvider();// cspParams);
            csp.ImportParameters(rsaParams);
            return csp;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Seleccionar certificado...";
            openFileDialog1.Filter = "Archivo de certificado (.cer/crt)|*.cer;*.crt";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
                textBox1.Text = openFileDialog1.FileName;
                pathToCert = openFileDialog1.FileName;
                pfxName = openFileDialog1.SafeFileName.Replace(".cer", "");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Title = "Seleccionar KEY...";
            openFileDialog1.Filter = "Archivo .KEY|*.key";
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox2.Font = new Font(textBox2.Font, FontStyle.Regular);
                textBox2.Text = openFileDialog1.FileName;
                pathToKey = openFileDialog1.FileName;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox3.Font = new Font(textBox3.Font, FontStyle.Regular);
                textBox3.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private bool validator()
        {
            foreach (string a in Directory.GetFiles("./"))
            {
                if (a.Contains("BouncyCastle.Crypto.dll"))
                {
                    dllCheck = true;
                    break;
                }
            }
            if (!dllCheck)
            {
                MessageBox.Show("Asegúrese que el archivo BouncyCastle.Crypto.dll se encuentra en la misma carpeta que el ejecutable");
                return false;
            }
            if (folderBrowserDialog1.SelectedPath.Equals(""))
            {
                return false;
            }
            if (textBox1.Text.Equals("Debe seleccionar un archivo"))
            {
                return false;
            }
            if (textBox2.Text.Equals("Debe seleccionar un archivo"))
            {
                return false;
            }

            return true;
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text.Equals("Ingresar contraseña para el PFX"))
            {
                textBox4.Text = "";
                textBox4.Font = new Font(textBox4.Font, FontStyle.Regular);
                textBox4.PasswordChar = '*';
            }
        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text.Equals(""))
            {
                textBox4.PasswordChar = '\0';
                textBox4.Font = new Font(textBox4.Font, FontStyle.Italic);
                textBox4.Text = "Ingresar contraseña para el PFX";
            }
        }


        private void textBox5_Enter(object sender, EventArgs e)
        {
            if (textBox5.Text.Equals("Confirmar contraseña"))
            {
                textBox5.Text = "";
                textBox5.Font = new Font(textBox5.Font, FontStyle.Regular);
                textBox5.PasswordChar = '*';
            }
        }

        private void textBox5_Leave(object sender, EventArgs e)
        {
            if (textBox5.Text.Equals(""))
            {
                textBox5.PasswordChar = '\0';
                textBox5.Font = new Font(textBox5.Font, FontStyle.Italic);
                textBox5.Text = "Ingresar contraseña para el PFX";
            }
        }
    }
}
