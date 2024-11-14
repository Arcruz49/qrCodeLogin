using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using qrCodeLogin.Models;
using System.Net.Mail;
using System.Net;
using qrCodeLogin.Util;

namespace qrCodeLogin.Controllers
{
    public class LoginController : Controller
    {
        private DbProjectContext db = new DbProjectContext();

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult LogOn(string username = "", string password = "")
        {

            try
            {
                LogOnCode();
                #region validações do form


                if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                    return Json(new Retorno<string> { Success = false, Message = "Invalid User" });

                if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                    return Json(new Retorno<string> { Success = false, Message = "Invalid Password" });

                #endregion

                var user = db.CadUsuarios.FirstOrDefault(a => a.NmUsuario == username);

                CadUsuario cadUsuario = new CadUsuario();
                

                if (user == null)
                    return Json(new Retorno<string> { Success = false, Message = "User Not Found" });


                PasswordHasher<CadUsuario> hasher = new PasswordHasher<CadUsuario>();
                var verificationResult = hasher.VerifyHashedPassword(user, user.Senha ?? "", password);

                if (verificationResult != PasswordVerificationResult.Success)
                    return Json(new Retorno<string> { Success = false, Message = "Login Failed" });

                return Json(new Retorno<string> { Success = true, Message = "" });

            }
            catch (Exception ex)
            {
                return Json(new Retorno<string> { Success = false, Message = "Error " + ex.Message });

            }

        }

        [HttpPost]
        public JsonResult Register(string username = "", string email = "", string password1 = "", string password2 = "")
        {
            try
            {
                #region validações do form
                if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                    return Json(new Retorno<string> { Success = false, Message = "Ivalid User Name" });
                if (string.IsNullOrEmpty(email) || string.IsNullOrWhiteSpace(email))
                    return Json(new Retorno<string> { Success = false, Message = "Ivalid E-mail" });

                if (!password1.Equals(password2))
                    return Json(new Retorno<string> { Success = false, Message = "Passwords Do Not Match" });

                #endregion


                if (db.CadUsuarios.Any(a => a.NmUsuario == username))
                    return Json(new Retorno<string> { Success = false, Message = "User Name Already Taken" });

                PasswordHasher<CadUsuario> hasher = new PasswordHasher<CadUsuario>();

                var usuario = new CadUsuario
                {
                    NmUsuario = username,
                    Email = email,
                    DtCriacao = DateTime.Now,
                    CdPerfilUsuario = 2
                };

                usuario.Senha = hasher.HashPassword(usuario, password1);


                db.Add(usuario);
                db.SaveChanges();


                var qrCodeImage = QrCodeUtil.GenerateImage(QrCodeUtil.GenerateEncryptedToken(usuario.CdUsuario, usuario.NmUsuario));

                string qrCodeBase64;
                using (var ms = new System.IO.MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    qrCodeBase64 = Convert.ToBase64String(ms.ToArray());
                }

                var emailResult = QrCodeUtil.EnviaEmail("Bem-vindo ao sistema", "Seu código QR", qrCodeBase64, email);

                if (!emailResult.Success)
                {
                    return Json(new Retorno<string> { Success = false, Message = "Error sending email: " + emailResult.Message });
                }


                return Json(new Retorno<string> { Success = true, Message = ""});


            }
            catch (Exception ex)
            {
                return Json(new Retorno<string> { Success = false, Message = "Error Trying To Create Account "+ ex.Message });
            }
        }


        public void EnviarEmailComQRCode(string email, string qrCodeBase64)
        {
            var subject = "Seu QR Code";
            var body = $"<h1>Seu QR Code</h1><p>Use o QR Code abaixo:</p><img src='data:image/png;base64,{qrCodeBase64}' />";

            using (var smtpClient = new SmtpClient("arthurcruzdbp@gmail.com", 587))
            {
                smtpClient.Credentials = new NetworkCredential("arthurcruzdbp@gmail.com", "Myidealsmementos420");
                smtpClient.EnableSsl = true;

                var mailMessage = new MailMessage
                {
                    From = new MailAddress("arthurcruzdbp@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                smtpClient.Send(mailMessage);
            }
        }


        public JsonResult LogOnCode(string token = "YmL5a9T44sEAKlL+1YE7Jw==:HFxTy/F/pIMZms5KAKuT9Q==")
        {
            try
            {

                if(string.IsNullOrEmpty(token)) return Json(new Retorno<string> { Success = false, Message = "Error " + "Token inválido." });

                var tokenDescrip = QrCodeUtil.DecryptToken(token);

                return Json(new Retorno<string> { Success = true, Message = "" });
            }
            catch (Exception ex)
            {
                return Json(new Retorno<string> { Success = false, Message = "Error " + ex.Message });
            }
        }
    }
}
