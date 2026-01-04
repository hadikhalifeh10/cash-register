// Ensures an Images folder exists at project root similar to Receipts
using System;
using System.IO;

namespace cashregister.ViewModel
{
    public static class ImageStorage
    {
        public static string GetImagesFolder()
        {
            var projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", ".."));
            var dir = Path.Combine(projectRoot, "Images");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return dir;
        }
    }
}
