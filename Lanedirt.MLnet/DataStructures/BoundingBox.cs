﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lanedirt.MLnet.DataStructures
{
    /// <summary>
    /// BoundingBox which represents a detected object in an image.
    /// </summary>
    public class BoundingBox
    {
        public int Id { get; set; }
        public string Label { get; set; } = "";
        public int XStart { get; set; }
        public int YStart { get; set; }
        public int XEnd { get; set; }
        public int YEnd { get; set; }
        public float Score { get; set; }
    }
}
