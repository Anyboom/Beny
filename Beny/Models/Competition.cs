﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace Beny.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Country { get; set; }
    }
}