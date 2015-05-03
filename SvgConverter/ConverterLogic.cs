using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
        static ConverterLogic()
        {
            //bringt leider nix? _nsManager.AddNamespace("", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            _nsManager.AddNamespace("defns", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            _nsManager.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
        }

        internal static XNamespace nsx = "http://schemas.microsoft.com/winfx/2006/xaml";
        internal static XNamespace nsDef = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        internal static XmlNamespaceManager _nsManager = new XmlNamespaceManager(new NameTable());

        public static string SvgFileToXaml(string filepath,
            ResultMode resultMode,
            WpfDrawingSettings wpfDrawingSettings = null)
        {
            string name;
            var obj = ConvertSvgToObject(filepath, resultMode, wpfDrawingSettings, out name);
            return SvgObjectToXaml(obj, wpfDrawingSettings != null ? wpfDrawingSettings.IncludeRuntime : false, name);
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

        public static object ConvertSvgToObject(string filepath, ResultMode resultMode, WpfDrawingSettings wpfDrawingSettings, out string name)
        {
            var dg = ConvertFileToDrawingGroup(filepath, wpfDrawingSettings);
            switch (resultMode)
            {
                case ResultMode.DrawingGroup:
                    name = BuildDrawingGroupName(filepath);
                    return dg;
                case ResultMode.DrawingImage:
                    name = BuildDrawingImageName(filepath);
                    return DrawingToImage(dg);
                default:
                    throw new ArgumentOutOfRangeException("resultMode");
            }
        }

        public static string SvgObjectToXaml(object obj, bool includeRuntime, string name)
        {
            var xamlUntidy = WpfObjToXaml(obj, includeRuntime);

            var doc = XDocument.Parse(xamlUntidy);
            BeautifyDrawingElement(doc.Root, name);
            var xamlWithNamespaces = doc.ToString();

            var xamlClean = RemoveNamespaceDeclarations(xamlWithNamespaces);
            return xamlClean;
        }

        public static string SvgDirToXaml(string folder, string xamlName)
        {
            return SvgDirToXaml(folder, xamlName, null);
        }

        public static string SvgDirToXaml(string folder, string xamlName, WpfDrawingSettings wpfDrawingSettings)
        {
            var files = SvgFilesFromFolder(folder);
            var dict = ConvertFilesToResourceDictionary(files, wpfDrawingSettings);
            var xamlUntidy = WpfObjToXaml(dict, wpfDrawingSettings != null ? wpfDrawingSettings.IncludeRuntime : false);

            var doc = XDocument.Parse(xamlUntidy);
            RemoveResDictEntries(doc.Root);
            var drawingGroupElements = doc.Root.XPathSelectElements("defns:DrawingGroup", _nsManager).ToList();
            foreach (var drawingGroupElement in drawingGroupElements)
            {
                BeautifyDrawingElement(drawingGroupElement, null);
            }
            ReplaceBrushesInDrawingGroups(doc.Root, xamlName);
            AddDrawingImagesToDrawingGroups(doc.Root);
            return doc.ToString();
        }

        public static IEnumerable<string> SvgFilesFromFolder(string folder)
        {
            try
            {
                return Directory.GetFiles(folder, "*.svg*");
            }
            catch
            {
                return Enumerable.Empty<string>();
            }
        }

        private static void ReplaceBrushesInDrawingGroups(XElement rootElement, string xamlName)
        {
            //three steps of colouring: 1. global Color, 2, global ColorBrush, 3. local ColorBrush
            //<Color x:Key="ImagesColor1">#FF000000</Color>
            //<SolidColorBrush x:Key="ImagesColorBrush1" Color="{DynamicResource ImagesColor1}" />
            //<SolidColorBrush x:Key="JOG_BrushColor1" Color="{Binding Color, Source={StaticResource ImagesColorBrush1}}" />

            var firstChar = Char.ToUpper(xamlName[0]);
            xamlName = firstChar + xamlName.Remove(0, 1);

            var allBrushes = CollectBrushAttributesWithColor(rootElement)
                .Select(a => a.Value)
                .Distinct(StringComparer.InvariantCultureIgnoreCase) //same Color only once
                .Select((s, i) => new
                {
                    ResKey1 = string.Format("{0}Color{1}", xamlName, i + 1), 
                    ResKey2 = string.Format("{0}ColorBrush{1}", xamlName, i + 1), 
                    Color = s
                }) //add numbers
                .ToList();

            //building Elements like: <SolidColorBrush x:Key="ImagesColorBrush1" Color="{DynamicResource ImagesColor1}" />
            rootElement.AddFirst(allBrushes
                .Select(brush => new XElement(nsDef + "SolidColorBrush", 
                    new XAttribute(nsx + "Key", brush.ResKey2),
                    new XAttribute("Color", string.Format("{{DynamicResource {0}}}", brush.ResKey1)))));

            //building Elements like: <Color x:Key="ImagesColor1">#FF000000</Color>
            rootElement.AddFirst(allBrushes
                .Select(brush => new XElement(nsDef + "Color", 
                    new XAttribute(nsx + "Key", brush.ResKey1),
                    brush.Color)));

            var colorKeys = allBrushes.ToDictionary(brush => brush.Color, brush => brush.ResKey2);

            var drawingGroups = rootElement.Elements(nsDef + "DrawingGroup").ToList();
            foreach (var node in drawingGroups)
            {
                //get Name of DrawingGroup
                var nameDg = node.Attribute(nsx + "Key").Value;
                var nameBrushBase = nameDg.Replace("DrawingGroup", "Brush");
                
                var brushAttributes = CollectBrushAttributesWithColor(node).ToList();
                
                foreach (var brushAttribute in brushAttributes)
                {
                    var color = brushAttribute.Value;
                    string resKey;
                    if (colorKeys.TryGetValue(color, out resKey))
                    {   //global color found
                        
                        //build resourcename
                        var localName = brushAttributes.Count > 1
                            ? string.Format("{0}Color{1}", nameBrushBase, brushAttributes.IndexOf(brushAttribute) + 1)
                            : string.Format("{0}Color", nameBrushBase); //dont add number if only one color
                        node.AddBeforeSelf(new XElement(nsDef + "SolidColorBrush", 
                            new XAttribute(nsx + "Key", localName), 
                            new XAttribute("Color", string.Format("{{Binding Color, Source={{StaticResource {0}}}}}", resKey)) ));
                        brushAttribute.Value = "{DynamicResource " + localName + "}";
                    }
                }
            }
        }

        private static IEnumerable<XAttribute> CollectBrushAttributesWithColor(XElement drawingElement)
        {
            return drawingElement.Descendants()
                .SelectMany(d => d.Attributes())
                .Where(a => a.Name.LocalName == "Brush" || a.Name.LocalName == "ForegroundBrush")
                .Where(a => a.Value.StartsWith("#")); //is Color like #FF000000
        }

        private static void AddDrawingImagesToDrawingGroups(XElement rootElement)
        {
            var drawingGroups = rootElement.Elements(nsDef + "DrawingGroup").ToList();
            foreach (var node in drawingGroups)
            {
                //get Name of DrawingGroup
                var nameDg = node.Attribute(nsx + "Key").Value;
                var nameImg = nameDg.Replace("DrawingGroup", "DrawingImage");
                //<DrawingImage x:Key="xxx" Drawing="{StaticResource cloud_5_icon_DrawingGroup}"/>
                var drawingImage = new XElement(nsDef + "DrawingImage",
                    new XAttribute(nsx + "Key", nameImg),
                    new XAttribute("Drawing", string.Format("{{StaticResource {0}}}", nameDg))
                    );
                node.AddAfterSelf(drawingImage);
            }
        }

        internal static ResourceDictionary ConvertFilesToResourceDictionary(IEnumerable<string> files, WpfDrawingSettings wpfDrawingSettings)
        {
            var dict = new ResourceDictionary();
            foreach (var file in files)
            {
                var drawingGroup = ConvertFileToDrawingGroup(file, wpfDrawingSettings);
                var keyDg = BuildDrawingGroupName(file);
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

            Action<Drawing> HandleDrawing = null;
            HandleDrawing = aDrawing =>
            {
                if (aDrawing is DrawingGroup)
                    foreach (Drawing d in ((DrawingGroup)aDrawing).Children)
                    {
                        HandleDrawing(d);
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
            HandleDrawing(drawing);

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

            //workaround: error when Id starts with a number
            var doc = XDocument.Load(Path.GetFullPath(filepath));
            ReplaceIdsWithNumbers(doc.Root); //id="3d-view-icon" -> id="_3d-view-icon"
            using (var ms = new MemoryStream())
            {
                doc.Save(ms);
                ms.Position = 0;
                reader.Read(ms);
                return reader.Drawing;
            }
        }

        private static void ReplaceIdsWithNumbers(XElement root)
        {
            var idAttributesStartingWithDigit = root.DescendantsAndSelf()
                .SelectMany(d=>d.Attributes())
                .Where(a=>string.Equals(a.Name.LocalName, "Id", StringComparison.InvariantCultureIgnoreCase))
                .Where(a=>char.IsDigit(a.Value.FirstOrDefault()));
            foreach (var attr in idAttributesStartingWithDigit)
            {
                attr.Value = "_" + attr.Value;
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

        private static void RemoveResDictEntries(XElement root)
        {
            var entriesElem = root.Element(nsDef + "ResourceDictionary.Entries");
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
            SetDrawingElementxName(drawingElement, name);
        }

        private static void InlineClipping(XElement drawingElement)
        {
            Rect clipRect;
            var clipElement = GetClipElement(drawingElement, out clipRect);
            if (clipElement != null && clipElement.Parent.Name.LocalName == "DrawingGroup")
            {   //add Attribute: ClipGeometry="M0,0 V40 H40 V0 H0 Z" this is the description of a rectangle-like Geometry
                clipElement.Parent.Add(new XAttribute("ClipGeometry", string.Format("M{0},{1} V{2} H{3} V{0} H{1} Z", clipRect.Left, clipRect.Top, clipRect.Right, clipRect.Bottom)));
                //delete the old Element
                clipElement.Remove();
            }
        }

        private static void RemoveCascadedDrawingGroup(XElement drawingElement)
        {
            //wenn eine DrawingGroup nix anderes wie eine andere DrawingGroup hat, werden deren Elemente eine Ebene hochgezogen und die überflüssige Group entfernt
            var drawingGroups = drawingElement.DescendantsAndSelf(nsDef + "DrawingGroup");
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
            var pathGeometries = drawingElement.Descendants(nsDef + "PathGeometry").ToArray();
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

        private static void SetDrawingElementxName(XElement drawingElement, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return;
            var attributes = drawingElement.Attributes().ToList();
            attributes.Insert(0, new XAttribute(nsx + "Key", name)); //place in first position
            drawingElement.ReplaceAttributes(attributes);
        }

        public static string RemoveNamespaceDeclarations(String xml)
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

        internal static string BuildDrawingGroupName(string filename)
        {
            var rawName = Path.GetFileNameWithoutExtension(filename) + "_DrawingGroup";
            return ValidateName(rawName);
        }
        internal static string BuildDrawingImageName(string filename)
        {
            var rawName = Path.GetFileNameWithoutExtension(filename) + "_DrawingImage";
            return ValidateName(rawName);
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
            var clipElement = drawingGroupElement.XPathSelectElement("//defns:DrawingGroup.ClipGeometry", _nsManager);
            if (clipElement != null)
            {
                var rectangleElement = clipElement.Element(nsDef + "RectangleGeometry");
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
