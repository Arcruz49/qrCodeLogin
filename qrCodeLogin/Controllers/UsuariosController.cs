using Microsoft.AspNetCore.Mvc;
using qrCodeLogin.Models;
using qrCodeLogin.Models.Resources;

namespace qrCodeLogin.Controllers
{
    public class UsuariosController : Controller
    {
        private dbProjectContext db = new dbProjectContext();

        public IActionResult Index()
        {

            var usuarios = (from a in db.CadUsuarios
                            select a).ToList();

            List<ResourceUsuarios> listaUsuarios = new List<ResourceUsuarios>();

            foreach(var usuario in usuarios)
            {
                ResourceUsuarios resource = new ResourceUsuarios();

                resource.cdUsuario = usuario.CdUsuario;
                resource.nmUsuario = usuario.NmUsuario ?? "";
                resource.email = usuario.Email ?? "";
                resource.dtCriacao = usuario.DtCriacao?.ToString("dd/MM/yyyy");

                listaUsuarios.Add(resource);
            }

            return View(listaUsuarios);
        }
    }
}
