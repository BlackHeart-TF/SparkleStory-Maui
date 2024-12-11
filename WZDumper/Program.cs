// See https://aka.ms/new-console-template for more information
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using MapleLib;
using MapleLib.WzLib;
using SparkleStory;
using MapleLib.WzLib.Util;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        string wzPath = @"D:\void\Games\MapleStory\Data"; // Change to your WZ file path
        string exportPath = @".\maplestrpites"; // Change to your desired export folder

        if (!Directory.Exists(exportPath))
        {
            Directory.CreateDirectory(exportPath);
        }
        MapleManager.init(wzPath);
        //ExportSprites(exportPath);
    }
    public static void ExportSprites(string exportDir)
    {
        if (!Directory.Exists(exportDir))
        {
            Directory.CreateDirectory(exportDir);
        }

        var categories = MapleManager.GetCategories();

        foreach (var category in categories)
        {
            var items = MapleManager.
                GetItems(category);

            foreach (var item in items)
            {
                var itemPath = Path.Combine(exportDir, category);

                if (!Directory.Exists(itemPath))
                {
                    Directory.CreateDirectory(itemPath);
                }

                var spriteFilePath = Path.Combine(itemPath, $"{item.Name}.png"); // or .jpg based on your preference

                if (!item.Parsed)
                {
                    item.ParseImage();
                }

                // Assuming you have a method to save the image
                SaveImage(item, spriteFilePath);
            }
        }
    }

    private static void SaveImage(WzImage image, string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            //image.SaveImage(writer); // Change format if needed
        }

    }


}

