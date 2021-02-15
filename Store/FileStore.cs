using System;
using System.IO;

namespace IgTrading.Store
{
    public class FileStore
    {
        public static void StoreFile(string ticker, string fileContents, DateTime downloadedTime)
        {
            string storePath = GetStorePath(ticker);
            CreateDirectoryIfNotExists(storePath);
            string path = GetFilePath(storePath, downloadedTime);

            File.AppendAllText(path, fileContents);
        }

        public static void CreateDirectoryIfNotExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static string GetFile(string ticker)
        {
            string fileName = GetFilePath(GetStorePath(ticker), DateTime.Now);
            string fileContents = string.Empty;
            if (File.Exists(fileName))
            {
                fileContents = File.ReadAllText(fileName);
            }
            return fileContents;
        }

        private static string GetStorePath(string ticker)
        {
            return $"./Store/{ticker.ToUpper()}";
        }

        private static string GetFilePath(string storePath, DateTime time)
        {
            return $"{storePath}/{time.Year}-{time.Month}-{time.Day}.json";
        }
    }
}