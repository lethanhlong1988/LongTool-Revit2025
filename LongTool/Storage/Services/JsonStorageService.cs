using Autodesk.Revit.DB;
using LongTool.Storage.Models;
using System;
using System.IO;
using System.Text.Json;

namespace LongTool.Storage.Services;

public static class JsonStorageService
{
    public static void Save(
        Document document,
        ElementData data)
    {
        string databaseFolder =
            @"C:\Users\letha\Desktop\Long\Hoc Hanh\REVIT API\Bai 4\LongTool\LongTool\Database";


        string projectName =
            GetProjectName(document);


        string projectFolder =
            Path.Combine(
                databaseFolder,
                projectName);


        Directory.CreateDirectory(
            projectFolder);


        string fileName =
            $"{data.Category}_{data.Id}.json";


        string filePath =
            Path.Combine(
                projectFolder,
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

    // Update method to update the existing JSON file with new data
    public static void Update(
        Document document,
        ElementData data)
    {
        Save(document, data);
    }


    private static string GetProjectName(
        Document document)
    {
        string path =
            document.PathName;


        if (string.IsNullOrEmpty(path))
        {
            return "Untitled_Project";
        }


        string fileName =
            Path.GetFileNameWithoutExtension(
                path);


        return fileName;
    }
}