#if UNITY_EDITOR
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities;
using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Callbacks;
using UnityEditor.Build.Reporting;
using Newtonsoft.Json;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

[HideReferenceObjectPicker]
public class PublishWindow : OdinEditorWindow
{
    [BoxGroup("Configuration", showLabel: false, Order = -15)]
    [HideLabel, TitleGroup("Configuration/Options", "Publisher Settings", TitleAlignments.Centered, horizontalLine: true)]
    public enum Publisher
    {
        None,
        [LabelText("azgames.io")] AzGames,
        [LabelText("1games.io")] OneGames
    }

    [TitleGroup("Configuration/Options")]
    [EnumToggleButtons, OnValueChanged("SetPublisher"), PropertyOrder(-14)]
    [LabelText("Select Publisher")]
    public Publisher publisher;

    [TitleGroup("Configuration/Options")]
    [ShowInInspector, OnInspectorInit("@companyName = GetCompanyName()"), ShowIf("@publisher == Publisher.None"), 
     OnValueChanged("SetCompanyName"), PropertyOrder(-13)]
    [LabelText("Company Name")]
    public string companyName;
    
    [TitleGroup("Configuration/Options")]
    [ShowInInspector, OnInspectorInit("@productName = GetProductName()"), OnValueChanged("SetProductName"), PropertyOrder(-12)]
    [LabelText("Product Name")]
    public string productName;
    
    [BoxGroup("Configuration", showLabel: false, Order = -15)]
    [TitleGroup("Configuration/Naming", "Build Name Format", TitleAlignments.Centered, horizontalLine: true)]
    [EnumToggleButtons, PropertyOrder(-11)]
    [LabelText("Build Name Option")]
    public BUILD_NAME_OPTION buildNameOption = BUILD_NAME_OPTION.HAS_BUILD_VERSION;
    
    [TitleGroup("Configuration/Naming")]
    [ShowInInspector, PropertyOrder(-10)]
    [LabelText("Separator")]
    public string separator = "_";
    
    [TitleGroup("Configuration/Options")]
    [ShowInInspector, ValueDropdown("GetAvailableTemplates"), OnInspectorInit("@webglTemplate = GetWebGLTemplate()"), OnValueChanged("SetWebGLTemplate"), PropertyOrder(-10)]
    [LabelText("WebGL Template")]
    public string webglTemplate = "Default";

    [BoxGroup("Configuration", showLabel: false, Order = -15)]
    [TitleGroup("Configuration/Server", "Local Host", TitleAlignments.Centered, horizontalLine: true)]
    [ShowInInspector, OnInspectorInit("@localHost = GetLocalHost()"), OnValueChanged("SetLocalHost"), PropertyOrder(-9)]
    [LabelText("Local URL")]
    public string localHost = "";

    [BoxGroup("Build Information", showLabel: false, Order = -5)]
    [HideLabel, TitleGroup("Build Information/Details", "Build Details", TitleAlignments.Centered, horizontalLine: true)]
    [DisplayAsString, ShowInInspector, PropertyOrder(-4)]
    [LabelText("File Name")]
    public string lastBuildFileName
    {
        get
        {
            GUIHelper.RequestRepaint();
            return GetLastBuildFileName();
        }
        set { }
    }

    [TitleGroup("Build Information/Details")]
    [DisplayAsString, ShowInInspector, PropertyOrder(-3)]
    [LabelText("Build Time")]
    public string lastBuildTime
    {
        get
        {
            GUIHelper.RequestRepaint();
            return HasBuildBefore() ? GetLastBuildTime().ToString("dd/MM/yyyy hh:mm:ss tt") : "...";
        }
        set { }
    }

    [TitleGroup("Build Information/Details")]
    [InlineButton("IncreaseBuildCount", "", Icon = SdfIconType.PlusCircle)]
    [InlineButton("DecreaseBuildCount", "", Icon = SdfIconType.DashCircle)]
    [InlineButton("ResetBuildCount", Icon = SdfIconType.ArrowCounterclockwise, Label = "")]
    [DisplayAsString, ShowInInspector, PropertyOrder(-2)]
    [LabelText("Build Number")]
    public int buildCount
    {
        get
        {
            GUIHelper.RequestRepaint();
            return GetDayBuildNumber() - 1;
        }
        set{ }
    }

    private void IncreaseBuildCount() 
    {
        EditorPrefs.SetInt(dayBuildNumberKey, GetDayBuildNumber() + 1);
    }

    private void DecreaseBuildCount() 
    {
        EditorPrefs.SetInt(dayBuildNumberKey, Mathf.Max(GetDayBuildNumber() - 1, 1));
    }

    [BoxGroup("Build Location", showLabel: false, Order = 15)]
    [HideLabel, TitleGroup("Build Location/Path", "Output Path", TitleAlignments.Centered, horizontalLine: true)]
    [ShowInInspector, FolderPath, PropertyOrder(16)]
    [OnValueChanged("OnSavePathChanged")]
    [LabelText("Save Path")]
    public string savePath
    {
        get
        {
            GUIHelper.RequestRepaint();
            return GetSelectedPath();
        }
    }
    
    private void OnSavePathChanged(string path)
    {
        SetSelectedPath(path);
    }

    [TitleGroup("Build Location/Path")]
    [DisplayAsString, ShowInInspector, PropertyOrder(17)]
    [LabelText("Build Name")]
    public string previewBuildName
    {
        get
        {
            GUIHelper.RequestRepaint();
            return GetPreviewBuildName();
        }
        set { }
    }

    [TitleGroup("Build Location/Path")]
    [DisplayAsString, ShowInInspector, PropertyOrder(18)]
    [LabelText("URL")]
    public string url
    {
        get
        {
            GUIHelper.RequestRepaint();
            return HasBuildBefore() ? $"{localHost}/{lastBuildFileName}" : "...";
        }
        set { }
    }

    private static string lastBuildTimeKey = "";
    private static string dayBuildNumberKey = "";
    private bool hasProductName;
    private bool hasBuildVersion;

    [Serializable]
    public class BuildStats
    {
        [DisplayAsString, LabelText("Build Size")]
        public string totalSize = "...";
        
        [DisplayAsString, LabelText("Build Time")]
        public string totalTime = "...";
        
        [DisplayAsString, LabelText("Warnings")]
        public string totalWarnings = "...";
        
        [DisplayAsString, LabelText("Errors")]
        public string totalErrors = "...";

        public BuildStats(string totalSize, string totalTime, string totalWarnings, string totalErrors)
        {
            this.totalSize = totalSize;
            this.totalTime = totalTime;
            this.totalWarnings = totalWarnings;
            this.totalErrors = totalErrors;
        }
    }

    [Flags]
    public enum BUILD_NAME_OPTION
    {
        [LabelText("Product Name")]
        HAS_PRODUCT_NAME = 1,
        [LabelText("Build Version")]
        HAS_BUILD_VERSION = 2
    }

    [BoxGroup("Build Statistics", showLabel: false, Order = 10)]
    [TitleGroup("Build Statistics/Details", "Build Statistics", TitleAlignments.Centered, horizontalLine: true)]
    [HideLabel, PropertyOrder(11)]
    public BuildStats buildStats;

    [MenuItem("GmSoft/Fast Build Tool &#b")]
    private static void OpenWindow()
    {
        var window = GetWindow<PublishWindow>();
        window.titleContent = new GUIContent()
        {
            text = "Fast Build Tool",
            image = EditorGUIUtility.IconContent("BuildSettings.WebGL").image
        };
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(700, 700);
        lastBuildTimeKey = $"{GetProductName()}_last_build_time";
        dayBuildNumberKey = $"{GetProductName()}_day_build_number";


        if (DateTime.Now.Date > GetLastBuildTime().Date)
        {
            ResetDayBuildNumber();
        }
    }

    [OnInspectorInit]
    private void InspectorInit()
    {
        string buildData = EditorPrefs.GetString($"{GetProductName()}_build_stats", "");
        if (!string.IsNullOrEmpty(buildData))
        {
            buildStats = JsonConvert.DeserializeObject<BuildStats>(buildData);
        }
    }

    public string GetLocalHost()
    {
        return EditorPrefs.GetString($"{GetProductName()}_build_local_host", "http://gamelocal.com");
    }

    private void SetLocalHost(string value)
    {
        EditorPrefs.SetString($"{GetProductName()}_build_local_host", value);
    }

    public string GetWebGLTemplate()
    {
        return EditorPrefs.GetString($"{GetProductName()}_webgl_template", "Default");
    }

    private void SetWebGLTemplate(string value)
    {
        EditorPrefs.SetString($"{GetProductName()}_webgl_template", value);
    }

    private IEnumerable<string> GetAvailableTemplates()
    {
        var templates = new List<string>();
        
        // Add default Unity built-in templates
        templates.Add("Default");
        templates.Add("Minimal");
        templates.Add("PWA");
        
        // Try to find Unity installation templates
        string templatesPath = Path.Combine(EditorApplication.applicationPath, "..", "PlaybackEngines", "WebGLSupport", "BuildTools", "WebGLTemplates");
        if (Directory.Exists(templatesPath))
        {
            var builtInTemplates = Directory.GetDirectories(templatesPath)
                .Select(dir => Path.GetFileName(dir))
                .Where(name => !string.IsNullOrEmpty(name) && !templates.Contains(name));
            templates.AddRange(builtInTemplates);
        }
        
        // Add project templates
        string projectTemplatesPath = Path.Combine(Application.dataPath, "WebGLTemplates");
        if (Directory.Exists(projectTemplatesPath))
        {
            var projectTemplates = Directory.GetDirectories(projectTemplatesPath)
                .Select(dir => Path.GetFileName(dir))
                .Where(name => !string.IsNullOrEmpty(name));
            templates.AddRange(projectTemplates);
        }
        
        return templates.Count > 0 ? templates : new[] { "Default", "2020.3-gmsoft-v4" };
    }

    private void SetCompanyName(string value)
    {
        PlayerSettings.companyName = value;
    }

    private void SetPublisher(Publisher publisher)
    {
        if (publisher == Publisher.None) return;
        PlayerSettings.companyName = publisher == Publisher.AzGames ? "azgames.io" : "1games.io";
        companyName = PlayerSettings.companyName;
    }

    public void SetProductName(string value)
    {
        PlayerSettings.productName = value;
    }

    protected override void OnGUI()
    {
        lastBuildTimeKey = $"{GetProductName()}_last_build_time";
        dayBuildNumberKey = $"{GetProductName()}_day_build_number";
        base.OnGUI();
    }

    [PostProcessBuild(1)]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {

    }

    private string GetCompanyName()
    {
        return PlayerSettings.companyName;
    }

    private static string GetProductName()
    {
        return PlayerSettings.productName;
    }

    [ButtonGroup("Actions", Order = 100)]
    [Button("Build", ButtonSizes.Large, Icon = SdfIconType.Upload)]
    [PropertySpace(SpaceBefore = 20)]
    private void Build()
    {
        string path = EditorUtility.SaveFolderPanel("Choose Location of Built Game", GetSelectedPath(), "");
        if (string.IsNullOrEmpty(path)) return;
        SetSelectedPath(path);
        string savePath = Path.Combine(path, GetPreviewBuildName());
        if (!Directory.Exists(savePath))
        {
            Directory.CreateDirectory(savePath);
        }
        PlayerSettings.bundleVersion = $"{GetBundleVersion()}";
        
        // Check if template is a project template and add PROJECT: prefix if needed
        string templateToUse = webglTemplate;
        string projectTemplatesPath = Path.Combine(Application.dataPath, "WebGLTemplates");
        if (Directory.Exists(projectTemplatesPath))
        {
            var projectTemplateNames = Directory.GetDirectories(projectTemplatesPath)
                .Select(dir => Path.GetFileName(dir));
            if (projectTemplateNames.Contains(webglTemplate))
            {
                templateToUse = "PROJECT:" + webglTemplate;
            }
        }
        
        PlayerSettings.WebGL.template = templateToUse;
        BuildReport br = BuildPipeline.BuildPlayer(EditorBuildSettings.scenes, savePath, BuildTarget.WebGL, BuildOptions.None);

        if (br.summary.result == BuildResult.Succeeded)
        {
            Debug.Log($"Build completed with a result of 'Succeeded' in {(int)br.summary.totalTime.TotalSeconds} seconds ({(int)br.summary.totalTime.TotalMilliseconds} ms)");
            string buildSize = BytesToString((long)br.summary.totalSize);
            string buildTime = $"{br.summary.totalTime.ToString(@"mm\:ss")}";
            string totalErrors = br.summary.totalErrors.ToString();
            string totalWarnings = br.summary.totalWarnings.ToString();
            buildStats = new BuildStats(buildSize, buildTime, totalWarnings, totalErrors);
            EditorPrefs.SetString($"{GetProductName()}_build_stats", JsonConvert.SerializeObject(buildStats));
            savePath = savePath.Replace("\\", "/");

            string windir = Environment.GetEnvironmentVariable("windir");
            if (string.IsNullOrEmpty(windir.Trim()))
            {
                windir = "C:\\Windows\\";
            }
            if (!windir.EndsWith("\\"))
            {
                windir += "\\";
            }

            FileInfo fileToLocate = null;
            fileToLocate = new FileInfo(savePath);

            ProcessStartInfo pi = new ProcessStartInfo(windir + "explorer.exe");
            pi.Arguments = "/select, \"" + fileToLocate.FullName + "\"";
            pi.WindowStyle = ProcessWindowStyle.Normal;
            pi.WorkingDirectory = windir;

            //Start Process
            Process.Start(pi);

            OnBuildCompleted();
        }
    }

    static string BytesToString(long byteCount)
    {
        string[] suf = { "B", "KB", "MB", "GB", "TB", "PB", "EB" }; //Longs run out around EB
        if (byteCount == 0)
            return "0" + suf[0];
        long bytes = Math.Abs(byteCount);
        int place = Convert.ToInt32(Math.Floor(Math.Log(bytes, 1024)));
        double num = Math.Round(bytes / Math.Pow(1024, place), 1);
        return (Math.Sign(byteCount) * num).ToString() + suf[place];
    }

    [ButtonGroup("Actions")]
    [Button("Preview", ButtonSizes.Large, Icon = SdfIconType.PlayCircle)]
    private void Preview()
    {
        if (!HasBuildBefore())
        {
            Debug.Log($"No previous build file.");
            return;
        }
        Application.OpenURL(url);
    }

    [ButtonGroup("Actions")]
    [Button("Player Settings", ButtonSizes.Large, Icon = SdfIconType.GearFill)]
    private void OpenPlayerSettings()
    {
        SettingsService.OpenProjectSettings("Project/Player");
    }

    [ButtonGroup("Actions")]
    [Button("Clear PlayerPrefs", ButtonSizes.Large, Icon = SdfIconType.Trash)]
    private void ClearPlayerPrefs()
    {
        if (EditorUtility.DisplayDialog("Clear PlayerPrefs", 
            $"Are you sure you want to clear all PlayerPrefs for {GetProductName()}?\n\nThis will reset all game save data for testing.", 
            "Clear", "Cancel"))
        {
            // Clear all PlayerPrefs
            PlayerPrefs.DeleteAll();
        }
    }

    [ButtonGroup("Actions")]
    [Button("Reset Counter", ButtonSizes.Large, Icon = SdfIconType.ArrowCounterclockwise)]
    private void ResetBuildCount()
    {
        ResetDayBuildNumber();
    }

    private string GetPreviewBuildName()
    {
        string previewBuildName = string.Empty;
        if (buildNameOption.HasFlag(BUILD_NAME_OPTION.HAS_PRODUCT_NAME)) previewBuildName += GetBuildProductName();
        if (buildNameOption.HasFlag(BUILD_NAME_OPTION.HAS_PRODUCT_NAME)
            && buildNameOption.HasFlag(BUILD_NAME_OPTION.HAS_BUILD_VERSION)) previewBuildName += separator;
        if (buildNameOption.HasFlag(BUILD_NAME_OPTION.HAS_BUILD_VERSION)) previewBuildName += GetBundleVersion();
        if (string.IsNullOrEmpty(previewBuildName)) previewBuildName = "build-test";
        return previewBuildName;
    }

    public static string GetLastBuildFileName()
    {
        return EditorPrefs.GetString($"{GetProductName()}_last_build_file_name", "");
    }

    public void SetLastBuildFileName(string value)
    {
        EditorPrefs.SetString($"{GetProductName()}_last_build_file_name", value);
    }

    public string GetBundleVersion()
    {
        return DateTime.Now.ToString("yyMMdd") + GetDayBuildNumber().ToString("00");
    }

    private static string GetSelectedPath()
    {
        return EditorPrefs.GetString($"{GetProductName()}_selected_path", "");
    }

    private static void SetSelectedPath(string path)
    {
        EditorPrefs.SetString($"{GetProductName()}_selected_path", path);
    }

    private static string GetBuildProductName()
    {
        return Application.productName.ToLower().Replace(" ", "-");
    }

    private static bool HasBuildBefore()
    {
        return EditorPrefs.HasKey(lastBuildTimeKey);
    }

    private static DateTime GetLastBuildTime()
    {
        DateTime lastBuildTime = DateTime.Now;
        if (HasBuildBefore() && !string.IsNullOrEmpty(lastBuildTimeKey))
        {
            lastBuildTime = DateTime.Parse(EditorPrefs.GetString(lastBuildTimeKey));
        }
        return lastBuildTime;
    }

    private static int GetDayBuildNumber()
    {
        int buildNumber = EditorPrefs.GetInt(dayBuildNumberKey, 1);
        return buildNumber;
    }

    private static void ResetDayBuildNumber()
    {
        EditorPrefs.SetInt(dayBuildNumberKey, 1);
    }

    private void OnBuildCompleted()
    {
        SetLastBuildFileName(GetPreviewBuildName());
        EditorPrefs.SetString(lastBuildTimeKey, DateTime.Now.ToString());
        EditorPrefs.SetInt(dayBuildNumberKey, GetDayBuildNumber() + 1);
    }
}
#endif