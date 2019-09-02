using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SvgConverterTest
{
    public class SvgzTest
    {
        [Test]
        public void TestUnzip()
        {
            var fs = File.OpenRead(@".\TestFiles\example.svgz");
            var stream = new System.IO.Compression.GZipStream(fs, CompressionMode.Decompress);
            var  destination = File.OpenWrite(@".\TestFiles\example.svg");
            stream.CopyTo(destination);
            
        }
    }
}
