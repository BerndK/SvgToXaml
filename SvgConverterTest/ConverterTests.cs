using System;
using System.Windows;
using System.Windows.Media;
using NUnit.Framework;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;
using SvgConverter;

namespace SvgConverterTest
{
    [TestFixture]
    public class ConverterTests
    {
        [TestCase("..\\..\\TestFiles\\cloud-3-icon.svg")]
        [TestCase("..\\..\\TestFiles\\cloud-4-icon.svg")]
        [TestCase("..\\..\\TestFiles\\JOG.svg")]
        public void ConvertFileToDrawingGroup(string filename)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                //WriteAsRoot = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingGroup , settings);
            Console.WriteLine(xaml);
        }

        [TestCase("..\\..\\TestFiles\\cloud-3-icon.svg")]
        [TestCase("..\\..\\TestFiles\\cloud-4-icon.svg")]
        [TestCase("..\\..\\TestFiles\\JOG.svg")]
        public void ConvertFileToDrawingImage(string filename)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                //WriteAsRoot = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingImage , settings);
            Console.WriteLine(xaml);
        }

        [TestCase("..\\..\\TestFiles\\cloud-3-icon.svg")]
        [TestCase("..\\..\\TestFiles\\JOG.svg")]
        public void ConvertFileToDrawingGroupWithRuntime(string filename)
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = true,
                TextAsGeometry = false,
                //WriteAsRoot = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingGroup, settings);
            Console.WriteLine(xaml);
        }

        [Test]
        public void SvgDirToXamlTest()
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                //WriteAsRoot = false,
                OptimizePath = true,
            };
            var xaml = ConverterLogic.SvgDirToXaml("..\\..\\TestFiles\\", "Test", settings);
            Console.WriteLine(xaml);
        }

        [Test]
        public void SvgDirToXaml_with_defaultSettingsTest()
        {
            var xaml = ConverterLogic.SvgDirToXaml("..\\..\\TestFiles\\", "Test", null);
            Console.WriteLine(xaml);
        }

        [Test, STAThread]
        public void Handwheel() //n.i.O
        {
            var settings = new WpfDrawingSettings
            {
                //IncludeRuntime = false,
                //TextAsGeometry = false,
                //WriteAsRoot = false,
                //OptimizePath = true,
            };

            var xaml = ConverterLogic.SvgFileToXaml("..\\..\\TestFiles\\Handwheel.svg", ResultMode.DrawingGroup, settings);
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
        [Test, STAThread]
        public void Handwheel1() //i.O
        {
            var settings = new WpfDrawingSettings
            {
                //IncludeRuntime = false,
                //TextAsGeometry = false,
                //WriteAsRoot = false,
                //OptimizePath = true,
            };
            var fileReader = new FileSvgReader(settings);
            DrawingGroup drawing = fileReader.Read("..\\..\\TestFiles\\Handwheel.svg");
            XmlXamlWriter writer = new XmlXamlWriter(settings);
            var xaml = writer.Save(drawing);
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
        [Test, STAThread]
        public void Handwheel2() //i.O.
        {
            var settings = new WpfDrawingSettings
            {
                //IncludeRuntime = false,
                //TextAsGeometry = false,
                //WriteAsRoot = false,
                //OptimizePath = true,
            };
            var drawing = SvgConverter.ConverterLogic.SvgFileToWpfObject("..\\..\\TestFiles\\Handwheel.svg", settings);
            XmlXamlWriter writer = new XmlXamlWriter(settings);
            var xaml = writer.Save(drawing);
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
        [Test, STAThread]
        public void Handwheel3() //i.O.
        {
            var settings = new WpfDrawingSettings
            {
                //IncludeRuntime = false,
                //TextAsGeometry = false,
                //WriteAsRoot = false,
                //OptimizePath = true,
            };
            var drawing = ConverterLogic.SvgFileToWpfObject("..\\..\\TestFiles\\Handwheel.svg", settings);

            var xaml = ConverterLogic.SvgObjectToXaml(drawing, true, "Test");
            Console.WriteLine(xaml);
            Clipboard.SetText(xaml);
        }
    }
}
