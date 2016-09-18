using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using BKLib.CommandLineParser;

namespace SvgConverter
{
    public class CmdLineTarget : SimpleBaseTarget
    {
        [ArgumentCommand(LongDesc = "Creates a ResourceDictionary with the svg-Images of a folder")]
        public int BuildDict(
            [ArgumentParam(Aliases = "i", Desc = "dir to the SVGs", LongDesc = "specify folder of the graphic files to process")]
            string inputdir,
            [ArgumentParam(Aliases = "o", LongDesc = "Name for the xaml outputfile")]
            string outputname,
            [ArgumentParam(DefaultValue = null, ExplicitNeeded = false, LongDesc = "folder for the xaml-Output, optional, default: folder of svgs")]
            string outputdir = null,
            [ArgumentParam(LongDesc = "Builds a htmlfile to browse the svgs, optional, default true")]
            bool buildhtmlfile = true,
            [ArgumentParam(DefaultValue = null, ExplicitNeeded = false, LongDesc = "Prefix to name alll items of this file, optional, default: no prefix")]
            string nameprefix = null,
            [ArgumentParam(DefaultValue = false, ExplicitNeeded = false, LongDesc = "If true, es explicit ResourceKey File is created, default: false", ExplicitWantedArguments = "resKeyNS,resKeyNSName")]
            bool useComponentResKeys = false,
            [ArgumentParam(DefaultValue = null, ExplicitNeeded = false, LongDesc = "Namespace to use with UseResKey")]
            string compResKeyNS = null,
            [ArgumentParam(DefaultValue = null, ExplicitNeeded = false, LongDesc = "name of Namespace to use with UseResKey" )]
            string compResKeyNSName = null
            )
        {
            Console.WriteLine("Building resource dictionary...");

            var outFileName = Path.Combine(outputdir ?? inputdir, outputname);
            if (!Path.HasExtension(outFileName))
                outFileName = Path.ChangeExtension(outFileName, ".xaml");

            var resKeyInfo = new ResKeyInfo
            {
                Name = null,
                XamlName = Path.GetFileNameWithoutExtension(outputname),
                Prefix = nameprefix,
                UseComponentResKeys = useComponentResKeys,
                NameSpace = compResKeyNS,
                NameSpaceName = compResKeyNSName,
            };

            File.WriteAllText(outFileName, ConverterLogic.SvgDirToXaml(inputdir, resKeyInfo, null));
            Console.WriteLine("xaml written to: {0}", outFileName);

            if (buildhtmlfile)
            {
                var htmlFilePath = Path.Combine(inputdir,
                    Path.GetFileNameWithoutExtension(outputname));
                var files = ConverterLogic.SvgFilesFromFolder(inputdir);
                BuildHtmlBrowseFile(files, htmlFilePath);
            }
            return 0; //no Error
        }


        //[ArgumentValue("Path", Desc = "Filepath or directory of the Icon(s)", Aliases = "p;SrcPath")]
        //public string SrcPath { get; set; }

        //[ArgumentValue("TargetFilename", Desc = "Name of the xaml-Targetfile to create", Aliases = "t", ExplicitNeeded = false, DefaultValue = null)]
        //public string TargetFilename { get; set; }

        //[ArgumentValue("br", Desc = "use this to replace Colors in the BrushDefinition e.g. -br \"#FF000000->{DynamicResource MyBrush2}\"", ExplicitNeeded = false)]
        //public string[] BrushReplaces { get; set; }

        //[ArgumentValue("browser", Desc = "Creates a browsable html file (only for Folders)", DefaultValue = true)]
        //public bool CreateHtmlBrowseFile { get; set; }

        //private WpfDrawingSettings settings;
        //[ArgumentCommand(Desc = "Builds a xaml-File", Aliases = "b", IsDefaultCmd = true)]
        //public void Build()
        //{
        //    settings = new WpfDrawingSettings { TextAsGeometry = true, WriteAsRoot = true, IncludeRuntime = false };
        //    var dict = new ResourceDictionary();

        //    if (Directory.Exists(SrcPath))
        //    {
        //        if (String.IsNullOrEmpty(TargetFilename))
        //        {
        //            DirectoryInfo dir = new DirectoryInfo(SrcPath);
        //            TargetFilename = Path.Combine(Path.GetDirectoryName(SrcPath), Path.ChangeExtension(dir.Name, ".xaml"));
        //        }
        //        string[] files = Directory.EnumerateFiles(SrcPath, "*.svg").ToArray();
        //        foreach (var file in files)
        //        {
        //            HandleIcon(file, dict);
        //        }

        //        if (CreateHtmlBrowseFile)
        //        {
        //            BuildHtmlBrowseFile(files, TargetFilename);
        //        }
        //    }
        //    else
        //    {
        //        if (!File.Exists(SrcPath))
        //            throw new FileNotFoundException("File not found", SrcPath);
        //        if (String.IsNullOrEmpty(TargetFilename))
        //            TargetFilename = Path.ChangeExtension(SrcPath, ".xaml");
        //        HandleIcon(SrcPath, dict);
        //    }

        //    Console.WriteLine("Output: {0}", TargetFilename);
        //    XmlXamlWriter writer = new XmlXamlWriter(settings);


        //    XmlDocument doc = new XmlDocument(); //könnte evtl. auch so gehen: var doc = XDocument.Parse(writer.Save(dict)); dort ist aber auch der NamespaceManager nötig

        //    doc.LoadXml(writer.Save(dict));

        //    RemoveNames(doc);
        //    ReplaceBrush(doc);

        //    doc.Save(TargetFilename);
        //    //File.WriteAllText(TargetFilename, writer.Save(dict));
        //}

        //[Obsolete("Use SvgFileToWpfObject")]
        //private void HandleIcon(string iconPath, ResourceDictionary dict)
        //{
        //    string iconName = Path.GetFileNameWithoutExtension(iconPath);
        //    Debug.Assert(iconName != null, "iconName != null");

        //    Console.WriteLine("Handling File {0}", iconName);
        //    var reader = new FileSvgReader(settings);
        //    reader.Read(iconPath);
        //    var obj = new DrawingImage(reader.Drawing);

        //    dict[iconName] = obj;
        //}

        //#region FlowEnabled


        //#endregion FlowEnabled

        //private void RemoveNames(XmlDocument doc)
        //{
        //    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        //    nsmgr.AddNamespace("nons", doc.DocumentElement.GetNamespaceOfPrefix(""));
        //    nsmgr.AddNamespace("x", doc.DocumentElement.GetNamespaceOfPrefix("x"));

        //    foreach (var attr in doc.SelectNodes("//nons:DrawingGroup/@x:Name", nsmgr).Cast<XmlAttribute>().ToArray())
        //    {
        //        attr.OwnerElement.Attributes.Remove(attr);
        //    }

        //    foreach (var attr in doc.SelectNodes("//nons:GeometryDrawing/@x:Name", nsmgr).Cast<XmlAttribute>().ToArray())
        //    {
        //        attr.OwnerElement.Attributes.Remove(attr);
        //    }
        //}


        //private void ReplaceBrush(XmlDocument doc)
        //{
        //    if (BrushReplaces == null)
        //        return;

        //    XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        //    nsmgr.AddNamespace("nons", doc.DocumentElement.GetNamespaceOfPrefix(""));

        //    Dictionary<string, string> brushReplaceDict = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
        //    foreach (var brushReplace in BrushReplaces)
        //    {
        //        //example:-br \"#FF000000->{DynamicResource MyBrush2}\"
        //        string[] parts = brushReplace.Split(new string[] { "->" }, StringSplitOptions.None);
        //        if (parts.Length == 2)
        //        { brushReplaceDict.Add(parts[0], parts[1]); }
        //    }

        //    //foreach (var attr in doc.SelectNodes("//nons:GeometryDrawing[@Brush='#FF000000']//@Brush", nsmgr).Cast<XmlAttribute>())
        //    foreach (var attr in doc.SelectNodes("//nons:GeometryDrawing//@Brush", nsmgr).Cast<XmlAttribute>())
        //    {
        //        string newValue;
        //        if (brushReplaceDict.TryGetValue(attr.Value, out newValue))
        //            attr.Value = newValue;
        //    }
        //}

        //private void BuildHtmlBrowseFile(IEnumerable<string> files, string xamlFilepath, int size = 128)
        //{
        //    //<html>
        //    //    <head>
        //    //        <title>Browse Images</title>
        //    //    </head>
        //    //    <body>
        //    //        Images in file xyz<br>
        //    //        <img src="cloud-17-icon.svg" title="Title" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //        <img src="cloud-17-icon.svg" height="128" width="128">
        //    //    </body>
        //    //</html>            
        //    XDocument doc = new XDocument(
        //    new XElement("html",
        //        new XElement("head",
        //            new XElement("title", "Browse svg images")),
        //        new XElement("body", string.Format("Images in file: {0}", xamlFilepath),
        //            new XElement("br"),
        //            files.Select(
        //            f => new XElement("img",
        //                new XAttribute("src", Path.GetFileName(f)),
        //                new XAttribute("title", Path.GetFileNameWithoutExtension(f)),
        //                new XAttribute("height", size),
        //                new XAttribute("width", size)
        //                )
        //            )
        //        )
        //    ));
        //    doc.Save(Path.ChangeExtension(xamlFilepath, ".html"));
        //}
        private static void BuildHtmlBrowseFile(IEnumerable<string> files, string outputFilename, int size = 128)
        {
            //<html>
            //    <head>
            //        <title>Browse Images</title>
            //    </head>
            //    <body>
            //        Images in file xyz<br>
            //        <img src="cloud-17-icon.svg" title="Title" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //        <img src="cloud-17-icon.svg" height="128" width="128">
            //    </body>
            //</html>            
            var doc = new XDocument(
            new XElement("html",
                new XElement("head",
                    new XElement("title", "Browse svg images")),
                new XElement("body", $"Images in file: {outputFilename}",
                    new XElement("br"),
                    files.Select(
                    f => new XElement("img",
                        new XAttribute("src", Path.GetFileName(f) ?? ""),
                        new XAttribute("title", Path.GetFileNameWithoutExtension(f) ?? ""),
                        new XAttribute("height", size),
                        new XAttribute("width", size)
                        )
                    )
                )
            ));
            var filename = Path.ChangeExtension(outputFilename, ".html");
            doc.Save(filename);
            Console.WriteLine("Html overview written to {0}", filename);
        }

        ////private void ReplaceBrush(DrawingImage image)
        ////{
        ////    Action<GeometryDrawing> HandleGeometryDrawing = gd =>
        ////    {
        ////        if (string.Equals(gd.Brush.ToString(), "#FF000000", StringComparison.InvariantCultureIgnoreCase))
        ////        {
        ////            gd.Brush
        ////        }
        ////    };

        ////    Action<DrawingGroup> HandleDrawingGroup = null;

        ////    HandleDrawingGroup = (@drawingGroup =>
        ////    {
        ////        if (drawingGroup != null)
        ////        {
        ////            foreach (var child in drawingGroup.Children)
        ////            {
        ////                if (child is GeometryDrawing)
        ////                    HandleGeometryDrawing((GeometryDrawing) child);

        ////                if (child is DrawingGroup)
        ////                    HandleDrawingGroup((DrawingGroup) child); //recurse
        ////            }
        ////        }
        ////    });

        ////    HandleDrawingGroup(image.Drawing as DrawingGroup);
        ////}

        //public static void HandleCommandline(string arg)
        //{
        //    string[] args = arg != null ? arg.Split(' ') : null;
        //    HandleCommandline(args);
        //}
        //public static void HandleCommandline(string[] args)
        //{
        //    CommandLineParser clp = new CommandLineParser();
        //    clp.Target = new IconResBuilder();
        //    clp.SkipCommandsWhenHelpRequested = true;
        //    clp.BreakOnExitCodeNonZero = true;
        //    clp.CaseSensitive = false;
        //    clp.CountExpectedCommands = 1;
        //    clp.Header = "SvgConverter (c) by BKEDV/symtevia 2013-2015\r\n";
        //    clp.LogErrorsToConsole = true;
        //    try
        //    {
        //        clp.ParseArgs(args, false);
        //    }
        //    catch
        //    {
        //        ArgumentBaseCommand helpArg =
        //            clp.ArgLookup.Values.OfType<ArgumentBaseCommand>().Where(a => a.IsHelpCommand).FirstOrDefault();
        //        if (helpArg != null)
        //            helpArg.Execute();
        //        else
        //            throw;
        //    }

        //    clp.ExecCommands(true);
        //}
    }
}
