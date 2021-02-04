using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
namespace ImageToBase64
{

    class Program
    {
        static List<ImageModel> imageModels = new List<ImageModel>();
        static string basePath;
        static void Main(string[] args)
        {
            basePath = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
            var htmlFilesPath = GetHtmlFiles(basePath);
            foreach (var path in htmlFilesPath)
            {
                FindImgSrcFromFile(path);
                ReplaceImgSrc(path);
            }
            CreateImageTsFile(imageModels);
            Console.WriteLine("Done");
            Console.ReadLine();
        }

        static void FindImgSrcFromFile(string htmlFilePath)
        {
            var htmlContent = File.ReadAllText(htmlFilePath);
            string pattern = @"<img\b[^<>]*?\bsrc[\s\t\r\n]*=[\s\t\r\n]*[""']?[\s\t\r\n]*(?<imgUrl>[^\s\t\r\n""'<>]*)[^<>]*?/?[\s\t\r\n]*>";
            foreach (Match match in Regex.Matches(htmlContent, pattern))
            {
                if (match.Groups.Count < 1)
                {
                    continue;
                }
                var imagePath = match.Groups[1].Value;
                if (imageModels.Any(image => image.Path == imagePath))
                {
                    continue;
                }

                imageModels.Add(new ImageModel
                {
                    Path = imagePath,
                    FullPath = Path.Combine(basePath, match.Groups[1].Value)
                });
            }
        }

        static void ReplaceImgSrc(string htmlFilePath)
        {
            var htmlContent = File.ReadAllText(htmlFilePath);
            foreach (var model in imageModels)
            {
                htmlContent = htmlContent.Replace(model.Path, $"{{{{images.{model.VariableName}}}}}");
            }
            File.WriteAllText($"{htmlFilePath}.new.html", htmlContent);
        }

        static List<string> GetHtmlFiles(string directoryPath)
        {
            return Directory.GetFiles(directoryPath, "*.html").ToList();
        }

        static void CreateImageTsFile(List<ImageModel> imageModels)
        {
            string fileName = "images.ts";
            fileName = Path.Combine(basePath, fileName);
            if (File.Exists("images.ts"))
            {
                File.Delete(fileName);

            }
            var lines = new List<string>();
            foreach (var item in imageModels)
            {
                lines.Add($"public static readonly {item.VariableName}: string = `{item.Base64Data}`;");
            }
            File.WriteAllLines(fileName, lines);
        }
    }
}
