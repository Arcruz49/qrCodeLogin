using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using qrCodeLogin.Models;
using System.Net.Mail;
using System.Net;



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
                #region validações do form


                if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                    return Json(new Retorno<string> { Success = false, Message = "Invalid User" });

                if (string.IsNullOrEmpty(username) || string.IsNullOrWhiteSpace(username))
                    return Json(new Retorno<string> { Success = false, Message = "Invalid Password" });

                #endregion

                var user = db.CadUsuarios.FirstOrDefault(a => a.NmUsuario == username);

                if (user == null)
                    return Json(new Retorno<string> { Success = false, Message = "User Not Found" });

                PasswordHasher<CadUsuario> hasher = new PasswordHasher<CadUsuario>();
                var verificationResult = hasher.VerifyHashedPassword(user, user.Senha ?? "", password);

                if (verificationResult != Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success)
                    return Json(new Retorno<string> { Success = false, Message = "Login Failed" });

                var ret = CookiesLogin(user);
                if (!ret.Success) throw new Exception();

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

                var qrCodeImage = Util.Util.GenerateImage(Util.Util.GenerateEncryptedToken(usuario.CdUsuario, usuario.NmUsuario));

                string qrCodeBase64;
                using (var ms = new System.IO.MemoryStream())
                {
                    qrCodeImage.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    qrCodeBase64 = Convert.ToBase64String(ms.ToArray());
                }

                var emailResult = Util.Util.EnviaEmail("Bem-vindo ao sistema", "Seu código QR", qrCodeBase64, email);

                if (!emailResult.Success)
                {
                    return Json(new Retorno<string> { Success = false, Message = "Error sending email: " + emailResult.Message });
                }

                var ret = CookiesLogin(usuario);

                if (!ret.Success) throw new Exception();

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

        [HttpPost]
        public JsonResult LogOnCode(string token = "")
        {
            try
            {

                if(string.IsNullOrEmpty(token)) return Json(new Retorno<string> { Success = false, Message = "Error " + "Token inválido." });

                var tokenDescrip = Util.Util.DecryptToken(token);

                var data = tokenDescrip.Split("/");

                int cdUsuario = Convert.ToInt32(data[0]);
                string nmUsuario = data[1];

                var userName = (from a in db.CadUsuarios
                                where a.CdUsuario == cdUsuario && a.NmUsuario == nmUsuario
                                select a).FirstOrDefault();

                if (userName == null) return Json(new Retorno<string> { Success = false, Message = "Usuário Inválido" });
                
                var ret = CookiesLogin(userName);
                return Json(new Retorno<string> { Success = true, Message = "" });
                
            }
            catch (Exception ex)
            {
                return Json(new Retorno<string> { Success = false, Message = "Error " + ex.Message });
            }
        }


        public Retorno<string> CookiesLogin(CadUsuario user)
        {
            try
            {
                var text = Util.Util.GenerateEncryptedToken(user.CdUsuario, user.NmUsuario);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTime.Now.AddMinutes(60)  // duração do cookie
                };

                HttpContext.Response.Cookies.Append("LoginCookie", text);

                return new Retorno<string> { Success = true, Message = "" };
            }
            catch(Exception ex)
            {
                return new Retorno<string> { Success = false, Message = ex.Message };
            }
        }

        [HttpPost]
        public JsonResult VerificaUsuarioLogado()
        {
            try
            {
                string cookieName = "LoginCookie";

                if (HttpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue))
                {

                    if (!string.IsNullOrEmpty(cookieValue))
                    {
                        var decriptedToken = Util.Util.DecryptToken(cookieValue);

                        var data = decriptedToken.Split("/");

                        int cdUsuario = Convert.ToInt32(data[0]);
                        string nmUsuario = data[1];

                        var userName = (from a in db.CadUsuarios
                                    where a.CdUsuario == cdUsuario
                                    select new
                                    {
                                        a.NmUsuario,
                                        a.CdPerfilUsuario
                                    }).FirstOrDefault();

                        var perfilAdm = userName.CdPerfilUsuario == 1 ? 1 : 0;

                        if(userName.NmUsuario == nmUsuario) return Json(new { Success = true, Message = "User is logged in", Permission = perfilAdm });
                    }
                }

                return Json(new { Success = false, Message = "User is not logged in" });
            }
            catch (Exception ex)
            {
                return Json(new { Success = false, Message = "Error: " + ex.Message });
            }
        }

        

    }
}
