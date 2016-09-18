using System;
using BKLib.CommandLineParser;

namespace SvgConverter
{
    public static class CmdLineHandler
    {
        public static int HandleCommandLine(string arg)
        {
            string[] args = arg != null ? arg.Split(' ') : new string[0];
            return HandleCommandLine(args);
        }
        public static int HandleCommandLine(string[] args)
        {
            var clp = new CommandLineParser { SkipCommandsWhenHelpRequested = true };

            clp.Target = new CmdLineTarget();
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
