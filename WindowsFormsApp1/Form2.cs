using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Windows.Forms;
using Org.BouncyCastle.OpenSsl;
using System.IO;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public Form2()
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            using (X509Certificate2 cert = new X509Certificate2("C:/Users/Innovacion/Desktop/tsa.cer"))
            {
                String base64 = File.ReadAllText("C:/Users/Innovacion/Desktop/keytsa.key");

                using (RSA key = RSA.Create())
                {
                    using (X509Certificate2 certWithKey = cert.CopyWithPrivateKey(ImportPrivateKey(base64)))
                    {
                        byte[] pkcs12 = certWithKey.Export(X509ContentType.Pfx, "test1234");
                        File.WriteAllBytes("C:/Users/Innovacion/Desktop/tsa.pfx", pkcs12);
                    }
                }
            }
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
    }
}
