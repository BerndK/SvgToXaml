using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SvgConverter
{
    public enum PathIs
    {
        Folder,
        File
    }

    public static class FileUtils
    {
        /// <summary>
        /// Creates a relative path from one file or folder to another.
        /// </summary>
        /// <param name="fromPath">Contains the directory that defines the start of the relative path.</param>
        /// <param name="fromIs">Is the fromPath a File or a Folder</param>
        /// <param name="toPath">Contains the path that defines the endpoint of the relative path.</param>
        /// <param name="toIs">Is the toPath a File or a Folder</param>
        /// <returns>The relative path from the start directory to the end path or <c>toPath</c> if the paths are not related.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="UriFormatException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        public static String MakeRelativePath(String fromPath, PathIs fromIs, String toPath, PathIs toIs)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            //Slash am Ende anfügen, damit Uri damit klarkommt und weiß, was ein Folder ist, und was nicht
            if (!fromPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !fromPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()) &&
                fromIs == PathIs.Folder)
                fromPath += Path.DirectorySeparatorChar;
            if (!toPath.EndsWith(Path.DirectorySeparatorChar.ToString()) &&
                !toPath.EndsWith(Path.AltDirectorySeparatorChar.ToString()) &&
                toIs == PathIs.Folder)
                toPath += Path.DirectorySeparatorChar;

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.ToUpperInvariant() == "FILE")
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }
            if (relativePath == string.Empty)
                relativePath = ".\\";
            //ein \ am Ende entfernen, dies macht Probleme, insbesondere in CommandLine wenn quoted
            //zudem scheint der .Net - Standard zu sein, kein \ am Ende zu haben vgl. Path.GetDirectoryname()
            return relativePath.TrimEnd(Path.DirectorySeparatorChar);
        }
    }
}
