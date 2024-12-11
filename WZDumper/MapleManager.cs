using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MapleLib.WzLib;

namespace SparkleStory
{
    public static class MapleManager
    {
        private static string dbConnectionString = "Data Source=sprite_data.db;Version=3;";
        private static Regex wzFilePattern = new Regex(@"[A-Za-z]+_\d+.wz", RegexOptions.IgnoreCase);
        private static Regex itemImagePattern = new Regex(@"\d+.img");

        private static string[] SkipCategory = new[] { "Afterimage" };
        public static string DirName { get; set; }
        public static WzFile StringWZ { get; private set; }

        private static Dictionary<string, WzFile> _loadedWz;
        private static SQLiteConnection conn;
            
        static MapleManager()
        {
             conn = new SQLiteConnection(dbConnectionString);
            conn.Open();
            _loadedWz = new Dictionary<string, WzFile>();
        }
        public static void init(string dirName)
        {
            DirName = dirName;
            //if (CharacterWZ == null)
            //{
            //    CharacterWZ = new MapleLib.WzLib.WzFile(Path.Combine(DirName, "Character\\Character.wz"), MapleLib.WzLib.WzMapleVersion.BMS);
            //    CharacterWZ.ParseWzFile();
            //}
            if (StringWZ == null)
            {
                StringWZ = new MapleLib.WzLib.WzFile(Path.Combine(DirName, "String\\String.wz"), MapleLib.WzLib.WzMapleVersion.BMS);
                StringWZ.ParseWzFile();
            }
            TraverseDirectory(DirName);
        }


        private static void createCategory(string category)
        {
        
                // Create tables for each category (Cape, Hair, Weapon, etc.)
                string createTableQuery = $@"CREATE TABLE IF NOT EXISTS {category} (ID INTEGER PRIMARY KEY, Path TEXT);";

                using (SQLiteCommand cmd = new SQLiteCommand(createTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            

        }
        private static void TraverseDirectory(string directoryPath)
        {
            foreach (var subDir in Directory.GetDirectories(Path.Combine(directoryPath,"Character")))
            {
                string category = new DirectoryInfo(subDir).Name;
                if (SkipCategory.Contains(category))
                    continue;
                createCategory(category);
                // Process each .wz file in the subdirectory, but filter by regex
                foreach (var wzFile in Directory.GetFiles(subDir, "*.wz"))
                {
                    if (wzFilePattern.IsMatch(Path.GetFileName(wzFile))) // Only process wz files matching the pattern
                    {
                        IndexWzFile(category, wzFile);
                    }
                }
            }
        }
        private static void IndexWzFile(string category, string filePath)
        {
            // Open the .wz file using MapleLib or another method
            var wz = new WzFile(filePath,WzMapleVersion.BMS);
            wz.ParseWzFile();
            // Iterate through the .img folder to extract item IDs
            foreach (var img in wz.WzDirectory.WzImages)
            {
                // Ensure we have a valid ID in the folder name (like "1234.img")
                if (itemImagePattern.IsMatch(img.Name))
                {
                    string id = Path.GetFileNameWithoutExtension(img.Name);
                    InsertIntoDatabase(category, id, filePath);
                }
                else
                {
                    Console.WriteLine("Invalid image name: " + img.Name);
                }

            }
        }
        private static void InsertIntoDatabase(string category, string id, string path)
        {
            // Insert into the appropriate table
            string query = $"INSERT OR IGNORE INTO {category} (ID, Path) VALUES (@id, @path)";
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@id", int.Parse(id));
                cmd.Parameters.AddWithValue("@path", path);
                cmd.ExecuteNonQuery();
            }

        }
        public static string[] GetCategories()
        {
            var dirs = Directory.GetDirectories(Path.Combine(DirName, "Character"));
            return dirs;
        }
        public static List<WzImage> GetItems(string CategoryString)
        {
            if (!_loadedWz.TryGetValue(CategoryString, out var wzfile))
            {
                string[] files = Directory.GetFiles(Path.Combine(DirName, "Character", CategoryString));

                // Create a regex pattern based on the input string
                string regexPattern = $"^{Regex.Escape(CategoryString)}_\\d{{3}}$";
                Regex regex = new Regex(regexPattern);

                var CharacterWZ = new WzFile(Path.Combine(DirName, $"Character\\{CategoryString}\\Character.wz"), MapleLib.WzLib.WzMapleVersion.BMS);
                CharacterWZ.ParseWzFile();

                var dir = CharacterWZ.WzDirectory.WzDirectories.Single(x => x.Name == CategoryString);
                dir.ParseImages();

                return dir.WzImages;
            }
            else
                return null;
        }
        //public static WzImage GetItem(string category, string itemid)
        //{
        //    var item = (WzImage)CharacterWZ.GetObjectFromPath($"Character/Face/{itemid.PadLeft(8, '0')}.img");
        //    if (!item.Parsed)
        //        item.ParseImage();
        //    return item;
        //}


        public static List<WzImageProperty> GetNames(string CategoryString)
        {
            var eqp = StringWZ.WzDirectory.WzImages.Single(x => x.Name == "Eqp.img");

            eqp.ParseImage();
            var names = eqp.WzProperties[0].WzProperties.Single(x => x.Name == CategoryString);

            //output.Add(int.Parse(name.Name), name.WzProperties[0].WzValue as string);

            return names.WzProperties.OrderBy(x => x.WzProperties[0].WzValue).ToList();
        }

        public static List<WzImageProperty> GetFilteredNames(string CategoryString)
        {
            var names = GetNames(CategoryString);
            var rx = new Regex(@"\(.*?\)");

            var nocolors = names.Where(x => !rx.IsMatch(x.WzProperties[0].WzValue as string) && (x.WzProperties[0].WzValue as string).Last() != '.');
            return nocolors.GroupBy(x => x.WzProperties[0].WzValue as string).Select(x => x.First()).ToList();
        }
        //var CharWZ = new MapleLib.WzLib.WzFile(FileName, MapleLib.WzLib.WzMapleVersion.Bms);
        //CharWZ.ParseWzFile();
        //            var face = CharWZ.WzDirectory.GetDirectoryByName("Face");
    }
}
