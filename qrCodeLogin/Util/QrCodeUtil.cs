using qrCodeLogin.Models;
using QRCoder;
using System.Drawing;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;
using System.Security.Cryptography;
using System.Text;

namespace qrCodeLogin.Util
{
    public class QrCodeUtil
    {
        private static DbProjectContext db = new DbProjectContext();

        public static Bitmap GenerateImage(string text)
        {
            var qrGenerator = new QRCodeGenerator();
            var qrCodeData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.H);
            var qrCode = new QRCode(qrCodeData);
            var qrCodeImage = qrCode.GetGraphic(10);
            return qrCodeImage;
        }

        public static Retorno<string> EnviaEmail(string message, string title, string qrCodeBase64, string destinatario)
        {
            try
            {
                var fromAddress = new MailAddress("senderqrcode2@gmail.com", "QrCode Sender");
                var toAddress = new MailAddress(destinatario);
                const string fromPassword = "kwml hbog mkbc kjuw"; 

                var smtp = new SmtpClient
                {
                    Host = "smtp.gmail.com", 
                    Port = 587,
                    EnableSsl = true,
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
                };

                using (var mailMessage = new MailMessage(fromAddress, toAddress)
                {
                    Subject = title,
                    Body = message,
                    IsBodyHtml = true
                })
                {
                    byte[] imageBytes = Convert.FromBase64String(qrCodeBase64);
                    using (var stream = new MemoryStream(imageBytes))
                    {
                        var attachment = new Attachment(stream, "qrcode.png", MediaTypeNames.Image.Png);
                        mailMessage.Attachments.Add(attachment);

                        smtp.Send(mailMessage);
                    }
                }

                return new Retorno<string> { Success = true };
            }
            catch (Exception ex)
            {
                return new Retorno<string> { Success = false, Message = $"Error while sending E-mail: {ex.Message}" };
            }
        }


        public static string GenerateEncryptedToken(int cdUsuario, string NmUsuario = "")
        {
            string token = $"{cdUsuario}/{NmUsuario}";

            var key = (from a in db.CadConfiguracao
                       select a.ChaveCriptografia).FirstOrDefault();


            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.GenerateIV(); 
                var iv = aes.IV;

                using (var encryptor = aes.CreateEncryptor(aes.Key, iv))
                {
                    var textBytes = Encoding.UTF8.GetBytes(token);
                    var encryptedBytes = encryptor.TransformFinalBlock(textBytes, 0, textBytes.Length);

                    var result = Convert.ToBase64String(iv) + ":" + Convert.ToBase64String(encryptedBytes);
                    return result;
                }
            }
        }


        public static string DecryptToken(string encryptedText)
        {
            var key = (from a in db.CadConfiguracao
                       select a.ChaveCriptografia).FirstOrDefault();

            var parts = encryptedText.Split(':');
            var iv = Convert.FromBase64String(parts[0]);
            var encryptedBytes = Convert.FromBase64String(parts[1]);

            using (var aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;

                using (var decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    var decryptedBytes = decryptor.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length);
                    return Encoding.UTF8.GetString(decryptedBytes);
                }
            }
        }

    }
}