﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BExIS.Dim.Entities.Responses
{
    public class VaelastraszResponse : IResponse
    {
        public string DOI { get; set; }

        public Status Status { get; set; }
    }

    public enum Status
    {

    }
}
