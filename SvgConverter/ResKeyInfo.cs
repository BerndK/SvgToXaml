using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SvgConverter
{
    public class ResKeyInfo
    {
        public string Name { get; set; }
        public string XamlName { get; set; }
        public string Prefix { get; set; }
        public bool UseComponentResKeys { get; set; }
        public bool BuildStaticResources { get; set; }
        public string NameSpace { get; set; }
        public string NameSpaceName { get; set; }
    }
}
