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
            var resKeyInfo = new ResKeyInfo {Prefix = "Prefix"};
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingGroup, resKeyInfo, settings);
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
            var resKeyInfo = new ResKeyInfo { Prefix = "Prefix" };
            var xaml = ConverterLogic.SvgFileToXaml("TestFiles\\3d-view-icon.svg", ResultMode.DrawingGroup, resKeyInfo, settings);
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
            var resKeyInfo = new ResKeyInfo { Prefix = "Prefix" };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingImage, resKeyInfo, settings);
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
            var resKeyInfo = new ResKeyInfo { Prefix = "Prefix" };
            var xaml = ConverterLogic.SvgFileToXaml(filename, ResultMode.DrawingGroup, resKeyInfo, settings);
            Console.WriteLine(xaml);
        }

        [Test]
        public void SvgDirToXamlTest_withNamePrefix()
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            var resKeyInfo = new ResKeyInfo { XamlName = "Test", Prefix = "NamePrefix" };
            var xaml = ConverterLogic.SvgDirToXaml("TestFiles\\", resKeyInfo, settings);
            Console.WriteLine(xaml);
            //File.WriteAllText("TestFiles\\ExpectedXaml_SvgDirToXamlTest_withNamePrefix.xaml", xaml);
            var expectedXaml = File.ReadAllText("TestFiles\\ExpectedXaml_SvgDirToXamlTest_withNamePrefix.xaml");
            xaml.Should().Be(expectedXaml);
        }

        [Test]
        public void SvgDirToXamlTest_withUseCompResKey()
        {
            var settings = new WpfDrawingSettings
            {
                IncludeRuntime = false,
                TextAsGeometry = false,
                OptimizePath = true,
            };
            
            var xaml = ConverterLogic.SvgDirToXaml("TestFiles\\", ResKeyInfoUseCompResKey, settings);
            Console.WriteLine(xaml);
            File.WriteAllText("TestFiles\\ExpectedXaml_SvgDirToXamlTest_withUseCompResKey.xaml", xaml);
            var expectedXaml = File.ReadAllText("TestFiles\\ExpectedXaml_SvgDirToXamlTest_withUseCompResKey.xaml");
            xaml.Should().Be(expectedXaml);
        }

        [Test]
        public void SvgDirToXaml_with_defaultSettingsTest()
        {
            var resKeyInfo = new ResKeyInfo { XamlName = "Test", Prefix = "NamePrefix" };
            var xaml = ConverterLogic.SvgDirToXaml("TestFiles\\", resKeyInfo, null);
            Console.WriteLine(xaml);
        }

        [Test, STAThread]
        public void Handwheel() //Full integrated with all optimizations
        {
            var resKeyInfo = new ResKeyInfo { Prefix = "Prefix" };
            var xaml = ConverterLogic.SvgFileToXaml("TestFiles\\Handwheel.svg", ResultMode.DrawingGroup, resKeyInfo, null);
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

        private static ResKeyInfo ResKeyInfoUseNamePrefix
        {
            get
            {
                var resKeyInfo = new ResKeyInfo
                {
                    UseComponentResKeys = false,
                    Prefix = "NamePrefix"
                };
                return resKeyInfo;
            }
        }

        private static ResKeyInfo ResKeyInfoUseCompResKey
        {
            get
            {
                var resKeyInfo = new ResKeyInfo
                {
                    UseComponentResKeys = true,
                    XamlName = "XamlName",
                    NameSpaceName = "NameSpaceName",
                    NameSpace = "MyLib.Components.Images",
                    //Prefix = "NamePrefix"
                };
                return resKeyInfo;
            }
        }

        [Test]
        public void BuildDrawingGroupName_returns_simpleName()
        {
            var resKeyInfo = new ResKeyInfo
            {
                UseComponentResKeys = false,
                Prefix = null
            };
            ConverterLogic.BuildDrawingGroupName("ElementName", resKeyInfo).Should().Be("ElementNameDrawingGroup");
        }
        [Test]
        public void BuildDrawingGroupName_returns_prefixedName()
        {
            var resKeyInfo = new ResKeyInfo
            {
                UseComponentResKeys = false,
                Prefix = "NamePrefix"
            };
            ConverterLogic.BuildDrawingGroupName("ElementName", resKeyInfo).Should().Be("NamePrefix_ElementNameDrawingGroup");
        }
        [Test]
        public void BuildDrawingGroupName_returns_prefixedName_using_CompResKey()
        {
            var resKeyInfo = new ResKeyInfo
            {
                UseComponentResKeys = true,
                XamlName = "XamlName",
                NameSpaceName = "NameSpaceName",
                //Prefix = "NamePrefix"
            };
            var key = ConverterLogic.BuildDrawingGroupName("ElementName", resKeyInfo);
            Console.WriteLine(key);
            key.Should().Be("{x:Static NameSpaceName:XamlName.ElementNameDrawingGroupKey}");
        }
        [Test]
        public void BuildDrawingImageName_returns_simpleName()
        {
            var resKeyInfo = new ResKeyInfo
            {
                UseComponentResKeys = false,
                Prefix = null
            };
            ConverterLogic.BuildDrawingImageName("ElementName", resKeyInfo).Should().Be("ElementNameDrawingImage");
        }
        [Test]
        public void BuildDrawingImageName_returns_prefixedName()
        {
            ConverterLogic.BuildDrawingImageName("ElementName", ResKeyInfoUseNamePrefix).Should().Be("NamePrefix_ElementNameDrawingImage");
        }

        [Test]
        public void BuildDrawingImageName_returns_prefixedName_using_CompResKey()
        {
            var key = ConverterLogic.BuildDrawingImageName("ElementName", ResKeyInfoUseCompResKey);
            Console.WriteLine(key);
            key.Should().Be("{x:Static NameSpaceName:XamlName.ElementNameDrawingImageKey}");
        }

        [Test]
        public void BuildResKeyReference_Static()
        {
            var actual = ConverterLogic.BuildResKeyReference("NamePrefix_ElementName", false);
            Console.WriteLine(actual);
            actual.Should().Be("{StaticResource NamePrefix_ElementName}");
        }

        [Test]
        public void BuildResKeyReference_usingCompResKey()
        {
            var actual = ConverterLogic.BuildResKeyReference("{x:Static NameSpaceName:XamlName.ElementName}",  true);
            Console.WriteLine(actual);
            actual.Should().Be("{DynamicResource {x:Static NameSpaceName:XamlName.ElementName}}");
        }

        [Test]
        public void GetElemNameFromResKey_NamePrefix()
        {
            ConverterLogic.GetElemNameFromResKey("NamePrefix_ElementName", ResKeyInfoUseNamePrefix).Should().Be("ElementName");
        }
        [Test]
        public void GetElemNameFromResKey_CompResKey()
        {
            ConverterLogic.GetElemNameFromResKey("{x:Static NameSpaceName:XamlName.ElementName}", ResKeyInfoUseCompResKey).Should().Be("ElementName");
        }
    }
}
