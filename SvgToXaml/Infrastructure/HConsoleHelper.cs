using System;
using System.Runtime.InteropServices;

namespace SvgToXaml.Infrastructure
{
    /// <summary>
    /// Diese Klasse stellt Funktionen bereit, um aus einer normalen WinForms- oder WPF-Anwendung 
    /// optional auch eine Konsolenanwendung zu machen.
    /// Im Prinzip muss nur InitConsoleHandles und später dann ReleaseConsoleHandles aufgerufen werden.
    /// Wenn das Prog aus der Kommandozeile gestartet wird, hängt es sich an die vorhandene Console, 
    /// ansonsten (Start über Windows) wird temporär eine Console erstellt.
    /// Hier ein Beispiel zu WPF (zusätzlich muss die BuildAction der app.xaml auf 'Page' gestellt werden)
    /// (Hier wird auch der CommandLineParser verwendet)
    /// <code>
    ///         static int Main(string[] args)
    ///    {
    ///        int exitCode = 0;
    ///     bool commandLine = false;
    ///     CommandLineParser clp = new CommandLineParser();
    ///     clp.SkipCommandsWhenHelpRequested = true;
    ///     if (args.Length > 0)
    ///     {
    ///         clp.Target = new CmdLineHandler();
    ///         clp.Header = "HCNCInstHelper - Tool to register HRemoteCNCSLServer\r\n";
    ///         clp.LogErrorsToConsole = true;
    ///         clp.ParseArgs(args, false);
    ///         commandLine = clp.ArgsParsed;
    ///     }
    ///
    ///     if (commandLine)
    ///     {
    ///         HToolBox.HConsoleHelper.InitConsoleHandles();
    ///         exitCode = clp.ExecCommands(true);
    ///         HToolBox.HConsoleHelper.ReleaseConsoleHandles();
    ///
    ///     }
    ///     else
    ///     {
    ///         //normale WPF-Applikationslogik
    ///         HCNCInstHelper.App app = new HCNCInstHelper.App();
    ///         app.InitializeComponent();
    ///         app.Run();
    ///     }
    ///     return exitCode;
    /// </code>
    /// </summary>
    public static class HConsoleHelper
    {
        [DllImport("kernel32.dll")]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll")]
        private static extern bool AttachConsole(uint pid);

        [DllImport("kernel32.dll")]
        private static extern bool FreeConsole();

        [StructLayout(LayoutKind.Explicit)]
        // ReSharper disable once InconsistentNaming
        public struct CHAR_UNION
        {
            // Fields
            [FieldOffset(0)]
            public byte AsciiChar;
            [FieldOffset(0)]
            public short UnicodeChar;
        }

        public enum InputEventType : short
        {
            KeyEvent = 0x0001,
            MouseEvent = 0x0002,
            WindowBufferSizeEvent = 0x004,
            MenuEvent = 0x0008,
            FocusEvent = 0x0010,
        }

        [Flags()]
        public enum ControlKeyState
        {
            None = 0x0000,
            RightAltPressed = 0x0001,
            LeftAltPressed = 0x0002,
            RightCtrlPressed = 0x0004,
            LeftCtrlPressed = 0x0008,
            ShiftPressed = 0x0010,
            NumLockOn = 0x0020,
            ScrollLockOn = 0x0040,
            CapsLockOn = 0x0080,
            EnhancedKey = 0x0100,
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct KeyEventStruct
        {
            public InputEventType EventType;
            [MarshalAs(UnmanagedType.Bool)]
            //wenn hier ein anderer Typ wie KeyEvent verwendet werden soll, so muss der struct entsprechend angepasst werden
            public bool bKeyDown;
            public short wRepeatCount;
            public short wVirtualKeyCode;
            public short wVirtualScanCode;
            public CHAR_UNION uChar;
            public ControlKeyState dwControlKeyState;
        }

        [DllImport("kernel32", SetLastError = true)]
        public static extern bool WriteConsoleInput(IntPtr hConsoleInput, KeyEventStruct[] lpBuffer, int nLength, ref int lpNumberOfEventsWritten);

        public enum StandardHandle
        {
            Input = -10,
            Output = -11,
            Error = -12,
        }

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr GetStdHandle(StandardHandle nStdHandle);

        // ReSharper disable once InconsistentNaming
        private const UInt32 ATTACH_PARENT_PROCESS = 0xFFFFFFFF;

        //true if attached - used to free it later
        public static bool ConsoleIsAttached { get; private set; }

        public static void InitConsoleHandles()
        {
            // Attach to console window – this may modify the standard handles
            if (AttachConsole(ATTACH_PARENT_PROCESS))
                ConsoleIsAttached = true;
            else
            {
                AllocConsole();
            }
        }

        public static void ReleaseConsoleHandles()
        {
            //Console.WriteLine("Bye Bye");

            if (ConsoleIsAttached)
            {
                //Leider wird im Falle von AttachConsole (an vorhandene Console) am Ende noch ein Enter erwartet
                //dies ist ein im Netz bekanntes Problem, man muss eben ein Enter simulieren, dafür gibt es versch. Methoden:

                //System.Windows.Forms.SendKeys.SendWait("{ENTER}"); //wir wollen kein WinForms
                //Process.GetCurrentProcess().Kill(); //klappt nicht
                //HToolBox.HSendKeys.Send("\r"); //"{ENTER}");

                SendEnterToConsoleInput(); //sauberste Lösung
            }
            else
            {
                FreeConsole(); //Pendant zu AllocConsole
            }
        }

        /// <summary>
        /// fügt einfach ein "Enter" in die Standardeingabe ein
        /// </summary>
        public static void SendEnterToConsoleInput()
        {
            IntPtr stdIn = GetStdHandle(StandardHandle.Input);

            int eventsWritten = 0;

            KeyEventStruct[] data = new KeyEventStruct[] { new KeyEventStruct() };
            data[0].EventType = InputEventType.KeyEvent;
            data[0].bKeyDown = true;
            data[0].uChar.AsciiChar = 13;
            data[0].dwControlKeyState = 0;
            data[0].wRepeatCount = 1;
            data[0].wVirtualKeyCode = 0;
            data[0].wVirtualScanCode = 0;
            WriteConsoleInput(stdIn, data, 1, ref eventsWritten);
            //Console.WriteLine("{0} events written to {1} Written:{2}", eventsWritten, stdIn.ToInt32(), written);
        }
    }
}
