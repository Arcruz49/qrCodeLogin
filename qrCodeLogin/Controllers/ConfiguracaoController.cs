using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qrCodeLogin.Models;
using qrCodeLogin.Util;
using System.Security.Policy;

namespace qrCodeLogin.Controllers
{
    public class ConfiguracaoController : Controller
    {
        private Util.Util util;
        private DbProjectContext db = new DbProjectContext();
        public IActionResult Index()
        {
            int cdUsuario = GetCdUsuarioLogado();

            var usuario = (from a in db.CadUsuarios
                           where a.CdUsuario == cdUsuario
                           select a).FirstOrDefault();

            var perfil = (from a in db.CadPerfil
                          where usuario.CdPerfilUsuario == a.CdPerfil
                          select a).FirstOrDefault();

            ViewBag.nmPerfil = perfil.NmPerfil;

            return View(usuario);
        }

        public int GetCdUsuarioLogado()
        {
            string cookieName = "LoginCookie";

            if (!HttpContext.Request.Cookies.TryGetValue(cookieName, out string cookieValue))
                return -1;

            var tokenDescrip = Util.Util.DecryptToken(cookieValue);

            var data = tokenDescrip.Split("/");

            int cdUsuario = Convert.ToInt32(data[0]);
            string nmUsuario = data[1];

            var user = (from a in db.CadUsuarios
                        where a.CdUsuario == cdUsuario
                        select a).FirstOrDefault();

            return user != null ? user.CdUsuario : -1; 
        }


        [HttpPost]
        public JsonResult AlteraDadosUsuario(CadUsuario usuario, IFormFile ProfileImage)
        {
            try
            {
                var usuarioAlt = db.CadUsuarios.FirstOrDefault(a => a.CdUsuario == usuario.CdUsuario);

                if (usuarioAlt == null)
                    return Json(new { success = false, message = "No user found" });

                usuarioAlt.Site = usuario.Site;
                usuarioAlt.Facebook = usuario.Facebook;
                usuarioAlt.Instagram = usuario.Instagram;
                usuarioAlt.Github = usuario.Github;
                usuarioAlt.Twitter = usuario.Twitter;
                usuarioAlt.Telefone1 = usuario.Telefone1;
                usuarioAlt.Telefone2 = usuario.Telefone2;
                usuarioAlt.Ocupacao = usuario.Ocupacao;
                usuarioAlt.DtAlteracao = DateTime.Now;

                if (ProfileImage != null)
                {
                    using (var ms = new MemoryStream())
                    {
                        ProfileImage.CopyTo(ms);
                        usuarioAlt.ProfileImage = ms.ToArray();
                    }
                }

                db.Entry(usuarioAlt).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new { success = true, message = "Dados atualizados com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


    }
}
