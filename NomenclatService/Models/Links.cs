﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NomenklatService.Models
{
    public class Links
    {
        public int Id { get; set; }

        public int NomenklatureId { get; set; }

        public int ParentId { get; set; }

        public int Kol { get; set; }
    }
}
