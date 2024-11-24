using System;
using System.Collections.Generic;

namespace qrCodeLogin.Models;

public partial class CadPublicacao
{
    public int CdPublicacao { get; set; }

    public string? DsPublicacao { get; set; }

    public int? CdUsuario { get; set; }

    public DateTime? DtCriacao { get; set; }

    public DateTime? DtDeletado { get; set; }

    public int? CdUsuarioAlt { get; set; }

    public byte[]? Imagem { get; set; }

    public virtual CadUsuario? CdUsuarioAltNavigation { get; set; }

    public virtual CadUsuario? CdUsuarioNavigation { get; set; }
}
