using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ImageToBase64
{
    public class ImageModel
    {
        public string Path { get; set; }
        public string FullPath { get; set; }
        public string Base64Data
        {
            get
            {
                return this.Convert();
            }
        }

        public string VariableName
        {
            get
            {
                return this.Path.Replace("/", "_").Replace("\\", "_").Replace(".", "_");
            }
        }

        private string Convert()
        {
            var fileExtension = System.IO.Path.GetExtension(this.FullPath).ToLower().Replace(".", "");
            var base64data = ImageToBase64(this.FullPath);
            return $"data:image/{fileExtension};base64,{base64data}";
        }
        public static string ImageToBase64(string fileFullName)
        {
            Bitmap bmp = null;
            try
            {
                var suffix = System.IO.Path.GetExtension(fileFullName).ToLower();
                var suffixName = suffix == ".png" ? ImageFormat.Png
                    : suffix == ".jpg" || suffix == ".jpeg"
                        ? ImageFormat.Jpeg
                        : suffix == ".bmp"
                            ? ImageFormat.Bmp
                            : suffix == ".gif"
                                ? ImageFormat.Gif
                                : ImageFormat.Jpeg;

                string result = string.Empty;
                bmp = new Bitmap(fileFullName);
                using (MemoryStream ms = new MemoryStream())
                {
                    bmp.Save(ms, suffixName);
                    byte[] arr = new byte[ms.Length];
                    ms.Position = 0;
                    ms.Read(arr, 0, (int)ms.Length);
                    ms.Close();
                    result = System.Convert.ToBase64String(arr);
                }
                return result;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (bmp != null)
                    bmp.Dispose();
            }
        }
    }
}
