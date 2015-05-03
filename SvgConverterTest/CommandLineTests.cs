using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using SvgConverter;

namespace SvgConverterTest
{
    [TestFixture]
    public class CommandLineTests
    {
        [Test]
        public void TestCreateXaml()
        {
            CmdLineHandler.HandleCommandLine(new[] {"BuildDict", "/inputdir:..\\..\\TestFiles", "/outputname:images", "/outputdir:."});
            File.Exists("images.xaml").Should().BeTrue();
        }
    }
}
