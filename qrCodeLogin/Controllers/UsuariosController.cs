using Microsoft.AspNetCore.Mvc;
using qrCodeLogin.Models;
using qrCodeLogin.Models.Resources;
using System.Drawing;
using System.Net.Mail;
using System.Net;

namespace qrCodeLogin.Controllers
{
    public class UsuariosController : Controller
    {
        private DbProjectContext db = new DbProjectContext();

        public IActionResult Index()
        {

            

            return View();
        }


        public JsonResult getUsuarios(string search = "")
        {
            var lista = (from a in db.CadUsuarios
                         where (a.NmUsuario.Contains(search) || search == "")
                         select new ResourceUsuarios
                         {
                             cdUsuario = a.CdUsuario,
                             nmUsuario = a.NmUsuario,
                             email = a.Email,
                             dtCriacao = a.DtCriacao.HasValue ? a.DtCriacao.Value.ToString("dd/MM/yyyy") : string.Empty 
                         }).ToList();

            return Json(new { data = lista });
        }


        public JsonResult DeletarUsuario(int cdUsuario = 0)
        {
            if (cdUsuario == 0) return Json(new Retorno<string> { Success = false, Message = "Account not found" });

            try
            {
                var usuario = (from a in db.CadUsuarios
                               where a.CdUsuario == cdUsuario
                               select a).FirstOrDefault();

                if (usuario == null) throw new Exception("Accout not found on Database");

                db.CadUsuarios.Remove(usuario);
                db.SaveChanges();

                return Json(new Retorno<string> { Success = true });

            }
            catch(Exception ex)
            {
                return Json(new Retorno<string> { Success = false, Message = "Error :" + ex.Message });
            }
        }



        

    }
}
