using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;

namespace WindowsDeduplicator
{
    public class Deduplicator
    {
        public string RootPath { get; }

        private readonly Dictionary<string, List<string>> _files = new Dictionary<string, List<string>>();

        public Deduplicator(string rootPath)
        {
            if (String.IsNullOrWhiteSpace(rootPath))
            {
                throw new ArgumentNullException("rootPath must not be null or empty.");
            }

            RootPath = rootPath;
        }

        public List<KeyValuePair<string, List<string>>> DuplicateFiles => _files.Where(c => c.Value.Count > 1).ToList();

        public async Task Scan() => await ScanPath(RootPath);

        public async Task ReplaceWithLinks()
        {
            await Task.Run(() =>
            {
                foreach (var duplicate in DuplicateFiles)
                {
                    var target = duplicate.Value.First();

                    foreach (var file in duplicate.Value.Skip(1))
                    {
                        File.Delete(file);
                        Symlink.Create(target, file);
                    }
                }
            });
        }

        private async Task ScanPath(string path)
        {
            Console.WriteLine($"Current Directory: {path}");
            var directory = new DirectoryInfo(path);

            foreach (var file in directory.GetFiles())
            {
                var hash = await ComputeHashAsync(file);

                if (_files.TryGetValue(hash, out var existingFiles))
                {
                    existingFiles.Add(file.FullName);
                }
                else
                {
                    _files.Add(hash, new List<string>() { file.FullName });
                }
            }

            foreach (var subDirectory in directory.GetDirectories())
            {
                await ScanPath(subDirectory.FullName);
            }
        }

        private async Task<string> ComputeHashAsync(FileInfo file)
        {
            using (var hash = SHA256.Create())
            {
                using (var stream = file.OpenRead())
                {
                    var resultHash = await Task.Run(() => hash.ComputeHash(stream));

                    var result = HashToString(resultHash);

                    Console.WriteLine($"{file.FullName} - {result}");

                    return result;
                }
            }
        }

        private string HashToString(byte[] hash) => BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
