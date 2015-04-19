using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BKLib.CommandLineParser;

namespace SvgConverter
{
    public static class CmdLineHandler
    {
        public static void HandleCommandLine(string arg)
        {
            string[] args = arg != null ? arg.Split(' ') : null;
            HandleCommandLine(args);
        }
        public static int HandleCommandLine(string[] args)
        {
            var clp = new CommandLineParser { SkipCommandsWhenHelpRequested = true };

            clp.Target = new IconResBuilder();
            clp.Header = "SvgToXaml - Tool to convert SVGs to a Dictionary\r\n(c) 2015 Bernd Klaiber";
            clp.LogErrorsToConsole = true;

            try
            {
                return clp.ParseArgs(args, true);
            }
            catch (Exception)
            {
                //nothing to do, the errors are hopefully already reported via CommandLineParser
                Console.WriteLine("Error while handling Commandline.");
                return -1;
            }
        }
    }
}
