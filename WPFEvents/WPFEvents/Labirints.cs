using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFEvents
{
    public class Labirints : IPathNode<Object>
    {
        public Int32 Platums { get; set; }
        public Int32 Augstums { get; set; }
        public Boolean Siena { get; set; }

        public bool Gaitenis(Object unused)
        {
            return !Siena;
        }

    }
}
