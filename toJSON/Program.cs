using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using toJSON.Classes;
using toJSON.Classes.InputEngines;
using toJSON.Classes.Output.Templates;

namespace toJSON
{
    class Program
    {
        public static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static string Input;
        static string Output;
        static string FileMask;
        static string Delimiter;
        static string OutputTemplate;
        static string FixedWidthFile;

        static OutputTemplateType OutputTemplateType = OutputTemplateType.JSON;

        static InputEngine InputEngineMode = InputEngine.undefined;
        static InputType InputMethodType = InputType.Invalid;

        static SearchOption SearchMode = SearchOption.TopDirectoryOnly;
        
        static void Main(string[] args)
        {
            if (args.Length > 0 && CheckArguments(args) && ValidateSettings())
            {
                if (InputMethodType == InputType.File)
                {
                    var fileInputManager = new FileInputManager(OutputTemplateType, new FileInfo(Input), new FileInfo(Output));
                    if (OutputTemplate != null) fileInputManager.OutputTemplate = new FileInfo(OutputTemplate);
                    Engine engine = null;
                    if (InputEngineMode == InputEngine.xlsx) engine = new ExcelEngine();
                    if (InputEngineMode == InputEngine.csv) engine = new CSVEngine();
                    if (InputEngineMode == InputEngine.del) engine = new DelimitedEngine(Delimiter);
                    if (InputEngineMode == InputEngine.fw) engine = new FixedWidthEngine(new FileInfo(FixedWidthFile));
                    if (engine != null)
                    {
                        fileInputManager.ProcessFile(engine);
                        engine.Shutdown();
                    }
                }
            } else
            {

            }
        }
        public static T FatalReturn<T>(string message, T returnStatus)
        {
            Logger.Fatal(message);
            return returnStatus;
        }

        static InputType CheckInput()
        {
            if (File.Exists(Input)) return InputType.File;
            if (Directory.Exists(Input)) return InputType.Directory;
            return InputType.Invalid;
        }

        private static bool ValidateSettings()
        {
            // Fatals
            if (Input == null) return FatalReturn("No input was specified.", false);
            if (Output == null) return FatalReturn("No output was specified.", false);
            if (OutputTemplateType == OutputTemplateType.Invalid) return FatalReturn("No output template type was specified.", false);
            if (InputEngineMode == InputEngine.undefined) return FatalReturn("No input engine was specified.", false);
            if (InputEngineMode == InputEngine.del && Delimiter == null) return FatalReturn("The input engine is in delimiter mode, but no delimiter was given.", false);
            if (InputEngineMode == InputEngine.fw && FixedWidthFile == null) return FatalReturn("The input engine is in fixed width mode, but no fixed width mapping file was given.", false);
            if (InputMethodType == InputType.Invalid) return FatalReturn("Invalid input specified.", false);
            if (InputMethodType == InputType.Directory && FileMask == null) return FatalReturn("Input is a directory, but no filemask was specified.", false);

            // Warnings
            if (FileMask != null && InputMethodType == InputType.File) Logger.Warn("Input is a file, but filemask is specified. Ignoring.");
            if (SearchMode == SearchOption.AllDirectories && InputMethodType == InputType.File) Logger.Warn("Input is a file, but the recursive flag was set. Ignorning.");
            if (Delimiter != null && InputEngineMode != InputEngine.del) Logger.Warn("Delimiter specified, but the input engine is not in delimeter mode. Ignoring");

            return true;
        }

        static bool CheckArguments(string[] args)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("--"))
                {
                    var command = args[i].Substring(2);
                    var hasNextIndex = (args.Length > (i + 1));
                    switch (command.ToLower())
                    {
                        case "i":
                            if (!hasNextIndex)
                                return FatalReturn("No input specified.", false);
                            Input = args[i + 1];
                            Logger.Info("Input: {0}", Input);
                            InputMethodType = CheckInput();
                            break;
                        case "o":
                            if (!hasNextIndex)
                                return FatalReturn("No output specified.", false);
                            Output = args[i + 1];
                            Logger.Info("Output: {0}", Output);
                            break;
                        case "fm":
                            if (!hasNextIndex)
                                return FatalReturn("No filemask specified.", false);
                            FileMask = args[i + 1];
                            Logger.Info("Filemask: {0}", FileMask);
                            break;
                        case "ie":
                            if (!hasNextIndex)
                                return FatalReturn("No input engine specified.", false);
                            switch(args[i + 1].ToLower())
                            {
                                case "xlsx":
                                    InputEngineMode = InputEngine.xlsx;
                                    break;
                                case "csv":
                                    InputEngineMode = InputEngine.csv;
                                    break;
                                case "del":
                                    InputEngineMode = InputEngine.del;
                                    break;
                                case "fw":
                                    InputEngineMode = InputEngine.fw;
                                    break;
                            }
                            break;
                        case "d":
                            if (!hasNextIndex)
                                return FatalReturn("No delimiter specified.", false);
                            Delimiter = args[i + 1];
                            Logger.Info("Delimiter: {0}", Delimiter);
                            break;
                        case "ot":
                            if (!hasNextIndex)
                                return FatalReturn("No output template specified.", false);
                            switch (args[i + 1].ToLower())
                            {
                                case "json":
                                    OutputTemplateType = OutputTemplateType.JSON;
                                    break;
                            }
                            break;
                        case "otf":
                            if (!hasNextIndex || !File.Exists(args[i + 1]))
                                return FatalReturn("No output template file was specified.", false);
                            OutputTemplate = args[i + 1];
                            Logger.Info("Output Template: {0}", OutputTemplate);
                            break;
                        case "r":
                            SearchMode = SearchOption.AllDirectories;
                            Logger.Info("Recursive mode active");
                            break;
                        case "fwmf":
                            if (!hasNextIndex || !File.Exists(args[i + 1]))
                                return FatalReturn("No fixed width mapping file was specified.", false);
                            FixedWidthFile = args[i + 1];
                            Logger.Info("Fixed Width Mapping File: {0}", FixedWidthFile);
                            break;
                    }
                }
            }
            return true;
        }

        enum InputEngine {
            xlsx,
            csv,
            del,
            fw,
            undefined
        }

        enum InputType
        {
            File,
            Directory,
            Invalid
        }
    }
}
