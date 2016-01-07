/* 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/. 
 */
using System.Threading.Tasks;
using PCLStorage;

namespace OS2Indberetning.BuisnessLogic
{
    /// <summary>
    /// FileHandler is responsible for handling reading and writing to text files.
    /// </summary>
    public static class FileHandler
    {
        /// <summary>
        /// Reads content from a file
        /// </summary>
        /// <param name="filename">the name of the file</param>
        /// <param name="foldername">the name of the folder containing the file</param>
        /// <returns>string of the read content</returns>
        public static async Task<string> ReadFileContent(string filename, string foldername)
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            IFolder specificFolder = await rootFolder.CreateFolderAsync(foldername, CreationCollisionOption.OpenIfExists);
            await specificFolder.CreateFileAsync(filename, CreationCollisionOption.OpenIfExists);
            ExistenceCheckResult exist = await specificFolder.CheckExistsAsync(filename);
            
            string text = null;
            if (exist == ExistenceCheckResult.FileExists)
            {
                IFile file = await specificFolder.GetFileAsync(filename);
                text = await file.ReadAllTextAsync();
            }
            return text;
        }

        /// <summary>
        /// Writes content to a file
        /// </summary>
        /// <param name="filename">the name of the file</param>
        /// <param name="foldername">the name of the folder containing the file</param>
        /// <param name="content">The content to be written to the file</param>
        /// <returns>returns true on success, false on failure</returns>
        public static async Task<bool> WriteFileContent(string filename, string foldername, string content)
        {
            var rootFolder = FileSystem.Current.LocalStorage;
            IFolder specificFolder = await rootFolder.CreateFolderAsync(foldername, CreationCollisionOption.OpenIfExists);
            ExistenceCheckResult exist = await specificFolder.CheckExistsAsync(filename);

            string text = null;
            if (exist == ExistenceCheckResult.FileExists)
            {
                IFile file = await specificFolder.GetFileAsync(filename);
                await file.WriteAllTextAsync(content);
            }
            else
            {
                IFile file = await specificFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                await file.WriteAllTextAsync(content);
            }
            return true;
        }
    }
}
