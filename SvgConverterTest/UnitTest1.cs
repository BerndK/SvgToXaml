using System;
using NUnit.Framework;
using SvgConverter;

namespace SvgConverterTest
{
    [TestFixture]
    public class UnitTest1
    {
        [Test]
        public void EmptyArgsTest1()
        {
            string arg = null;
            CmdLineHandler.HandleCommandLine(arg);
        }
        [Test]
        public void EmptyArgsTest2()
        {
            CmdLineHandler.HandleCommandLine("");
        }
        [Test]
        public void HelpTest()
        {
            CmdLineHandler.HandleCommandLine("-H");
        }

        [Test]
        public void SingleFile1Test()
        {
            CmdLineHandler.HandleCommandLine("/SrcPath=\"..\\..\\TestFiles\\cloud-3-icon.svg\" -br \"#FF000000->{DynamicResource MyBrush2}\"");
        }
        [Test]
        public void SingleFile2Test()
        {
            CmdLineHandler.HandleCommandLine("/SrcPath=\"..\\..\\TestFiles\\cloud-3-icon.svg\" /TargetFilename=\"SingleFile.xaml\"");
        }

        [Test]
        public void Dir1Test()
        {
            CmdLineHandler.HandleCommandLine("/SrcPath=\"..\\..\\TestFiles\\\"");
        }
        [Test]
        public void Dir2Test()
        {
            CmdLineHandler.HandleCommandLine("/SrcPath=\"..\\..\\TestFiles\\\" /TargetFilename=\"Dir.xaml\"");
        }

    }
}
