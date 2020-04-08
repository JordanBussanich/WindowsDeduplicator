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

            await deduplicator.ReplaceWithLinks();

            Console.WriteLine("Finished");
        }
    }
}
