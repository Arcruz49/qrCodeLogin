using System;
using System.Collections.Generic;

namespace qrCodeLogin.Models;

public partial class CadUsuario
{
    public int CdUsuario { get; set; }

    public string? NmUsuario { get; set; }

    public string? Email { get; set; }

    public int? CdPerfilUsuario { get; set; }

    public string? Senha { get; set; }

    public int? UsuarioC { get; set; }

    public DateTime? DtCriacao { get; set; }

    public int? UsuarioA { get; set; }

    public DateTime? DtAlteracao { get; set; }

    public string? Site { get; set; }

    public string? Github { get; set; }

    public string? Twitter { get; set; }

    public string? Instagram { get; set; }

    public string? Facebook { get; set; }

    public string? Telefone1 { get; set; }

    public string? Telefone2 { get; set; }

    public virtual CadPerfil? CdPerfilUsuarioNavigation { get; set; }
}
