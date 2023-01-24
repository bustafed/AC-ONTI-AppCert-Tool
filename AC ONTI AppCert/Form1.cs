using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace AC_ONTI_AppCert
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        FolderBrowserDialog folderBrowserDialog1 = new FolderBrowserDialog();

        private void button1_Click(object sender, EventArgs e)
        {
            string[] csr;
            string[] key;

            if (validator())
            {
                GeneratePkcs10(textBox1.Text, textBox2.Text, textBox3.Text, textBox4.Text, RootLenght.RootLength2048, out csr, out key);
                MessageBox.Show("CSR y KEY generados con éxito.\n" +
                    "No elimine ni divulgue el archivo KEY.");
            }
            else
            {
                MessageBox.Show("Por favor revise todos los campos");
            }
        }

        private bool validator()
        {
            if (folderBrowserDialog1.SelectedPath.Equals(""))
            {
                return false;
            }
            if (textBox1.Text.Equals("Nombre de la aplicación"))
            {
                return false;
            }
            if (textBox2.Text.Equals("CUIT del Organismo, sin guiones"))
            {
                return false;
            }
            if (!textBox2.Text.Length.Equals(11))
            {
                return false;
            }
            if (textBox3.Text.Equals("Razón social del Organismo"))
            {
                return false;
            }
            if (textBox4.Text.Equals("Nombre del área que solicita el certificado"))
            {
                return false;
            }
            textBox1.Text = textBox1.Text.Trim();
            textBox3.Text = textBox3.Text.Trim();
            textBox4.Text = textBox4.Text.Trim();

            textBox1.Text = textBox1.Text.Replace("  ", " ");
            textBox3.Text = textBox3.Text.Replace("  ", " ");
            textBox4.Text = textBox4.Text.Replace("  ", " ");
            return true;
        }

        private void GeneratePkcs10
            (string commonName, string sn, string org, string orgUnit,
             RootLenght rootLength, out string[] csr, out string[] privateKey)
        {
            csr = new string[3];
            privateKey = new string[3];

            try
            {
                var rsaKeyPairGenerator = new RsaKeyPairGenerator();

                // Note: the numbers {3, 5, 17, 257 or 65537} as Fermat primes.
                // NIST doesn't allow a public exponent smaller than 65537, since smaller exponents are a problem if they aren't properly padded.
                // Note: the default in openssl is '65537', i.e. 0x10001.
                var genParam = new RsaKeyGenerationParameters
                    (BigInteger.ValueOf(0x10001), new SecureRandom(), (int)rootLength, 128);

                rsaKeyPairGenerator.Init(genParam);

                AsymmetricCipherKeyPair pair = rsaKeyPairGenerator.GenerateKeyPair();

                var attributes = new Dictionary<DerObjectIdentifier, string>
                {
                    { X509Name.CN, commonName },
                    { X509Name.SerialNumber, "CUIT " + sn },
                    { X509Name.C, "AR" },
                    { X509Name.O, org },
                    { X509Name.OU, orgUnit }
                };

                var subject = new X509Name(attributes.Keys.ToList(), attributes);

                var pkcs10CertificationRequest = new Pkcs10CertificationRequest
                    (PkcsObjectIdentifiers.Sha256WithRsaEncryption.Id, subject, pair.Public, null, pair.Private);

                csr[0] = "-----BEGIN CERTIFICATE REQUEST-----";
                csr[1] = Convert.ToBase64String(pkcs10CertificationRequest.GetEncoded());
                csr[2] = "-----END CERTIFICATE REQUEST-----";

                TextWriter textWriter = new StringWriter();
                PemWriter pemWriter = new PemWriter(textWriter);
                pemWriter.WriteObject(pair.Private);
                pemWriter.Writer.Flush();

                File.WriteAllText(folderBrowserDialog1.SelectedPath + "/key.key", textWriter.ToString());
                File.WriteAllLines(folderBrowserDialog1.SelectedPath + "/csr.csr", csr);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private enum RootLenght
        {

            RootLength2048 = 2048,

            RootLength3072 = 3072,

            RootLength4096 = 4096,

        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals(""))
            {
                textBox1.Font = new Font(textBox1.Font, FontStyle.Italic);
                textBox1.Text = "Nombre de la aplicación";
            }
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            if (textBox1.Text.Equals("Nombre de la aplicación"))
            {
                textBox1.Text = "";
                textBox1.Font = new Font(textBox1.Font, FontStyle.Regular);
            }
        }

        private void textBox2_Leave(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals(""))
            {
                textBox2.Font = new Font(textBox2.Font, FontStyle.Italic);
                textBox2.Text = "CUIT del Organismo, sin guiones";
            }
            else
            {
                textBox2.Text = textBox2.Text.Replace("-", "");
            }
        }

        private void textBox2_Enter(object sender, EventArgs e)
        {
            if (textBox2.Text.Equals("CUIT del Organismo, sin guiones"))
            {
                textBox2.Text = "";
                textBox2.Font = new Font(textBox2.Font, FontStyle.Regular);
            }
        }

        private void textBox3_Enter(object sender, EventArgs e)
        {
            if (textBox3.Text.Equals("Razón social del Organismo"))
            {
                textBox3.Text = "";
                textBox3.Font = new Font(textBox3.Font, FontStyle.Regular);
            }            
        }

        private void textBox3_Leave(object sender, EventArgs e)
        {
            if (textBox3.Text.Equals(""))
            {
                textBox3.Font = new Font(textBox3.Font, FontStyle.Italic);
                textBox3.Text = "Razón social del Organismo";
            }
        }

        private void textBox2_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void textBox4_Enter(object sender, EventArgs e)
        {
            if (textBox4.Text.Equals("Nombre del área que solicita el certificado"))
            {
                textBox4.Text = "";
                textBox4.Font = new Font(textBox4.Font, FontStyle.Regular);
            }

        }

        private void textBox4_Leave(object sender, EventArgs e)
        {
            if (textBox4.Text.Equals(""))
            {
                textBox4.Font = new Font(textBox4.Font, FontStyle.Italic);
                textBox4.Text = "Nombre del área que solicita el certificado";
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBox5.Font = new Font(textBox5.Font, FontStyle.Regular);
                textBox5.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Form2 form2 = new Form2();
            form2.Show();
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            Regex regex = new Regex("[^0-9.-]+");
            if (regex.IsMatch(textBox2.Text) && !textBox2.Text.Equals("CUIT del Organismo, sin guiones"))
            {
                MessageBox.Show("Por favor ingresar solamente caracteres numéricos");
                textBox2.Text = "";
            }
        }
    }
}
