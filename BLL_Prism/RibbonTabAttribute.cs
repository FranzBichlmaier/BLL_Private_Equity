using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BLL_Prism
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RibbonTabAttribute: Attribute
    {
        public Type type { get; private set; }
        public RibbonTabAttribute(Type ribbonType)
        {
            type = ribbonType;
        }
    }
}
