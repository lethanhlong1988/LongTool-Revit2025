using LongTool.Storage.Models;
using System.IO;
using System.Text.Json;

namespace LongTool.Storage.Services;

public static class JsonStorageService
{
    private static readonly string FolderPath =
        @"C:\LongTool\Database";


    public static void Save(ElementData data)
    {
        Directory.CreateDirectory(FolderPath);


        string fileName =
            $"{data.Category}_{data.Id}.json";


        string filePath =
            Path.Combine(
                FolderPath,
                fileName);


        string json =
            JsonSerializer.Serialize(
                data,
                new JsonSerializerOptions
                {
                    WriteIndented = true
                });


        File.WriteAllText(
            filePath,
            json);
    }
}