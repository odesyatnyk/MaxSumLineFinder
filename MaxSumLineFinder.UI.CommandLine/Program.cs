using MaxSumLineFinder.ApplicationLogic;
using System;
using System.Linq;

namespace MaxSumLineFinder.UI.CommandLine
{
    class Program
    {
        static int Main(string[] args)
        {
            string path;

            if (args.Length == 0)
            {
                Console.Write("Please enter a path string: ");
                path = Console.ReadLine();
            }
            else
            {
                path = args[0];
            }

            var parser = new FileParser();

            try
            {
                var result = parser.Parse(path);
                DisplayResult(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occured during current operation:");
                Console.WriteLine($"  - {ex.Message}");
                Console.WriteLine("Stack Trace");
                Console.WriteLine($"{ex.StackTrace}");

                if (!(ex.InnerException is null))
                {
                    Console.WriteLine();
                    Console.WriteLine("Inner exception:");
                    Console.WriteLine($"  - {ex.InnerException.Message}");
                    Console.WriteLine("Inner exception Stack Trace:");
                    Console.WriteLine($"{ex.InnerException.StackTrace}");
                }
                
                Console.WriteLine();
                Console.WriteLine("Press Enter to exit");
                Console.ReadLine();
                return 1;
            }

            Console.WriteLine("Press Enter to exit");
            Console.ReadLine();
            return 0;
        }

        static void DisplayResult(FileParseResult result)
        {
            if (!result.AnyInvalidLine && !result.AnyValidLine)
            {
                Console.WriteLine("No results to display.");
            }
            else
            {
                if (result.AnyValidLine)
                    Console.WriteLine($"Lines with max sum of elements: {result.LinesWithMaxSum.ToCommaSeparatedString()}");

                if (result.AnyInvalidLine)
                    Console.WriteLine($"Invalid lines: {result.InvalidLines.ToCommaSeparatedString()}");
            }
        }
    }
}
