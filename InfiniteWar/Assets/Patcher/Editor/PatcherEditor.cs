using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.IO.Compression;

public class PatcherEditor : Editor {

	[MenuItem("Patcher/PC/BuildAsset")]
    static void BuildPCAsset()
    {
        BuildAsset(BuildTarget.StandaloneWindows);
    }

    [MenuItem("Patcher/Android/BuildAsset")]
    static void BuildAndroidAsset()
    {
        BuildAsset(BuildTarget.Android);
    }
    [MenuItem("Patcher/IOS/BuildAsset")]
    static void BuildIOSAsset()
    {
        BuildAsset(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Patcher/PC/Zip")]
    static void BuildPCZip()
    {
        BuildZip(BuildTarget.StandaloneWindows);
    }
    [MenuItem("Patcher/Android/Zip")]
    static void BuildAndroidZip()
    {
        BuildZip(BuildTarget.Android);
    }

    [MenuItem("Patcher/IOS/Zip")]
    static void BuildIOSZip()
    {
        BuildZip(BuildTarget.iOS);
    }

    [MenuItem("Patcher/DeletePersist")]
    static void DeletePersist()
    {
        if (Directory.Exists(Application.persistentDataPath))
            Directory.Delete(Application.persistentDataPath,true);
        PlayerPrefs.DeleteAll();
    }

    static void BuildZip(BuildTarget target)
    {
        string outPath = Application.dataPath + "/Patcher/Resources/game.bytes";
        if (File.Exists(outPath))
            File.Delete(outPath);
        ZipStorer zip = ZipStorer.Create(outPath, target.ToString());
        string path = GetPath(target);
        string di = "Assets/Patcher/ABs/" + path;
        string[] fs = Directory.GetFiles(di);
        foreach(var f in fs)
        {
            
            if (f.EndsWith(".meta")
                || f.EndsWith(".manifest")
                )
                continue;
            string szName = Path.GetFileNameWithoutExtension(f);
            if (path == szName)
                continue;
            zip.AddFile(ZipStorer.Compression.Store, f, szName, szName);

        }
        zip.Close();
        AssetDatabase.Refresh();
    }

    static string GetPath(BuildTarget target)
    {
        string Path = string.Empty;
        switch (target)
        {
            case BuildTarget.Android:
                Path = Patcher.GetABsPath(RuntimePlatform.Android);
                break;
            case BuildTarget.iOS:
                Path = Patcher.GetABsPath(RuntimePlatform.IPhonePlayer);
                break;
            default:
                Path = Patcher.GetABsPath(RuntimePlatform.WindowsPlayer);
                break;
        }
        return Path;
    }
    static string  GetBuildEnd(string f, string folder ,out string fileName)
    {
        string ret = null;
        fileName = null;
        if (f.EndsWith(".bytes"))
        {

            ret = ".bytes";
        }

        if (f.EndsWith(".prefab"))
        {

            ret = ".prefab";
        }

        if (f.EndsWith(".unity"))
        {

            ret = ".unity";
        }

        if (!string.IsNullOrEmpty(ret))
        {
            string szName = System.IO.Path.GetFileNameWithoutExtension(f);
            fileName = folder + "/" + szName + ret;
            ret = szName;
        }
        return ret;
        
    }
    static void AddAssets(List<AssetBundleBuild> buildMap, string path,string folder)
    {
        
        string[] files = Directory.GetFiles(path);
        foreach(var f in files)
        {
            string filePath = null;
            string ret = GetBuildEnd(f, folder, out filePath);
            if (!string.IsNullOrEmpty(ret))
            {
                AssetBundleBuild ab = new AssetBundleBuild();
                ab.assetBundleName = ret;
                ab.assetNames = new string[] { filePath };
                buildMap.Add(ab);
            }
        }
      string[] subs =  Directory.GetDirectories(path);
        foreach(var s in subs)
        {
            string subName = System.IO.Path.GetFileNameWithoutExtension(s);
            AddAssets(buildMap, s, folder + "/" + subName);
        }
    }
    static void BuildAsset(BuildTarget target)
    {
        string Path = GetPath(target);
        string outPath = "Assets/Patcher/ABs/" + Path;
        if (!Directory.Exists(outPath))
            Directory.CreateDirectory(outPath);
        List<AssetBundleBuild> buildMap = new List<AssetBundleBuild>();
        string inpath = "Assets/Patcher/Prefabs";
        AddAssets(buildMap, inpath, "Assets/Patcher/Prefabs");
        AssetBundleManifest manifest = BuildPipeline.BuildAssetBundles(outPath,buildMap.ToArray(), BuildAssetBundleOptions.None, target);
        if(null != manifest)
        {
           
            Patcher.PatcherElem elem = new Patcher.PatcherElem();
            string[] abs = manifest.GetAllAssetBundles();
            foreach(var ab in abs)
            {
                Patcher.PatcherElem.Elem e = new Patcher.PatcherElem.Elem();
                e.szName = ab;
                e.mVersion = manifest.GetAssetBundleHash(ab).ToString();
                e.mDepends = manifest.GetAllDependencies(ab);
                string filePath = outPath + "/" + ab;
                byte[] bs = File.ReadAllBytes(filePath);
                e.mLength = bs.Length;
                elem.AddElem(e);
            }
            string manifestPath = Application.dataPath + "/Patcher/ABs/" + Path;
            elem.Serialize(manifestPath + "/Pather");
        }
        AssetDatabase.Refresh();
    }
}
