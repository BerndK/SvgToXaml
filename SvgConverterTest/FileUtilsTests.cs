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
        [TestCase(@"C:\Temp\", PathIs.Folder, @"C:\Temp\Sub", PathIs.Folder, @"Sub")]
        [TestCase(@"C:\Temp", PathIs.Folder, @"C:\Temp\Sub", PathIs.Folder, @"Sub")]
        [TestCase(@"C:\Temp", PathIs.Folder, @"C:\Temp\", PathIs.Folder, @".")]
        [TestCase(@"C:\Temp", PathIs.Folder, @"C:\", PathIs.Folder, @"..")]
        [TestCase(@"D:\Projects\SvgToXaml\WpfDemoApp\ImagesC\Svg", PathIs.Folder, @"D:\Projects\SvgToXaml\WpfDemoApp\ImagesC", PathIs.Folder, @"..")]
        [TestCase(@"C:\Temp\", PathIs.Folder, @"C:\Temp\Sub", PathIs.File, @"Sub")]
        [TestCase(@"C:\Temp", PathIs.File, @"C:\Temp\Sub", PathIs.Folder, @"Temp\Sub")]
        [TestCase(@"C:\Temp\Sub", PathIs.Folder, @"C:\Temp\file", PathIs.File, @"..\file")]
        [TestCase(@"C:\Temp", PathIs.File, @"C:\Temp", PathIs.File, @".")]
        [TestCase(@"C:\Temp", PathIs.Folder, @"C:\Temp", PathIs.Folder, @".")]
        public void MakeRelativePath(string fromPath, PathIs fromIs, string toPath, PathIs toIs, string result)
        {
            FileUtils.MakeRelativePath(fromPath, fromIs, toPath, toIs).Should().Be(result);
        }
    }
}
