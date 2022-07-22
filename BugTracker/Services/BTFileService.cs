using BugTracker.Data;
using BugTracker.Models;
using BugTracker.Services.Interfaces;

namespace BugTracker.Services
{
    public class BTFileService : IBTFileService
    {

        #region Properties
        private readonly string[] suffixes = { "Bytes", "KB", "MB", "GB", "TB", "PB" };
        #endregion

        #region Constructor
        public BTFileService()
        {
        }
        #endregion

        #region Convert Byte Array To File
        public string ConvertByteArrayToFile(byte[] fileData, string extension)
        {
            try
            {
                string imageBase64Data = Convert.ToBase64String(fileData);

                return string.Format($"data:{extension};base64,{imageBase64Data}");
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Convert File To Byte Array
        public async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            try
            {
                MemoryStream memoryStream = new();
                await file.CopyToAsync(memoryStream);
                byte[] byteFile = memoryStream.ToArray();
                memoryStream.Close();
                memoryStream.Dispose();

                return byteFile;
            }
            catch (Exception)
            {

                throw;
            }
        }
        #endregion

        #region Format File Size
        public string FormatFileSize(long bytes)
        {
            int counter = 0;
            decimal fileSize = bytes;

            while (Math.Round(fileSize / 1024) >= 1)
            {
                fileSize /= bytes;
                counter++;
            }

            return string.Format("{0:n1}{1}", fileSize, suffixes[counter]);

        }
        #endregion

        #region Get File Icon
        public string GetFileIcon(string file)
        {
            string fileImage = "default";

            if (!string.IsNullOrWhiteSpace(file))
            {
                fileImage = Path.GetExtension(file).Replace(".", "").ToLower();

                return $"/img/contenttype/{fileImage}.png";
            }

            return fileImage;
        } 
        #endregion
    }
}
