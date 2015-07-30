using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class DirectoryHelper
    {
        public static bool TryCreate(string directory)
        {
            try
            {
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);

                    return true;
                }
            }
            catch (IOException)
            {
                return false;
            }

            return false;
        }

        public static bool Copy(string sourceDirectory, string destinationDirectory)
        {
            var success = true;
            var files = Directory.GetFiles(sourceDirectory);

            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            foreach (var f in files)
            {
                File.Copy(f, Path.Combine(destinationDirectory, Path.GetFileName(f)));
            }

            foreach (var subdir in Directory.GetDirectories(sourceDirectory))
            {
                var name = new DirectoryInfo(subdir).Name;

                if (!Copy(subdir, Path.Combine(destinationDirectory, name)))
                {
                    success = false;
                }
            }

            return success;
        }
    }
}
