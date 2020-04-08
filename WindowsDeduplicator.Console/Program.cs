using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsDeduplicator
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Count() != 2)
            {
                PrintUsage();
                return;
            }

            if (!args[1].Equals("-scan", StringComparison.OrdinalIgnoreCase) || !args[1].Equals("-run", StringComparison.OrdinalIgnoreCase))
            {
                PrintUsage();
                return;
            }

            bool run = args[1].Equals("-run", StringComparison.OrdinalIgnoreCase);

            try
            {
                var deduplicator = new Deduplicator(@"C:\test");

                await deduplicator.Scan();

                Console.WriteLine();
                Console.WriteLine();

                foreach (var duplicate in deduplicator.DuplicateFiles)
                {
                    Console.WriteLine($"Hash: {duplicate.Key}, files:");
                    foreach (var file in duplicate.Value)
                    {
                        Console.WriteLine(file);
                    }
                }

                Console.WriteLine();
                Console.WriteLine();

                if (run)
                {
                    await deduplicator.ReplaceWithLinks();
                }
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                PrintUsage();
            }

            Console.WriteLine("Finished");
        }

        static void PrintUsage()
        {
            Console.WriteLine("Windows Deduplicator\n");
            Console.WriteLine("WindowsDeduplicator.Console.exe <path> {-run|-scan}\n\n");
            Console.WriteLine("Usage:\n");
            Console.WriteLine("<path>\tThe directory to be scanned.");
            Console.WriteLine("{-scan}\tOnly scan the specified directory, print a list of duplicate files and their hashes.");
            Console.WriteLine("{-run}\tScan the specified directory for duplicate files and replace the duplicates with symlinks.");
        }
    }
}
