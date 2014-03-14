using System;
using System.Linq;
using System.Threading;

namespace Toprope.Aggregator
{
    /// <summary>
    /// Represents a program.
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Represents an entry point of the application.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        public static void Main(string[] args)
        {
            ConsoleKeyInfo c;
            string[] newArgs = null;

            if (args != null && args.Any())
            {
                if (string.Compare(args[0], "--parse", true) == 0)
                {
                    newArgs = args.Skip(1).ToArray();

                    while (true)
                    {
                        Parse(newArgs);

                        Console.WriteLine();
                        Console.Write(string.Format("Finished at {0}. Continue from last checkpoint? (Y/N): ", DateTime.Now.ToString()));

                        c = Console.ReadKey(true);

                        Console.WriteLine();

                        if (char.ToLowerInvariant(c.KeyChar) != 'y')
                            break;
                    }
                }
                else if (string.Compare(args[0], "--dump", true) == 0)
                {
                    newArgs = args.Skip(1).ToArray();
                    Dump(args);

                    Console.WriteLine();
                    Console.Write("Finished.");
                }
            }
        }

        /// <summary>
        /// Runs the parsing process.
        /// </summary>
        /// <param name="args">Arguments.</param>
        private static void Parse(string[] args)
        {
            Thread t = null;
            Parsing.Parser parser = null;
            Storage.Writer writer = null;

            if (args != null && args.Length > 0)
            {
                try
                {
                    writer = new Storage.Writer();
                    parser = Parsing.Parser.Create(args[0]);

                    if (parser != null)
                    {
                        parser.Settings.ExtendedSettings = ParseExtendedSettings(args.Skip(1).ToArray());

                        Console.WriteLine(string.Format("Starting to parse data using '{0}'...", parser.GetType().Name));

                        t = new Thread(new ParameterizedThreadStart(BeginParse));
                        t.Start(new ExecutionContext() { Parser = parser, Writer = writer });

                        t.Join();
                    }
                    else
                        Console.WriteLine(string.Format("No parser found for '{0}'.", args[0]));
                }
                catch (Exception exception) { ErrorLog.Write(exception); }
            }
        }

        /// <summary>
        /// Begins parse process.
        /// </summary>
        /// <param name="parameter">Parameter.</param>
        private static void BeginParse(object parameter)
        {
            ExecutionContext context = null;

            if (parameter != null && parameter is ExecutionContext)
            {
                context = parameter as ExecutionContext;

                context.Parser.TraceReceived += (sender, args) =>
                    {
                        Console.WriteLine(args.Message);
                    };

                context.Parser.Parsing += (sender, args) =>
                    {
                        Console.WriteLine(string.Format("Writing next {0} results to disk.", context.Parser.Settings.ChunkSize));
                        context.Writer.Write(args.Areas);
                    };

                context.Parser.BeginParse();
            }
        }

        /// <summary>
        /// Runs the dump process.
        /// </summary>
        /// <param name="args">Arguments.</param>
        private static void Dump(string[] args)
        {
            using (Storage.Dumper dumber = new Storage.Dumper())
                dumber.Dump();
        }

        /// <summary>
        /// Parses extended settings.
        /// </summary>
        /// <param name="args">Arguments.</param>
        /// <returns>Extended settings.</returns>
        private static System.Collections.Specialized.NameValueCollection ParseExtendedSettings(string[] args)
        {
            string key = string.Empty;
            string value = string.Empty;
            System.Collections.Specialized.NameValueCollection ret = new System.Collections.Specialized.NameValueCollection();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].StartsWith("-"))
                {
                    if (!string.IsNullOrEmpty(key))
                        value = args[i];
                    else
                        key = args[i].TrimStart('-');
                }
                else
                    value = args[i];

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    ret[key] = value;

                    key = string.Empty;
                    value = string.Empty;
                }
            }

            return ret;
        }
    }
}
