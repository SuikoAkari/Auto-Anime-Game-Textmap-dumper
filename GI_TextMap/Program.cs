using Newtonsoft.Json;
using GI_TextMap;
using System.Reflection;
using System.IO;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Diagnostics;

public static class DictionaryExtensions
{
    public static void AddRange<TKey, TValue>(this IDictionary<TKey, TValue> target, IEnumerable<KeyValuePair<TKey, TValue>> source)
    {
        if (target == null)
            throw new ArgumentNullException(nameof(target));
        if (source == null)
            throw new ArgumentNullException(nameof(source));
        foreach (var element in source)
        {
            if (!target.ContainsKey(element.Key)) target.Add(element.Key, element.Value);
        }

            
    }
}

public class Program
{
    public static string GAME_BLOCKS_PATH = "path to \\StreamingAssets\\AssetBundles\\blocks";
    public static string ASSET_STUDIO_CLI_PATH = "path to \\AssetStudio.CLI.exe";
    public static string AI_FILE = "path to \\output_assetindex_minify.json";
    public static string XOR_KEY = "0x97"; //version xor key
    public static string VER = "5.4";
    public static void ParseTextMap(string dir,string name="TextMap_EN")
    {
        //Dumping automatically with ASSETSTUDIO
        string thisPath = Path.GetFullPath(dir);
        Console.WriteLine("Dumping " + thisPath + " Folder");
        string cmd = $"\"{GAME_BLOCKS_PATH}\\{dir}\" \"{thisPath}\" --game GI --ai_file \"{AI_FILE}\" --key {XOR_KEY}  --group_assets ByType";
        Process process = Process.Start(ASSET_STUDIO_CLI_PATH,cmd);
        while (!process.HasExited)
        {
            
        }
        Console.WriteLine("Dumping done");
        //Get all important files
        for(int i=0; i <= 1023; i++)
        {
            File.Delete(dir + "/" + i);
            File.Move(dir + "/MiHoYoBinData/" + i, dir+"/"+i);
            Console.WriteLine($"Moving file {dir}/{i}");
        }
        Directory.Delete(dir + "/MiHoYoBinData",true);
        name = name + "_" + VER;
        Dictionary<ulong, string> finalTextMap = new Dictionary<ulong, string>();
        string outputDirectory = "output";
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        Console.WriteLine("Starting parse!");
        string outputFilePath = Path.Combine(outputDirectory+$"/{VER}", name+".json");
        foreach (var file in Directory.GetFiles(dir))
        {
            FileParser parser = new FileParser();
            Dictionary<ulong, string> entries = parser.ParseTextmapFile(file);
            finalTextMap.AddRange(entries);
        }
        Console.WriteLine($"Done {name}!");
        using (StreamWriter outputFile = new StreamWriter(outputFilePath))
        {
            outputFile.Write(DataToJson(finalTextMap));
        }

    }

    public static void Main(string[] args)
    {
        ParseTextMap("01", "TextMap_CHS");
        ParseTextMap("02", "TextMap_CHT");
        ParseTextMap("03", "TextMap_DE");
        ParseTextMap("04", "TextMap_EN");
        ParseTextMap("05", "TextMap_ES");
        ParseTextMap("06", "TextMap_FR");
        ParseTextMap("07", "TextMap_ID");
        ParseTextMap("08", "TextMap_JP");
        ParseTextMap("14", "TextMap_IT");
/*
        1 > CHS
        2 > CHT
        3 > DE
        4 > EN
        5 > ES
        6 > FR
        7 > ID
        8 > JP
        9 > KR
        10 > PT
        11 > RU
        12 > TH
        13 > VI
        14 > IT
        15 > TR
*/
    }

    private static string DataToJson<T>(T data)
    {
        JsonSerializerSettings settings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore,
        };

        return JsonConvert.SerializeObject(data, settings);
    }
}
