using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp
{
    internal class Program
    {
        /// <summary>
        /// The base folder is a positional parameter, it must be the first one.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static string GetBaseFolder(ref string[] args)
        {
            var baseFolder = args.FirstOrDefault() ?? string.Empty;
            if (Directory.Exists(baseFolder))
            {
                // If the first element is the folder, remove it from the array...
                args = args.Skip(1).ToArray();
            }
            else
            {
                baseFolder = ".";
            }
            return baseFolder;
        }

        private static bool IsParameterDefined(string shortName, string longName, ref string[] args)
        {
            static bool condition(string w, string vsShortName, string vsLongName)
                => w.Equals(vsShortName, StringComparison.InvariantCultureIgnoreCase)
                    || w.Equals(vsLongName, StringComparison.InvariantCultureIgnoreCase);

            // Force to only one parameter... if duplicate, it will fail...
            bool result;
            try
            {
                result = !string.IsNullOrEmpty(args.SingleOrDefault(w => condition(w, shortName, longName)));
            }
            catch (InvalidOperationException ex)
            {
                throw new ArgumentException($"Invalid Parameter {shortName}, {longName}", ex);
            }

            if (result)
            {
                // Remove the parameter from the input args...
                args = args
                        .Where(w => !condition(w, shortName, longName))
                        .ToArray();
            }

            return result;
        }

        private static string GetResult(
            string baseFolder,
            bool dryrun,
            IEnumerable<JxF.IO.DeletedStatus> deletedItems)
        {
            var result = new StringBuilder();

            result.AppendLine($"Base Folder '{baseFolder}'");
            result.AppendLine($"Dry-Run '{dryrun}'");

            foreach (var item in deletedItems)
            {
                var msg = item.Exception == null ? "Success" : $"Error: {item?.Exception?.Message ?? "Unknow" }";
                result.AppendLine($"{item.FullName}, {msg}");
            }

            result.AppendLine($"Folder Count {deletedItems.Count()}");
            result.AppendLine("---------------------------------------------");

            return result.ToString();
        }

        /// <summary>
        ///
        /// Command-Line Options
        /// folder: Name of the base folder
        /// </summary>
        /// <param name="args"></param>
        private static void Main(string[] args)
        {
            // Clean up the args to get the patterns list...
            var baseFolder = GetBaseFolder(ref args);
            var useRecycleBin = IsParameterDefined("-r", "--recycle-bin", ref args);
            var quiet = IsParameterDefined("-q", "--quiet", ref args);
            var dryrun = IsParameterDefined("-d", "--dry-run", ref args);

            if (!args.Any())
            {
                var msg = @"Jx
Usage: DeleteFolders [BaseFolder] regex-pattern... [Options]

Delete unwanted content from your work folders.

Options
BaseFolder         [Optional] Base folder, this program will search for subdirectory folders that match the patterns. If not defined, it looks in the current folder.
regex-pattern       Required, one or multiple regex patterns to match against the subdirectories names.
-r, --recycle-bin   [Optional] If defined, the deleted folders will be moved to Recycle Bin, if not they will be permanently deleted.
-q, --quiet         [Optional] Hide any program output.
-d, --dry-run       [Optional] Emulate the program execution without actually deleting any folder.

";
                Console.Write(msg);
                return;
            }

            var fs = new JxF.IO.Directory();

            var tasks = args.Select(s => fs.DeleteFolders(baseFolder, s, useRecycleBin, dryrun)).ToArray();
            Task.WaitAll(tasks);

            if (quiet)
            {
                return;
            }

            foreach (var task in tasks)
            {
                Console.WriteLine(
                    GetResult(
                        baseFolder,
                        dryrun,
                        task.Result
                    )
                );
            }   // foreach pattern
        }
    }
}