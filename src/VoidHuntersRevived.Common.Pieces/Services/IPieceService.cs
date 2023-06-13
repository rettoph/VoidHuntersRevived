﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VoidHuntersRevived.Common.Entities.Services;

namespace VoidHuntersRevived.Common.Pieces.Services
{
    public interface IPieceService
    {
        Guid Create(PieceType type, Guid id);
    }
}
