using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using SharpVectors.Converters;
using SharpVectors.Renderers.Wpf;

namespace SvgConverter
{
    public enum ResultMode
    {
        DrawingGroup,
        DrawingImage
    }
    public static class ConverterLogic
    {
        private const char CPrefixSeparator = '_';

        static ConverterLogic()
        {
            //bringt leider nix? _nsManager.AddNamespace("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            NsManager.AddNamespace("defns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            NsManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        }

        internal static XNamespace Nsx = "http://schemas.microsoft.com/winfx/2006/xaml";
        internal static XNamespace NsDef = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        internal static XmlNamespaceManager NsManager = new XmlNamespaceManager(new NameTable());

        public static string SvgFileToXaml(string filepath, ResultMode resultMode, ResKeyInfo resKeyInfo,
            bool filterPixelsPerDip, WpfDrawingSettings wpfDrawingSettings = null)
        {
            var obj = ConvertSvgToObject(filepath, resultMode, wpfDrawingSettings, out var name, resKeyInfo);
            return SvgObjectToXaml(obj, wpfDrawingSettings != null && wpfDrawingSettings.IncludeRuntime, name, filterPixelsPerDip);
        }

        public static ConvertedSvgData ConvertSvg(string filepath, ResultMode resultMode)
        {
            //Lazy Loading: all these elements are loaded if someone accesses the getter
            //string name;
            //var obj = ConvertSvgToObject(filepath, resultMode, null, out name) as DependencyObject;
            //var xaml = SvgObjectToXaml(obj, false, name);
            //var svg = File.ReadAllText(filepath);
           
            return new ConvertedSvgData { Filepath = filepath
            //, ConvertedObj = obj, Svg = svg, Xaml = xaml 
            };
        }

        public static object ConvertSvgToObject(string filepath, ResultMode resultMode, WpfDrawingSettings wpfDrawingSettings, out string name, ResKeyInfo resKeyInfo)
        {
            var dg = ConvertFileToDrawingGroup(filepath, wpfDrawingSettings);
            var elementName = Path.GetFileNameWithoutExtension(filepath);
            switch (resultMode)
            {
                case ResultMode.DrawingGroup:
                    name = BuildDrawingGroupName(elementName, resKeyInfo);
                    return dg;
                case ResultMode.DrawingImage:
                    name = BuildDrawingImageName(elementName, resKeyInfo);
                    return DrawingToImage(dg);
                default:
                    throw new ArgumentOutOfRangeException(nameof(resultMode));
            }
        }

        public static string SvgObjectToXaml(object obj, bool includeRuntime, string name, bool filterPixelsPerDip)
        {
            var xamlUntidy = WpfObjToXaml(obj, includeRuntime);

            var doc = XDocument.Parse(xamlUntidy);
            BeautifyDrawingElement(doc.Root, name);
            if (filterPixelsPerDip)
                FilterPixelsPerDip(doc.Root);
            var xamlWithNamespaces = doc.ToString();

            var xamlClean = RemoveNamespaceDeclarations(xamlWithNamespaces);
            return xamlClean;
        }

        public static string SvgDirToXaml(string folder, ResKeyInfo resKeyInfo, bool filterPixelsPerDip)
        {
            return SvgDirToXaml(folder, resKeyInfo, null, filterPixelsPerDip);
        }

        public static string SvgDirToXaml(string folder, ResKeyInfo resKeyInfo, WpfDrawingSettings wpfDrawingSettings,
            bool filterPixelsPerDip, bool handleSubFolders = false)
        {
            //firstChar Upper
            var firstChar = char.ToUpperInvariant(resKeyInfo.XamlName[0]);
            resKeyInfo.XamlName = firstChar + resKeyInfo.XamlName.Remove(0, 1);


            var files = SvgFilesFromFolder(folder, handleSubFolders);
            var dict = ConvertFilesToResourceDictionary(files, wpfDrawingSettings, resKeyInfo);
            var xamlUntidy = WpfObjToXaml(dict, wpfDrawingSettings?.IncludeRuntime ?? false);

            var doc = XDocument.Parse(xamlUntidy);
            RemoveResDictEntries(doc.Root);
            var drawingGroupElements = doc.Root.XPathSelectElements("defns:DrawingGroup", NsManager).ToList();
            foreach (var drawingGroupElement in drawingGroupElements)
            {
                BeautifyDrawingElement(drawingGroupElement, null);
                if (filterPixelsPerDip)
                    FilterPixelsPerDip(drawingGroupElement);

                ExtractGeometries(drawingGroupElement, resKeyInfo);
            }

            AddNameSpaceDef(doc.Root, resKeyInfo);
            //ReplaceBrushesInDrawingGroups(doc.Root, resKeyInfo);
            AddDrawingImagesToDrawingGroups(doc.Root);
            return doc.ToString();
        }

        public static IEnumerable<string> SvgFilesFromFolder(string folder, bool handleSubFolders = false)
        {
            try
            {
                if (handleSubFolders)
                {
                    return Directory.GetFiles(folder, "*.svg*", SearchOption.AllDirectories);
                }
                return Directory.GetFiles(folder, "*.svg*");
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private static void AddNameSpaceDef(XElement root, ResKeyInfo resKeyInfo)
        {
            if (resKeyInfo.UseComponentResKeys)
            {
                root.Add(new XAttribute(XNamespace.Xmlns + resKeyInfo.NameSpaceName, "clr-namespace:" + resKeyInfo.NameSpace));
            }
        }

        /// <summary>
        /// This one uses local and global colors
        /// </summary>
        /// <param name="rootElement"></param>
        /// <param name="resKeyInfo"></param>
        private static void ReplaceBrushesInDrawingGroupsOld(XElement rootElement, ResKeyInfo resKeyInfo)
        {
            //three steps of colouring: 1. global Color, 2, global ColorBrush, 3. local ColorBrush
            //<Color x:Key="ImagesColor1">#FF000000</Color>
            //<SolidColorBrush x:Key="ImagesColorBrush1" Color="{DynamicResource ImagesColor1}" />
            //<SolidColorBrush x:Key="JOG_BrushColor1" Color="{Binding Color, Source={StaticResource ImagesColorBrush1}}" />

            var allBrushes = CollectBrushAttributesWithColor(rootElement)
                .Select(a => a.Value)
                .Distinct(StringComparer.InvariantCultureIgnoreCase) //same Color only once
                .Select((s, i) => new
                {
                    ResKey1 = BuildColorName(i+1, resKeyInfo), 
                    ResKey2 = BuildColorBrushName(i + 1, resKeyInfo), 
                    Color = s
                }) //add numbers
                .ToList();

            //building global Elements like: <SolidColorBrush x:Key="ImagesColorBrush1" Color="{DynamicResource ImagesColor1}" />
            rootElement.AddFirst(allBrushes
                .Select(brush => new XElement(NsDef + "SolidColorBrush", 
                    new XAttribute(Nsx + "Key", brush.ResKey2),
                    new XAttribute("Color", $"{{DynamicResource {brush.ResKey1}}}"))));

            //building global Elements like: <Color x:Key="ImagesColor1">#FF000000</Color>
            rootElement.AddFirst(allBrushes
                .Select(brush => new XElement(NsDef + "Color", 
                    new XAttribute(Nsx + "Key", brush.ResKey1),
                    brush.Color)));

            var colorKeys = allBrushes.ToDictionary(brush => brush.Color, brush => brush.ResKey2);

            //building local Elements
            var drawingGroups = rootElement.Elements(NsDef + "DrawingGroup").ToList();
            foreach (var node in drawingGroups)
            {
                //get Name of DrawingGroup
                var keyDg = node.Attribute(Nsx + "Key").Value;
                var elemName = GetElemNameFromResKey(keyDg, resKeyInfo);
                var elemBaseName = elemName.Replace("DrawingGroup", "");
                
                var brushAttributes = CollectBrushAttributesWithColor(node).ToList();
                
                foreach (var brushAttribute in brushAttributes)
                {
                    var color = brushAttribute.Value;
                    string resKeyColor;
                    if (colorKeys.TryGetValue(color, out resKeyColor))
                    {   //global color found
                        
                        //build resourcename
                        var nameBrush = brushAttributes.Count > 1
                            ? $"{elemBaseName}Color{brushAttributes.IndexOf(brushAttribute) + 1}Brush"
                            : $"{elemBaseName}ColorBrush"; //dont add number if only one color
                        var resKeyBrush = BuildResKey(nameBrush, resKeyInfo);
                        node.AddBeforeSelf(new XElement(NsDef + "SolidColorBrush", 
                            new XAttribute(Nsx + "Key", resKeyBrush), 
                            new XAttribute("Color", $"{{Binding Color, Source={BuildResKeyReference(resKeyColor, false)}}}") ));
                        //set brush value as Reference
                        //  <GeometryDrawing Brush="{DynamicResource {x:Static nsname:Test.cloud-3-iconBrushColor}}" ... />
                        brushAttribute.Value = BuildResKeyReference(resKeyBrush, true);
                    }
                }
            }
        }

        private static void ReplaceBrushesInDrawingGroups(XElement rootElement, ResKeyInfo resKeyInfo)
        {
            //building local Elements
            var drawingGroups = rootElement.Elements(NsDef + "DrawingGroup").ToList();
            foreach (var node in drawingGroups)
            {
                var brushAttributes = CollectBrushAttributesWithColor(node).ToList();
                
                foreach (var brushAttribute in brushAttributes)
                {
                    var color = brushAttribute.Value;
                    var index = brushAttributes.IndexOf(brushAttribute);
                    brushAttribute.Value =
                        $"{{Binding Path=(brushes:Props.ContentBrushes)[{index}], RelativeSource={{RelativeSource AncestorType=Visual}}, FallbackValue={color}}}";
                }
            }

        }

        private static IEnumerable<XAttribute> CollectBrushAttributesWithColor(XElement drawingElement)
        {
            return drawingElement.Descendants()
                .SelectMany(d => d.Attributes())
                .Where(a => a.Name.LocalName == "Brush" || a.Name.LocalName == "ForegroundBrush")
                .Where(a => a.Value.StartsWith("#", StringComparison.InvariantCulture)); //is Color like #FF000000
        }

        private static void AddDrawingImagesToDrawingGroups(XElement rootElement)
        {
            var drawingGroups = rootElement.Elements(NsDef + "DrawingGroup").ToList();
            foreach (var node in drawingGroups)
            {
                //get Name of DrawingGroup
                var nameDg = node.Attribute(Nsx + "Key").Value;
                var nameImg = nameDg.Replace("DrawingGroup", "DrawingImage");
                //<DrawingImage x:Key="xxx" Drawing="{StaticResource cloud_5_icon_DrawingGroup}"/>
                var drawingImage = new XElement(NsDef + "DrawingImage",
                    new XAttribute(Nsx + "Key", nameImg),
                    new XAttribute("Drawing", string.Format(CultureInfo.InvariantCulture, "{{StaticResource {0}}}", nameDg))
                    );
                node.AddAfterSelf(drawingImage);
            }
        }

        internal static ResourceDictionary ConvertFilesToResourceDictionary(IEnumerable<string> files, WpfDrawingSettings wpfDrawingSettings, ResKeyInfo resKeyInfo)
        {
            var dict = new ResourceDictionary();
            foreach (var file in files)
            {
                var drawingGroup = ConvertFileToDrawingGroup(file, wpfDrawingSettings);
                var elementName = Path.GetFileNameWithoutExtension(file);
                var keyDg = BuildDrawingGroupName(elementName, resKeyInfo);
                dict[keyDg] = drawingGroup;
            }
            return dict;
        }

        private static DrawingGroup ConvertFileToDrawingGroup(string filepath, WpfDrawingSettings wpfDrawingSettings)
        {
            var dg = SvgFileToWpfObject(filepath, wpfDrawingSettings);
            SetSizeToGeometries(dg);
            RemoveObjectNames(dg);
            return dg;
        }

        internal static void SetSizeToGeometries(DrawingGroup dg)
        {
            var size = GetSizeFromDrawingGroup(dg);
            if (size.HasValue)
            {
                var geometries = GetPathGeometries(dg).ToList();
                geometries.ForEach(g => SizeGeometry(g, size.Value));
            }
        }

        public static IEnumerable<PathGeometry> GetPathGeometries(Drawing drawing)
        {
            var result = new List<PathGeometry>();

            Action<Drawing> handleDrawing = null;
            handleDrawing = aDrawing =>
            {
                if (aDrawing is DrawingGroup)
                    foreach (Drawing d in ((DrawingGroup)aDrawing).Children)
                    {
                        handleDrawing(d);
                    }
                if (aDrawing is GeometryDrawing)
                {
                    var gd = (GeometryDrawing)aDrawing;
                    Geometry geometry = gd.Geometry;
                    if (geometry is PathGeometry)
                    {
                        result.Add((PathGeometry)geometry);
                    }
                }
            };
            handleDrawing(drawing);

            return result;
        }

        public static void SizeGeometry(PathGeometry pg, Size size)
        {
            if (size.Height > 0 && size.Height > 0)
            {
                PathFigure[] sizeFigures =
                {
                    new PathFigure(new Point(size.Width, size.Height), Enumerable.Empty<PathSegment>(), true),
                    new PathFigure(new Point(0,0), Enumerable.Empty<PathSegment>(), true),
                };

                var newGeo = new PathGeometry(sizeFigures.Concat(pg.Figures), pg.FillRule, null);//pg.Transform do not add transform here, it will recalculate all the Points
                pg.Clear();
                pg.AddGeometry(newGeo);
                //return new PathGeometry(sizeFigures.Concat(pg.Figures), pg.FillRule, pg.Transform);
            }
        }

        internal static DrawingGroup SvgFileToWpfObject(string filepath, WpfDrawingSettings wpfDrawingSettings)
        {
            if (wpfDrawingSettings == null) //use defaults if null
                wpfDrawingSettings = new WpfDrawingSettings { IncludeRuntime = false, TextAsGeometry = false, OptimizePath = true };
            var reader = new FileSvgReader(wpfDrawingSettings);

            //this is straight forward, but in this version of the dlls there is an error when name starts with a digit
            //var uri = new Uri(Path.GetFullPath(filepath));
            //reader.Read(uri); //accessing using the filename results is problems with the uri (if the dlls are packed in ressources)
            //return reader.Drawing;

            //this should be faster, but using CreateReader will loose text items like "JOG" ?!
            //using (var stream = File.OpenRead(Path.GetFullPath(filepath)))
            //{
            //    //workaround: error when Id starts with a number
            //    var doc = XDocument.Load(stream);
            //    ReplaceIdsWithNumbers(doc.Root); //id="3d-view-icon" -> id="_3d-view-icon"
            //    using (var xmlReader = doc.CreateReader())
            //    {
            //        reader.Read(xmlReader);
            //        return reader.Drawing;
            //    }
            //}

            filepath = Path.GetFullPath(filepath);
            Stream stream = IsSvgz(filepath)
                ? (Stream)new GZipStream(File.OpenRead(filepath), CompressionMode.Decompress, false)
                : File.OpenRead(filepath);
            var doc = XDocument.Load(stream);
            stream.Dispose();

            //workaround: error when Id starts with a number
            FixIds(doc.Root); //id="3d-view-icon" -> id="_3d-view-icon"
            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                ms.Position = 0;
                reader.Read(ms);
                return reader.Drawing;
            }
        }

        private static bool IsSvgz(string filepath)
        {
            return string.Equals(Path.GetExtension(filepath), ".svgz", StringComparison.OrdinalIgnoreCase);
        }

        private static void FixIds(XElement root)
        {
            var idAttributesStartingWithDigit = root.DescendantsAndSelf()
                .SelectMany(d=>d.Attributes())
                .Where(a=>string.Equals(a.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase));
            foreach (var attr in idAttributesStartingWithDigit)
            {
                if (char.IsDigit(attr.Value.FirstOrDefault()))
                {
                    attr.Value = "_" + attr.Value;
                }

                attr.Value = attr.Value.Replace("/", "_");
            }
        }


        internal static DrawingImage DrawingToImage(Drawing drawing)
        {
            return new DrawingImage(drawing);
        }

        internal static string WpfObjToXaml(object wpfObject, bool includeRuntime)
        {
            XmlXamlWriter writer = new XmlXamlWriter(new WpfDrawingSettings { IncludeRuntime = includeRuntime});
            var xaml = writer.Save(wpfObject);
            return xaml;
        }

        internal static void RemoveResDictEntries(XElement root)
        {
            var entriesElem = root.Element(NsDef + "ResourceDictionary.Entries");
            if (entriesElem != null)
            {
                root.Add(entriesElem.Elements());
                entriesElem.Remove();
            }
        }

        private static void BeautifyDrawingElement(XElement drawingElement, string name)
        {
            InlineClipping(drawingElement);
            RemoveCascadedDrawingGroup(drawingElement);
            CollapsePathGeometries(drawingElement);
            SetDrawingElementxKey(drawingElement, name);
        }

        private static void InlineClipping(XElement drawingElement)
        {
            Rect clipRect;
            var clipElement = GetClipElement(drawingElement, out clipRect);
            if (clipElement != null && clipElement.Parent.Name.LocalName == "DrawingGroup")
            {   //add Attribute: ClipGeometry="M0,0 V40 H40 V0 H0 Z" this is the description of a rectangle-like Geometry
                clipElement.Parent.Add(new XAttribute("ClipGeometry", string.Format(CultureInfo.InvariantCulture, "M{0},{1} V{2} H{3} V{0} H{1} Z", clipRect.Left, clipRect.Top, clipRect.Bottom, clipRect.Right)));
                //delete the old Element
                clipElement.Remove();
            }
        }

        private static void RemoveCascadedDrawingGroup(XElement drawingElement)
        {
            //wenn eine DrawingGroup nix anderes wie eine andere DrawingGroup hat, werden deren Elemente eine Ebene hochgezogen und die überflüssige Group entfernt
            var drawingGroups = drawingElement.DescendantsAndSelf(NsDef + "DrawingGroup");
            foreach (var drawingGroup in drawingGroups)
            {
                var elems = drawingGroup.Elements().ToList();
                if (elems.Count == 1 && elems[0].Name.LocalName == "DrawingGroup")
                {
                    var subGroup = elems[0];

                    //var subElems = subGroup.Elements().ToList();
                    //subElems.Remove();
                    //drawingGroup.Add(subElems);
                    var subAttrNames = subGroup.Attributes().Select(a => a.Name);
                    var attrNames = drawingGroup.Attributes().Select(a => a.Name);
                    if (subAttrNames.Intersect(attrNames).Any())
                        return;
                    drawingGroup.Add(subGroup.Attributes());
                    drawingGroup.Add(subGroup.Elements());
                    subGroup.Remove();
                }
            }
        }

        private static void CollapsePathGeometries(XElement drawingElement)
        {
            //<DrawingGroup x:Name="cloud_3_icon_DrawingGroup" ClipGeometry="M0,0 V512 H512 V0 H0 Z">
            //  <GeometryDrawing Brush="#FF000000">
            //    <GeometryDrawing.Geometry>
            //      <PathGeometry FillRule="Nonzero" Figures="M512,512z M0,0z M409.338,216.254C398.922,161.293 z" />
            //    </GeometryDrawing.Geometry>
            //  </GeometryDrawing>
            //</DrawingGroup>

            //würde auch gehen:var pathGeometries = drawingElement.XPathSelectElements(".//defns:PathGeometry", _nsManager).ToArray();
            var pathGeometries = drawingElement.Descendants(NsDef + "PathGeometry").ToArray();
            foreach (var pathGeometry in pathGeometries)
            {
                if (pathGeometry.Parent != null && pathGeometry.Parent.Parent != null && pathGeometry.Parent.Parent.Name.LocalName == "GeometryDrawing")
                {
                    //check if only FillRule and Figures is available
                    var attrNames = pathGeometry.Attributes().Select(a => a.Name.LocalName).ToList();
                    if (attrNames.Count <= 2 && attrNames.Contains("Figures") && (attrNames.Contains("FillRule") || attrNames.Count == 1))
                    {
                        var sFigures = pathGeometry.Attribute("Figures").Value;
                        var fillRuleAttr = pathGeometry.Attribute("FillRule");
                        if (fillRuleAttr != null)
                        {
                            if (fillRuleAttr.Value == "Nonzero")
                                sFigures = "F1 " + sFigures; //Nonzero
                            else
                                sFigures = "F0 " + sFigures; //EvenOdd
                        }
                        pathGeometry.Parent.Parent.Add(new XAttribute("Geometry", sFigures));
                        pathGeometry.Parent.Remove();
                    }
                }
            }
        }

        private static void SetDrawingElementxKey(XElement drawingElement, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            var attributes = drawingElement.Attributes().ToList();
            attributes.Insert(0, new XAttribute(Nsx + "Key", name)); //place in first position
            drawingElement.ReplaceAttributes(attributes);
        }

        private static void FilterPixelsPerDip(XElement drawingElement)
        {
            var glyphRuns = drawingElement.Descendants(NsDef + nameof(GlyphRun)).ToList();
            foreach (var glyphRun in glyphRuns)
            {
                var pixelsPerDipAttr = glyphRun.Attribute(nameof(GlyphRun.PixelsPerDip));
                if (pixelsPerDipAttr != null)
                    pixelsPerDipAttr.Remove();
            }
        }

        private static void ExtractGeometries(XElement drawingGroupElement, ResKeyInfo resKeyInfo)
        {
            //get Name of DrawingGroup
            var nameDg = drawingGroupElement.Attribute(Nsx + "Key").Value;
            var name = nameDg.Replace("DrawingGroup", "");
            name = GetElemNameFromResKey(name, resKeyInfo);

            //find this: <GeometryDrawing Brush="{DynamicResource _3d_view_icon_BrushColor}" Geometry="F1 M512,512z M0,0z M436.631,207.445L436.631,298.319z" />
            //var geos = drawingGroupElement.XPathSelectElements(".//defns:GeometryDrawing/@defns:Geometry", _nsManager).ToList();
            var geos = drawingGroupElement.Descendants()
                .Where(e => e.Name.LocalName == "GeometryDrawing")
                .SelectMany(e => e.Attributes())
                .Where(a => a.Name.LocalName == "Geometry")
                .ToList();
            foreach (var geo in geos)
            {
                //build resourcename
                int? no = geos.Count > 1
                    ? geos.IndexOf(geo) + 1
                    : (int?)null;
                var localName = BuildGeometryName(name, no, resKeyInfo);
                //Add this: <Geometry x:Key="cloud_3_iconGeometry">F1 M512,512z M0,0z M409.338,216.254C398.922,351.523z</Geometry>
                drawingGroupElement.AddBeforeSelf(new XElement(NsDef+"Geometry",
                    new XAttribute(Nsx + "Key", localName),
                    geo.Value));
                geo.Value = BuildResKeyReference(localName, false);
            }
        }

        public static string RemoveNamespaceDeclarations(string xml)
        {
            //hier wird nur die Deklaration des NS rausgeschmissen (rein auf StringBasis), so dass man den Kram pasten kann
            xml = xml.Replace(" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"", "");
            xml = xml.Replace(" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\"", "");
            return xml;
        }

        public static void RemoveObjectNames(DrawingGroup drawingGroup)
        {
            if (drawingGroup.GetValue(FrameworkElement.NameProperty) != null)
                drawingGroup.SetValue(FrameworkElement.NameProperty, null);
            foreach (var child in drawingGroup.Children.OfType<DependencyObject>())
            {
                if (child.GetValue(FrameworkElement.NameProperty) != null)
                    child.SetValue(FrameworkElement.NameProperty, null);
                if (child is DrawingGroup)
                    RemoveObjectNames(child as DrawingGroup);
            }
        }

        internal static string BuildDrawingGroupName(string elementName, ResKeyInfo resKeyInfo)
        {
            var rawName = elementName + "DrawingGroup";
            return BuildResKey(rawName, resKeyInfo);
        }
        internal static string BuildDrawingImageName(string elementName, ResKeyInfo resKeyInfo)
        {
            var rawName = elementName + "DrawingImage";
            return BuildResKey(rawName, resKeyInfo);
        }

        internal static string BuildGeometryName(string name, int? no, ResKeyInfo resKeyInfo)
        {
            var rawName = no.HasValue
                ? $"{name}Geometry{no.Value}"
                : $"{name}Geometry"; //dont add number if only one Geometry
            return BuildResKey(rawName, resKeyInfo);
        }

        internal static string BuildColorName(int no, ResKeyInfo resKeyInfo)
        {
            var rawName = $"Color{no}";
            return BuildResKey(rawName, resKeyInfo);
        }
        internal static string BuildColorBrushName(int no, ResKeyInfo resKeyInfo)
        {
            var rawName = $"Color{no}Brush";
            return BuildResKey(rawName, resKeyInfo);
        }

        internal static string BuildResKey(string name, ResKeyInfo resKeyInfo)
        {
            if (resKeyInfo.UseComponentResKeys)
            {
                return $"{{x:Static {resKeyInfo.NameSpaceName}:{resKeyInfo.XamlName}.{ValidateName(name)}Key}}";
            }
            string result = name;
            if (resKeyInfo.Prefix != null)
                result = resKeyInfo.Prefix + CPrefixSeparator + name;
            result = ValidateName(result);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="refName">ist der schon komplett fertige name mit prefix oder Reskey</param>
        /// <param name="dynamic"></param>
        /// <returns></returns>
        internal static string BuildResKeyReference(string refName, bool dynamic = false)
        {
            var resourceIdent = dynamic ? "DynamicResource" : "StaticResource";
            return $"{{{resourceIdent} {refName}}}";
        }

        internal static string GetElemNameFromResKey(string name, ResKeyInfo resKeyInfo)
        {
            if (resKeyInfo.UseComponentResKeys)
            {   //{x:Static NameSpaceName:XamlName.ElementName}
                var p1 = name.IndexOf(".", StringComparison.Ordinal);
                var p2 = name.LastIndexOf("}", StringComparison.Ordinal);
                string result;
                if (p1 < p2)
                    result = name.Substring(p1 + 1, p2 - p1 - 1);
                else
                    result = name;
                if (result.EndsWith("Key", StringComparison.InvariantCulture))
                    result = result.Substring(0, result.Length - 3);
                return result;
            }
            else
            {
                if (resKeyInfo.Prefix == null)
                    return name;
                var prefixWithSeparator = resKeyInfo.Prefix + CPrefixSeparator;
                if (name.StartsWith(resKeyInfo.Prefix + CPrefixSeparator, StringComparison.OrdinalIgnoreCase))
                    name = name.Remove(0, prefixWithSeparator.Length);
                return name;
            }
        }

        internal static string ValidateName(string name)
        {
            var result = Regex.Replace(name, @"[^[0-9a-zA-Z]]*", "_");
            if (Regex.IsMatch(result, "^[0-9].*"))
                result = "_" + result;
            return result;
        }

        internal static void SetRootElementname(DependencyObject drawingGroup, string name)
        {
            drawingGroup.SetValue(FrameworkElement.NameProperty, name);
        }

        internal static XElement GetClipElement(XElement drawingGroupElement, out Rect rect)
        {
            rect = default(Rect);
            if (drawingGroupElement == null)
                return null;
            //<DrawingGroup x:Key="cloud_3_icon_DrawingGroup">
            //   <DrawingGroup>
            //       <DrawingGroup.ClipGeometry>
            //           <RectangleGeometry Rect="0,0,512,512" />
            //       </DrawingGroup.ClipGeometry>
            var clipElement = drawingGroupElement.XPathSelectElement(".//defns:DrawingGroup.ClipGeometry", NsManager);
            if (clipElement != null)
            {
                var rectangleElement = clipElement.Element(NsDef + "RectangleGeometry");
                if (rectangleElement != null)
                {
                    var rectAttr = rectangleElement.Attribute("Rect");
                    if (rectAttr != null)
                    {
                        rect = Rect.Parse(rectAttr.Value);
                        return clipElement;
                    }
                }
            }
            return null;
        }

        internal static Size? GetSizeFromDrawingGroup(DrawingGroup drawingGroup)
        {
            //<DrawingGroup x:Key="cloud_3_icon_DrawingGroup">
            //   <DrawingGroup>
            //       <DrawingGroup.ClipGeometry>
            //           <RectangleGeometry Rect="0,0,512,512" />
            //       </DrawingGroup.ClipGeometry>
            if (drawingGroup != null)
            {
                var subGroup = drawingGroup.Children
                    .OfType<DrawingGroup>()
                    .FirstOrDefault(c => c.ClipGeometry != null);
                if (subGroup != null)
                {
                    return subGroup.ClipGeometry.Bounds.Size;
                }
            }
            return null;
        }
    }
}
