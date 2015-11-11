using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using FluentAssertions;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SvgConverter;

namespace SvgConverterTest
{
    [TestFixture]
    public class ConverterTests
    {
        [TestCase("TestFiles\\cloud-3-icon.svg")]
        [TestCase("TestFiles\\3d-view-icon.svg")]
        [TestCase("TestFiles\\JOG.svg")]
        public void ConvertFileToDrawingGroup(string filename)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingGroup, "Prefix", settings);
            Console.WriteLine(xaml);
        }

        [Test]
        public void ConvertFileToDrawingGroup2()
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml("TestFiles\\3d-view-icon.svg", ResultMode.DrawingGroup, "Prefix", settings);
            Console.WriteLine(xaml);
            string expected = File.ReadAllText("TestFiles\\3d-view-icon_expected.txt");
            xaml.Should().Be(expected);
        }

        [TestCase("TestFiles\\cloud-3-icon.svg")]
        [TestCase("TestFiles\\3d-view-icon.svg")]
        [TestCase("TestFiles\\JOG.svg")]
        public void ConvertFileToDrawingImage(string filename)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingImage, "Prefix", settings);
            Console.WriteLine(xaml);
        }

        [TestCase("TestFiles\\cloud-3-icon.svg")]
        [TestCase("TestFiles\\JOG.svg")]
        public void ConvertFileToDrawingGroupWithRuntime(string filename)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = true,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingGroup, "Prefix", settings);
            Console.WriteLine(xaml);
        }

        [Test]
        public void SvgDirToXamlTest()
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgDirToXaml("TestFiles\\", "Test", "NamePrefix", settings);
            Console.WriteLine(xaml);
        }

        [Test]
        public void SvgDirToXaml_with_defaultSettingsTest()
        {
            var xaml = ConverterLogic.SvgDirToXaml("TestFiles\\", "Test", "NamePrefix", null);
            Console.WriteLine(xaml);
        }

        [Test, STAThread]
        public void Handwheel() //Full integrated with all optimizations
        {
            var xaml = ConverterLogic.SvgFileToXaml("TestFiles\\Handwheel.svg", ResultMode.DrawingGroup, "Prefix", null);
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
        [Test, STAThread]
        public void Handwheel1() //pure svg# without any modifications
        {
            var fileReader = new FileSvgReader(null);
            DrawingGroup drawing = fileReader.Read("TestFiles\\Handwheel.svg");
            XmlXamlWriter writer = new XmlXamlWriter(null);
            var xaml = writer.Save(drawing);
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
        [Test, STAThread]
        public void Handwheel2() //integrated conversion, manual writing
        {
            var drawing = SvgConverter.ConverterLogic.SvgFileToWpfObject("TestFiles\\Handwheel.svg", null);
            XmlXamlWriter writer = new XmlXamlWriter(null);
            var xaml = writer.Save(drawing);
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
        [Test, STAThread]
        public void Handwheel3() //integrated conversion, integrated writing
        {
            var drawing = ConverterLogic.SvgFileToWpfObject("TestFiles\\Handwheel.svg", null);
            var xaml = ConverterLogic.SvgObjectToXaml(drawing, true, "Test");
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
    }
}
