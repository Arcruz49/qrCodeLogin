using System;
using System.Collections.Generic;

namespace qrCodeLogin.Models;

public partial class CadPerfil
{
    public int CdPerfil { get; set; }

    public string? NmPerfil { get; set; }

    public int? UsuarioC { get; set; }

    public DateTime? DtCriacao { get; set; }

    public int? UsuarioA { get; set; }

    public DateTime? DtAlteracao { get; set; }

    public virtual ICollection<CadUsuario> CadUsuarios { get; set; } = new List<CadUsuario>();
}
