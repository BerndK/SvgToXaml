using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using NUnit.Framework;

namespace SvgConverterTest
{
    public class T4Methods
    {
        [Test]
        public void Test_KeysFromXaml()
        {
            string nameSpaceName;
            string prefix;
            var keys = KeysFromXaml(@"TestFiles\Expected\SvgDirToXamlTest_withUseCompResKey.xaml", out nameSpaceName, out prefix);
            Console.WriteLine($"NS:{nameSpaceName}, Prefix:{prefix}");
            foreach (var key in keys)
            {
                Console.WriteLine(key);
            }
        }

        public static IEnumerable<string> KeysFromXaml(string fileName, out string nameSpace, out string prefix)
        {
            var doc = XDocument.Load(fileName);
            //var allElems = doc.Root.Elements(); //doc.Descendants(); das wären alle samt SubNodes
            XNamespace xamlNs = "http://schemas.microsoft.com/winfx/2006/xaml";
            //var keyAttrs = allElems.Attributes(xamlNs+"Key");


            //Console.WriteLine(keyAttrs.Count());
            //foreach (var attr in keyAttrs)
            //{
            //    Console.WriteLine(attr.Name);
            //}
            nameSpace = doc.Root.LastAttribute.Value; //hoffentlich ist es immer das letzte, aber nach Namen suchen is nich, und andere ausschließen ist auch nicht besser
            var keys = doc.Root.Elements().Attributes(xamlNs + "Key").Select(a => a.Value).ToArray();
            //keys liegen in dieser Form vor: { x: Static NameSpaceName:XamlName.Color1}

            prefix = "unknownPrefix";
            var first = keys.FirstOrDefault();
            if (first != null)
            {
                var p1 = first.LastIndexOf(":");
                var p2 = first.LastIndexOf("}");
                if (p1 < p2)
                    prefix = first.Substring(p1 + 1, p2 - p1 - 1).Split('.').FirstOrDefault();
            }

            var names = keys.Select(key =>
            {
                var p1 = key.LastIndexOf(".");
                var p2 = key.LastIndexOf("}");
                if (p1 < p2)
                    return key.Substring(p1 + 1, p2 - p1 - 1);
                else
                    return key;
            }).ToArray();


            return names;
        }
    }
}
