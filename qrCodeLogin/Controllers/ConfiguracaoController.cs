using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using qrCodeLogin.Models;
using qrCodeLogin.Util;

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
        public JsonResult AlteraDadosUsuario([FromBody] CadUsuario usuario)
        {
            try
            {
                return Json(new { success = true, message = "Dados atualizados com sucesso." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

    }
}
