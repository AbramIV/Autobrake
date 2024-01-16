using System.IO.Compression;
using System.Text;

namespace Helpers;

public static class FileWorker
{
    /// <summary>
    /// Create new filename if either already exist. Create directory.
    /// </summary>
    /// <param name="FullPath">Old filename.</param>
    /// <returns>New filename.</returns>
    public static string GetNextPath(string fullPath, bool deleteExist = false)
    {
        int count = 1;
        string nextPath;

        if (string.IsNullOrEmpty(Path.GetFileName(fullPath)) ||
            string.IsNullOrEmpty(Path.GetExtension(fullPath)))
        {
            if (Directory.Exists(fullPath) || deleteExist)
                Directory.Delete(fullPath, true);

            if (!Directory.Exists(fullPath))
            {
                Directory.CreateDirectory(fullPath);
                return fullPath.EndsWith(@"\") ? fullPath : fullPath + @"\";
            }

            nextPath = $"{fullPath.Trim('\\')} ({count})\\";

            while (Directory.Exists(nextPath.Trim('\\')))
                nextPath = $"{fullPath.Trim('\\')} ({++count})\\";

            Directory.CreateDirectory(nextPath);
        }
        else
        {
            string extension = Path.GetExtension(fullPath);
            string directoryName = Path.GetDirectoryName(fullPath);
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            nextPath = $"{directoryName}\\{fileName} ({count}){extension}";

            if (!Directory.Exists(directoryName))
            {
                Directory.CreateDirectory(directoryName);
                return fullPath;
            }

            if (File.Exists(fullPath) && deleteExist)
                File.Delete(fullPath);

            if (!File.Exists(fullPath)) return fullPath;

            while (File.Exists(nextPath))
                nextPath = $"{directoryName}\\{fileName} ({++count}){extension}";
        }

        return nextPath;
    }

    /// <summary>
    /// Extract files from archive and archives include.
    /// </summary>
    /// <param name="ArchivePath">Path to archive.</param>
    /// <param name="ExtractDirectory">Path to extract directory.</param>
    /// <param name="Recursive">It means about extraction including archives. </param>
    /// <returns>Paths of files.</returns>
    public static string[] Extract(string archivePath, string extractDirectory, bool isRecursive)
    {
        extractDirectory = GetNextPath(extractDirectory.Trim('\\') + @"\" + Path.GetFileNameWithoutExtension(archivePath) + @"\");

        ZipFile.ExtractToDirectory(archivePath, extractDirectory, Encoding.GetEncoding(866));

        if (isRecursive)
            foreach (string path in Directory.GetFiles(extractDirectory, "*.zip", SearchOption.AllDirectories))
                Extract(path, Path.GetDirectoryName(path), true);

        return Directory.GetFiles(extractDirectory, "*.*", SearchOption.AllDirectories);
    }
}