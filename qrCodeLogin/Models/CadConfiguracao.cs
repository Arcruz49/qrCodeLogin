using System;
using System.Collections.Generic;

namespace qrCodeLogin.Models;

public partial class CadConfiguracao
{
    public int CdConfiguracao { get; set; }

    public string? ChaveCriptografia { get; set; }
}
