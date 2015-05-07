# SvgToXaml
Smart tool to view svg-files and convert them to xaml for use in .NET
There are 3 major use cases:
View many svg-files, examine single files closer (see additional Info, svg sources, xaml code)
Convert svg-file to xaml
Batch conversion of many svg-files
# View
Just start SvgToXaml and drag a folder into the view, this folder will open and you'll see a list of all the svgs located in that folder. Sure there is a button to open also a "open folder" dialog. Double click a icon see the detail view.

![Main View](/Doc/MainView.PNG)

Just drag a file into the view and the detail view will open.

![Detail View](/Doc/DetailView.PNG)

# Convert
You can open the detail view by double-clicking an icon. Here you can inspect the icon closer and can also see the converted xaml-code.

![Detail View Xaml](/Doc/DetailViewXaml.PNG)
# Batch conversion
The idea is that you collect some svgs and want to use them in your .net app. So just put them in a folder and use SvgToXaml to batch convert them into one xaml-file.
The SvgToXaml exe file is designed as a "hybrid" app. Just call it without params and it will start as a WPF app, when you specify params, it will change to a console app. Provide "/?" to see the help, there is only one command rigth now: "BuildDict"
```
>SvgToXaml.exe /? BuildDict
SvgToXaml - Tool to convert SVGs to a Dictionary
(c) 2015 Bernd Klaiber
   BuildDict  Creates a ResourceDictionary with the svg-Images of a folder
              Needed Arguments:
                /inputdir: specify folder of the graphic files to process
                /outputname: Name for the xaml outputfile
              Optional Params:
                /outputdir: folder for the xaml-Output, optional, default: folder of svgs
                /buildhtmlfile: Builds a htmlfile to browse the svgs, optional,default true
```
Example:
`..\..\SvgToXaml\bin\Debug\SvgToXaml.exe BuildDict /inputdir:.\svg /outputname:images /outputdir:.`
This is the content of the cmd file included in the sample app. It will produce a xaml file named "images.xaml" in the current folder, including all svg-files from folder ".\svg".
That's it, after that, include the file "images.xaml" into your app, merge it into resourcedictionaries in the app.xaml and you can use the icons in e.g. a button like this:
```
<Button>
    <Image Source="{StaticResource cloud_3_iconDrawingImage}"/>
</Button>
```
after adding more new icons, just run the command again and the new icons will appear in the updated xaml-file.
For each Icon the colors, paths, a drawingimage are created (with resource-keys) so you can use them as you like.
You can change the color of all icons at once (like theming your app) or you can change all the color of the icons separately (see the sample app included in the sources).
