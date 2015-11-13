using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;
using SvgConverter;

namespace SvgConverterTest
{
    
    public class FileUtilsTests
    {
        [Test]
        public void MakeRelativePath()
        {
            FileUtils.MakeRelativePath(@"C:\Temp\", PathIs.Folder, @"C:\Temp\Sub", PathIs.Folder).Should().Be(@"Sub\");
            FileUtils.MakeRelativePath(@"C:\Temp", PathIs.Folder, @"C:\Temp\Sub", PathIs.Folder).Should().Be(@"Sub\");

            FileUtils.MakeRelativePath(@"C:\Temp", PathIs.Folder, @"C:\Temp\", PathIs.Folder).Should().Be(@".\");

            FileUtils.MakeRelativePath(@"C:\Temp", PathIs.Folder, @"C:\", PathIs.Folder).Should().Be(@"..\");
            FileUtils.MakeRelativePath(@"D:\Projects\SvgToXaml\WpfDemoApp\ImagesC\Svg", PathIs.Folder, @"D:\Projects\SvgToXaml\WpfDemoApp\ImagesC", PathIs.Folder).Should().Be(@"..\");


            FileUtils.MakeRelativePath(@"C:\Temp\", PathIs.Folder, @"C:\Temp\Sub", PathIs.File).Should().Be(@"Sub");
            FileUtils.MakeRelativePath(@"C:\Temp", PathIs.File, @"C:\Temp\Sub", PathIs.Folder).Should().Be(@"Temp\Sub\");

            FileUtils.MakeRelativePath(@"C:\Temp", PathIs.File, @"C:\Temp", PathIs.File).Should().Be(@".\");
            FileUtils.MakeRelativePath(@"C:\Temp", PathIs.Folder, @"C:\Temp", PathIs.Folder).Should().Be(@".\");
        }
    }
}
