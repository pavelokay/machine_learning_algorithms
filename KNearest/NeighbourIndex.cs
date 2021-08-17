using System;
using System.Collections.Generic;
using System.Text;

namespace KNearest
{
    public class NeighbourIndex : IComparable<NeighbourIndex>
    {
        public int Index { get; set; }
        public double Distance { get; set; }
        public int Classification { get; set; }

        public int CompareTo(NeighbourIndex other)
        {
            if (this.Distance < other.Distance) return -1;
            else if (this.Distance > other.Distance) return +1;
            else return 0;
        }
    }
}
