using System;
using System.IO;

namespace cashregister.Services
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
