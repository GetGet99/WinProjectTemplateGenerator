﻿// See https://aka.ms/new-console-template for more information

using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;



static string Input(string prompt)
{
    Console.Write(prompt);
    Console.Write(": ");
    return Console.ReadLine() ?? "";
}

var templatePath = typeof(JustARandomType).Assembly.Location;
// project path
templatePath = templatePath[..templatePath.LastIndexOf("WinProjectTemplateGenerator")];
// solution path
templatePath = templatePath[..templatePath.LastIndexOf("WinProjectTemplateGenerator")];
templatePath = Path.Combine(
    templatePath[..templatePath.LastIndexOf("WinProjectTemplateGenerator")],
    "ProjectTemplate"
);
var newName = Input("New Project Name");
var newNamespace = Input("New Project Namespace (Empty for same)");
if (string.IsNullOrWhiteSpace(newNamespace)) newNamespace = newName;
var dest = Input("New Project Location");


(string GuidTemplateName, Guid Guid)[] guids = [
    ("SHARD_PROJECT_GUID", Guid.NewGuid()),
    ("UWP_PROJECT_GUID", Guid.NewGuid()),
    ("WASDK_PROJECT_GUID", Guid.NewGuid()),
    ("SOLUTION_GUID", Guid.NewGuid()),
];
void Warn(string warning)
{
    Console.WriteLine(new WarningException(warning).ToString());
}
const string PROJECT_NAME = "PROJECT_NAME";
void RenamePath(string path)
{
    foreach (var fn in Directory.EnumerateFiles(path))
    {
        try
        {
            var original = File.ReadAllText(fn);
            var @new = original.Replace("PROJECT_NAME", newName);
            @new = @new.Replace("PROJNAMESPACE_NAME", newNamespace);
            foreach (var (name, guid) in guids)
            {
                @new = @new.Replace(name, guid.ToString());
            }
            if (@new != original)
            {
                File.WriteAllText(fn, @new);
                Console.WriteLine($"Successfully updated {fn}");
            }
            else
                Console.WriteLine($"Content Remained the same {fn}");
        }
        catch (Exception e)
        {
            Warn($"Could not read/write {fn}: {e.Message}");
        }
        if (fn.Contains(PROJECT_NAME))
        {
            var newfn = fn.Replace(PROJECT_NAME, newName);
            File.Move(fn, newfn);
            Console.WriteLine($"Successfully renamed {fn} -> {newfn}");
        }
    }
    foreach (var dir in Directory.EnumerateDirectories(path))
    {
        if (dir.Contains("bin")) continue;
        if (dir.Contains("obj")) continue;
        if (dir.Contains("/.")) continue;
        if (dir.Contains("\\.")) continue;
        if (dir.Contains(PROJECT_NAME))
        {
            var newdir = dir.Replace(PROJECT_NAME, newName);
            Directory.Move(dir, newdir);
            Console.WriteLine($"Successfully renamed {dir} -> {newdir}");
            RenamePath(newdir);
        }
        else
            RenamePath(dir);
    }
}

Debug.Assert(!PROJECT_NAME.Contains(newName));
Debug.Assert(!newName.Contains(PROJECT_NAME));
Debug.Assert(!dest[..1].Contains(PROJECT_NAME));
var newdir = Path.Combine(dest, newName);
CopyFilesRecursively(templatePath, newdir);
RenamePath(newdir);

// Reference: https://stackoverflow.com/a/3822913
static void CopyFilesRecursively(string sourcePath, string targetPath)
{
    //Now Create all of the directories
    foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
    {
        Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
    }

    //Copy all the files & Replaces any files with the same name
    foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories))
    {
        File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
    }
}

class JustARandomType { }