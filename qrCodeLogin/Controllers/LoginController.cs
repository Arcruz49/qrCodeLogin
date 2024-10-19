using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using qrCodeLogin.Models;

namespace qrCodeLogin.Controllers
{
    public class LoginController : Controller
    {
        private dbProjectContext db = new dbProjectContext();

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
                var verificationResult = hasher.VerifyHashedPassword(user, user.Senha, password);

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

                return Json(new Retorno<string> { Success = true, Message = ""});


            }
            catch (Exception ex)
            {
                return Json(new Retorno<string> { Success = false, Message = "Error Trying To Create Account "+ ex.Message });
            }
        }
    }
}
