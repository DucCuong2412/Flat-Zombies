using Sirenix.OdinInspector;
using Sirenix.Utilities;
#if UNITY_EDITOR
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using Sirenix.Utilities.Editor.Expressions;
#endif
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
public class GmSoftHelper : OdinMenuEditorWindow
{
    public static Object GmSoftPrefab;
    public const string VERSION = "4.1";

    [MenuItem("GmSoft/Help &#g")] //alt + shift + g
    public static void OpenWindow()
    {
        var window = GetWindow<GmSoftHelper>("GmSoft SDK v4.2");
        window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 600);
        GmSoftPrefab = AssetDatabase.LoadMainAssetAtPath("Assets/GmSoft/Prefabs/GmSoft.prefab") as Object;
    }

    [InitializeOnLoadMethod]
    private static void AutoOpen()
    {
        if (PlayerPrefs.GetInt($"GMSOFT_SDK_GETTING_STARTED_{VERSION}", 0) == 0) // mỗi một phiên bản mới lại tự động mở cửa sổ 1 lần
            return;
        PlayerPrefs.SetInt($"GMSOFT_SDK_GETTING_STARTED_{VERSION}", 1);
        EditorApplication.delayCall += () =>
        {
            OpenWindow();
        };
    }

    protected override OdinMenuTree BuildMenuTree()
    {
        OdinMenuTree tree = new OdinMenuTree(supportsMultiSelect: true);
        tree.Config.DrawSearchToolbar = true;
        tree.Add("Home", HelperContent.Home(), SdfIconType.HouseFill);
        tree.Add("Setup", HelperContent.Setup(), SdfIconType.GearFill);
        tree.Add("Advertisement", HelperContent.Advertisement(), SdfIconType.PlayBtnFill);
        tree.Add("Advertisement/Interstitial", HelperContent.AdvertisementInterstitial());
        tree.Add("Advertisement/Reward", HelperContent.AdvertisementReward());
        tree.Add("Analytics", HelperContent.Analytics(), SdfIconType.FileBarGraphFill);
        tree.Add("Analytics/Event", HelperContent.AnalyticsEvent());
        tree.Add("Analytics/EventWithParameter", HelperContent.AnalyticsEventWithParameters());
        tree.Add("Analytics/UIEventTracker", HelperContent.AnalyticsUIEventTracker());
        tree.Add("Analytics/LifeCycleEventTracker", HelperContent.LifeCycleEventTracker());
        tree.Add("API", HelperContent.API(), SdfIconType.Server);
        tree.Add("API/Get Game Image", HelperContent.APIGetGameImage());
        tree.Add("API/Get Logo", HelperContent.APIGetLogo());
        tree.Add("API/Promotion", HelperContent.APIPromotion());
        return tree;
    }
}

public class HelperContent
{
    [HideInInspector]
    private string text;
    public Action ShowGUIAction;

    public static GUIStyle GUIDefault()
    {
        return new GUIStyle()
        {
            richText = true,
            normal = new GUIStyleState()
            {
                textColor = SyntaxHighlighter.TextColor
            }
        };
    }

    [OnInspectorGUI("ShowGUI")]
    private void ShowGUI()
    {
        ShowGUIAction?.Invoke();
    }

    private static void DrawCodeBox(string code)
    {
        Rect rect = SirenixEditorGUI.BeginToolbarBox();
        SirenixEditorGUI.DrawSolidRect(rect, SyntaxHighlighter.BackgroundColor);

        EditorGUI.DrawRect(rect.AlignTop(1f).AddY(-1f), SirenixGUIStyles.BorderColor);
        SirenixEditorGUI.BeginToolbarBoxHeader();
        GUILayout.FlexibleSpace();
        if (SirenixEditorGUI.ToolbarButton("Copy"))
        {
            Clipboard.Copy(code);
        }
        SirenixEditorGUI.EndToolbarBoxHeader();
        GUILayout.BeginVertical();
        GUIStyle style = GUIDefault();
        string content = SyntaxHighlighter.Parse(code);
        Rect currentRect = GUIHelper.GetCurrentLayoutRect();
        GUIContent guiContent = new GUIContent(content);
        EditorGUILayout.SelectableLabel(content, style, GUILayout.Height(style.CalcHeight(guiContent, currentRect.width)));
        GUILayout.EndVertical();
        SirenixEditorGUI.EndToolbarBox();
    }

    private static void DrawLabel(string label, int fontSize = 10)
    {
        GUILayout.BeginVertical();
        string content = label;
        GUIStyle style = new GUIStyle(SirenixGUIStyles.MultiLineLabel);
        style.fontSize = fontSize;
        GUILayout.Label(content, style, Array.Empty<GUILayoutOption>());
        GUILayout.EndVertical();
    }

    public HelperContent(string text)
    {
        this.text = text;
    }

    public HelperContent()
    {
    }

    public static HelperContent Home()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction = () =>
        {
            GUILayout.BeginVertical();
            GUIStyle style = new GUIStyle(SirenixGUIStyles.WhiteLabel); 
            style.alignment = TextAnchor.MiddleCenter;
            style.fontStyle = FontStyle.Bold;
            style.richText = true;
            GUILayout.Label("<size=35>GmSoft SDK v4.2</size>", style);
            GUILayout.EndVertical();
            GUILayout.Space(15);
            DrawLabel("<b>ChangeLog:</b>", 13);
            GUILayout.Space(5);
            DrawChangeLog($"[14/01/2025] v4.2", $"- Integrate GameDistribution SDK.\r\n- Fix Webtemplate.");
            GUILayout.Space(5);
            DrawChangeLog($"[06/12/2024] v4.1", $"- Add new lock scene.\r\n- Fix logic GmSoft.cs.");
            GUILayout.Space(5);
            DrawChangeLog($"[26/11/2024] v4.0", $"- Webtemplate: 2020.3-gmsoft-v4 (Working on <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Unity 2020+</color></b>).\r\n- Firebase: add analytics level.\r\n- Add Promotion.");
            GUILayout.Space(5);
            DrawChangeLog("[10/10/2024] v3.2", "- Integrate LogFPS Firebase.\r\n- Log firebase: showInterAds; showRewardAds; completeRewardAds.");
            GUILayout.Space(5);
            DrawChangeLog("[07/10/2024] v3.1", "- Fix ENABLE ADS FALSE load sence");
            GUILayout.Space(5);
            DrawChangeLog($"[04/10/2024] v3.0", $"- ADS SDK: H5 + Gamedistribution + Weegoo Ads.\r\n- Firebase.\r\n- Webtemplate: gmsoft-v3 / BetterTemplate (Working on <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Unity 2020+</color></b>).");
        };
        return content;
    }

    private static void DrawChangeLog(string label, string content)
    {
        Rect rect = SirenixEditorGUI.BeginToolbarBox();
        SirenixEditorGUI.DrawSolidRect(rect, SyntaxHighlighter.BackgroundColor);

        EditorGUI.DrawRect(rect.AlignTop(1f).AddY(-1f), SirenixGUIStyles.BorderColor);
        GUILayout.BeginVertical();
        string changeContent = $"<b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>{label}</color></b>\r\n{content}";
        Rect currentRect = GUIHelper.GetCurrentLayoutRect();
        GUIContent guiContent = new GUIContent(changeContent);
        GUIStyle changeLogStyle = new GUIStyle(SirenixGUIStyles.MultiLineLabel);
        EditorGUILayout.LabelField(changeContent, changeLogStyle, GUILayout.Height(changeLogStyle.CalcHeight(guiContent, currentRect.width)));
        GUILayout.EndVertical();
        SirenixEditorGUI.EndToolbarBox();
    }

    public static HelperContent Setup()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction = () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>Setup</b>", 18);
            GUILayout.Space(8);
            GUILayout.BeginHorizontal();
            string content = "GmSoft Prefab";
            GUIStyle style = new GUIStyle(SirenixGUIStyles.MultiLineLabel);
            GUIStyle btnStyle = new GUIStyle(SirenixGUIStyles.ButtonRight);
            btnStyle.fixedWidth = 200;
            style.fontSize = 13;
            GUILayout.Label(content, style, Array.Empty<GUILayoutOption>());
            if (GUILayout.Button("Select", btnStyle))
            {
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(@"Assets/GmSoft/Prefabs/GmSoft.prefab");
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            string content2 = "Promotion Demo Prefab";
            style.fontSize = 13;
            GUILayout.Label(content2, style, Array.Empty<GUILayoutOption>());
            if (GUILayout.Button("Select", btnStyle))
            {
                Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(@"Assets/GmSoft/Prefabs/Promotion.prefab");
                EditorGUIUtility.PingObject(Selection.activeObject);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(8);
            if (GUILayout.Button("Open Setup Document")) 
            {
                Application.OpenURL("https://docs.google.com/document/d/18dW5Y76uTIy_LEDKEiWrsiuajHmDZl5YnNk9ji5KoKI");
            }
        };
        
        return content;
    }

    public static HelperContent Advertisement()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction = () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>Triển khai quảng cáo trong trò chơi</b>", 18);
            GUILayout.Space(5);
            DrawLabel("Có 2 loại quảng cáo chính là quảng cáo <b>Interstitial</b> và quảng cáo <b>Reward</b>.", 13);
            DrawLabel("Quảng cáo <b>Interstitial</b> thường được đặt ở những nút nhấn trong màn hình <b><i>Game Over</i></b> hoặc <b><i>Level Completed</i></b> để đảm bảo trải nghiệm của người dùng.", 13);
            DrawLabel("<b>Lưu ý:</b>", 13);
            DrawLabel(" - Chỉ hiển thị quảng cáo khi có tương tác của người chơi.", 13);
            DrawLabel(" - Quảng cáo phải được đặt bên ngoài Gameplay.", 13);
            DrawLabel(" - Đảm bảo rằng trò chơi đã dừng và âm thanh trong game được tắt khi phát quảng cáo.", 13);
            DrawLabel(" - Nút nhấn phát quảng cáo reward phải có icon để thông báo cho người chơi.", 13);
            GUILayout.Space(5);
            DrawLabel($"Có thể triển khai thêm 1 số tính năng khác trong khi <b>pause</b> và <b>resume</b> game ở <color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>AdvertisementManager.cs</color>", 13);
            DrawCodeBox("    private void OnPauseGame()\r\n    {\r\n        AudioListener.volume = 0;\r\n    }\r\n\r\n    private void OnResumeGame()\r\n    {\r\n        Application.ExternalEval(\"window.focus();\");\r\n        AudioListener.volume = 1f;\r\n        ResumeAction?.Invoke();\r\n        ResumeAction = null;\r\n        HideMassagePanel();\r\n    }");
            GUILayout.Space(5);
            DrawLabel($"Với những game giới hạn thời gian giữa 2 lần quảng cáo reward, sử dụng <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>AdvertisementManager.Instance.GetNextRewardAdsTime</color></b> để lấy số giây còn lại cho quảng cáo reward tiếp theo.", 13);
            DrawCodeBox("float nextRewardAdsTime = AdvertisementManager.Instance.GetNextRewardAdsTime();");
            GUILayout.Space(5);
            DrawLabel($"Sử dụng <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>AdvertisementManager.Instance.CanShowRewardAds</color></b> để kiểm tra xem quảng cáo reward đã sẵn sàng hay chưa.", 13);
            DrawCodeBox("bool canShowReward = AdvertisementManager.Instance.CanShowRewardAds();");
        };
        return content;
    }

    public static HelperContent AdvertisementInterstitial()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction = () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>Triển khai quảng cáo Interstitial</b>", 18);
            GUILayout.Space(5);
            DrawLabel($"Sử dụng <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>AdvertisementManager.Instance.ShowAds</color></b> để hiển thị quảng cáo Interstitial.", 13);
            GUILayout.Space(5);
            DrawLabel("Ví dụ:", 13);
            GUILayout.Space(3);
            DrawCodeBox("    private Button playAgainBtn;\r\n\r\n    private void Start() \r\n    {\r\n        playAgainBtn.onClick.AddListener(PlayAgain);\r\n    }\r\n\r\n    private void PlayAgain() \r\n    {\r\n        AdvertisementManager.Instance.ShowAds(ReloadLevel);\r\n    }\r\n\r\n    private void ReloadLevel() \r\n    {\r\n       \r\n    }");
            GUILayout.Space(3);
            DrawLabel($"Tham số truyền vào <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>ReloadLevel</color></b> trong <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>AdvertisementManager.Instance.ShowAds</color></b> là action khi người chơi xem xong quảng cáo, skip hoặc hiển thị quảng cáo không thành công.", 13);
        };
        return content;
    }

    public static HelperContent AdvertisementReward()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction = () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>Triển khai quảng cáo Reward</b>", 18);
            GUILayout.Space(5);
            DrawLabel($"Sử dụng <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>AdvertisementManager.Instance.ShowReward</color></b> để hiển thị quảng cáo Reward.", 13);
            GUILayout.Space(5);
            DrawLabel("Ví dụ:", 13);
            GUILayout.Space(3);
            DrawCodeBox("public Button reviveBtn;\r\n\r\nprivate void Start() \r\n{\r\n        reviveBtn.onClick.AddListener(TryRevive);\r\n}\r\n\r\nprivate void TryRevive() \r\n{\r\n        AdvertisementManager.Instance.ShowReward(RevivePlayer);\r\n}\r\n\r\nprivate void RevivePlayer() \r\n{\r\n        \r\n}");
            DrawLabel($"Để nhận được phần thưởng, người chơi phải xem hết quảng cáo mà không nhấn <b>skip</b>(bỏ qua). Khi người chơi xem hết quảng cáo mà không skip, action <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>RevivePlayer</color></b> sẽ được thực hiện.", 13);
            GUILayout.Space(3);
        };
        return content;
    }

    public static HelperContent Analytics()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction += () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>Triển khai Analytics</b>", 18);
            GUILayout.Space(5);
            DrawLabel($"<b>Analytics</b> <size=13>là công cụ ghi lại sự kiện nhằm thu thập dữ liệu người chơi, xác định hành vi của người chơi để đưa ra những thay đổi phù hợp.</size>", 13);
            DrawLabel($"<b>Sự kiện (event)</b> <size=13>là những gì đang diễn ra trong trò chơi. vd: thao tác người dùng, các tương tác trong game hoặc kết quả của 1 màn chơi,...</size>", 13);
            DrawLabel($"Analytics tự động ghi lại một số sự kiện mà không cần phải thêm bất kì mã nào để ghi lại: <b>first_visit</b>(lần đầu tiên người dùng chơi trò chơi), <b>session_start</b>(khi người dùng tương tác với trò chơi),...", 13);
            GUILayout.Space(5);
            DrawLabel($"<b>Sự kiện đề xuất</b>", 14);
            DrawLabel($" - <b>level_end:</b> hoàn thành một cấp độ trong trò chơi.", 13);
            DrawLabel($" - <b>level_start:</b> bắt đầu một cấp độ mới trong trò chơi.", 13);
            DrawLabel($" - <b>level_up:</b> lên cấp trong trò chơi.", 13);
            DrawLabel($" - <b>play_again:</b> chơi lại 1 cấp độ trong trò chơi.", 13);
            DrawLabel($" - <b>unlock_achievement:</b> mở khoá một thành tựu.", 13);
            DrawLabel($" - <b>buy_item:</b> mua vật phẩm trong trò chơi.", 13);
            GUILayout.Space(5);
            DrawLabel($"<b>Cấp độ thu thập dữ liệu người chơi</b>", 14);
            DrawLabel($" - Có một tham số truyền vào khi ghi sự kiện là <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>level(cấp độ)</color></b> mặc định level = 0.", 13);
            DrawLabel($" - Những event liên quan đến gameplay vd: <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>level_start, play_again, level_end,...</color></b> nên được đặt ở <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>level 1</color></b> nhằm phân tích và điều chỉnh cấp độ phù hợp hơn với người chơi.", 13);
            DrawLabel($" - Những event liên quan đến giao diện người dùng: <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>click_button, view_item, select_item, click_more_games...</color></b> nên được đặt ở <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>level 2</color></b>.", 13);
            DrawLabel($" - Tùy theo từng trò chơi khác nhau có thể đánh cấp độ khác nhau. <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Cấp độ càng thấp thì độ ưu tiên càng cao.</color></b>", 13);
        };
        return content;
    }

    public static HelperContent AnalyticsEvent()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction += () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>Event (sự kiện)</b>", 18);
            GUILayout.Space(5);
            DrawLabel($"Sử dụng <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.GreenText)}>gmsoft.Analytics.LogEvent</color></b> để ghi lại sự kiện.", 13);
            DrawLabel($"Ví dụ:", 13);
            GUILayout.Space(5);
            DrawCodeBox("private void OnLevelCompleted() \r\n{\r\n        gmsoft.Analytics.LogEvent(\"level_completed\", 1);\r\n}");
            DrawLabel($"hoặc:", 13);
            DrawCodeBox("private void OnLevelCompleted(int level)\r\n{\r\n        gmsoft.Analytics.LogEvent(\"level_completed_\" + level, 1);\r\n}");
            GUILayout.Space(5);
            DrawLabel($"Khi người chơi hoàn thành 1 level tiến hành ghi lại sự kiện hoàn thành màn chơi cho người chơi. Tham số <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>level_completed</color></b> là sự kiện cần ghi lại, <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>1</color></b> là cấp độ analytics.", 13);
        };
        return content;
    }

    public static HelperContent AnalyticsEventWithParameters()
    {
        HelperContent content = new HelperContent();
        return content;
    }

    public static HelperContent AnalyticsUIEventTracker()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction += () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>UIEventTracker (Theo dõi sự kiện trên UI)</b>", 18);
            GUILayout.Space(5);
            DrawLabel($"Gắn script lên <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Button</color></b> (nút nhấn) hoặc <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Image</color></b> (ảnh) để theo dõi hành vi của người chơi.", 13);
            DrawLabel("<b>Các bước thực hiện:</b>", 13);
            DrawLabel(" - Chọn button hoặc image cần gắn event.", 13);
            DrawLabel($" - Add Component <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>UIEventTracker</color></b>.", 13);
            DrawLabel($" - Nhấn vào dấu <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>+</color></b> ở Events để add event.", 13);
            DrawLabel($" - PointerState là trạng thái của con trỏ chuột: <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Enter</color></b> (khi đưa chuột vào UI), <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Exit</color></b> (đưa chuột ra khỏi UI), <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Down</color></b> (Nhấn chuột xuống), <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Up</color></b> (Thả chuột ra) hoặc <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Click</color></b>.", 13);
            DrawLabel($" - Analytics level là cấp độ analytics, mặc định là 2.", 13);
            DrawLabel($" - Name là tên event cần ghi lại. vd: click_button_play, click_button_more_games,...", 13);
            DrawLabel($" - Has Params tích khi cần ghi lại events với những tham số khác.", 13);
        };
        return content;
    }

    public static HelperContent LifeCycleEventTracker()
    {
        HelperContent content = new HelperContent();
        content.ShowGUIAction += () =>
        {
            GUILayout.Space(5);
            DrawLabel("<b>LifeCycleEventTracker (Theo dõi vòng đời 1 game object)</b>", 18);
            GUILayout.Space(5);
            DrawLabel($"Gắn script lên <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>game object</color></b> (nút nhấn) cần theo dõi vòng đời.", 13);
            DrawLabel("<b>Các bước thực hiện:</b>", 13);
            DrawLabel(" - Chọn game object cần gắn event.", 13);
            DrawLabel($" - Add Component <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>LifeCycleEventTracker</color></b>.", 13);
            DrawLabel($" - Nhấn vào dấu <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>+</color></b> ở Events để add event.", 13);
            DrawLabel($" - Life Cycle bao gồm: <b><color=#{ColorUtility.ToHtmlStringRGBA(SyntaxHighlighter.TextColor)}>Awake, Start, Enable, Disable, Pause, Unpause, Destroy</color></b>.", 13);
            DrawLabel($" - Name là tên event cần ghi lại. vd: player_awake, menu_pause,...", 13);
        };
        return content;
    }

    public static HelperContent API()
    {
        HelperContent content = new HelperContent();
        return content;
    }

    public static HelperContent APIGetGameImage()
    {
        HelperContent content = new HelperContent();
        return content;
    }

    public static HelperContent APIGetLogo()
    {
        HelperContent content = new HelperContent();
        return content;
    }

    public static HelperContent APIPromotion()
    {
        HelperContent content = new HelperContent();
        return content;
    }
}

public class SyntaxHighlighter
{
    public static Color BackgroundColor = new Color(0.118f, 0.118f, 0.118f, 1f);
    public static Color TextColor = new Color(0.863f, 0.863f, 0.863f, 1f);
    public static Color KeywordColor = new Color(0.337f, 0.612f, 0.839f, 1f);
    public static Color IdentifierColor = new Color(0.306f, 0.788f, 0.69f, 1f);
    public static Color CommentColor = new Color(0.341f, 0.651f, 0.29f, 1f);
    public static Color LiteralColor = new Color(0.71f, 0.808f, 0.659f, 1f);
    public static Color StringLiteralColor = new Color(0.839f, 0.616f, 0.522f, 1f);

    public static Color RedText = new Color(255 / 255f, 24 / 255f, 8 / 255f, 1f);
    public static Color GreenText = new Color(24 / 255f, 245 / 255f, 32 / 255f, 1f);
    public static Color YellowText = new Color(240 / 255f, 236 / 255f, 26 / 255f, 1f);
    private Tokenizer tokenizer;
    private StringBuilder result = new StringBuilder();
    private List<SyntaxHighlighter.TokenBuffer> statement = new List<SyntaxHighlighter.TokenBuffer>();
    private int textPosition;

    public static string Parse(string text) => new SyntaxHighlighter().ParseText(text);

    public string ParseText(string text)
    {
        this.result.Length = 0;
        this.statement.Clear();
        this.textPosition = 0;
        this.tokenizer = new Tokenizer(text)
        {
            TokenizeComments = true,
            TokenizePreprocessors = true
        };
        this.ReadDeclaration();
        return this.result.ToString();
    }

    private void ReadDeclaration()
    {
        Token token = Token.UNKNOWN;
        while (token != Token.EOF)
        {
            token = this.tokenizer.GetNextToken();
            if (token != Token.EOF)
            {
                if (token == Token.COMMENT)
                {
                    this.AppendWhitespace(this.tokenizer.TokenStartedStringPosition);
                    this.Colorize(this.tokenizer.TokenStartedStringPosition, this.tokenizer.ExpressionStringPosition - this.tokenizer.TokenStartedStringPosition, SyntaxHighlighter.CommentColor);
                }
                else if (token == Token.PREPROCESSOR)
                {
                    this.AppendWhitespace(this.tokenizer.TokenStartedStringPosition);
                    this.Append(this.tokenizer.TokenStartedStringPosition, this.tokenizer.ExpressionStringPosition - this.tokenizer.TokenStartedStringPosition);
                }
                else
                {
                    this.statement.Add(new SyntaxHighlighter.TokenBuffer(token, this.tokenizer));
                    if (this.statement[0].Token == Token.LEFT_BRACKET)
                    {
                        if (token == Token.RIGHT_BRACKET)
                        {
                            this.AppendDeclaration(this.statement, ref this.textPosition);
                            this.statement.Clear();
                        }
                    }
                    else if (token == Token.SCOPE_BEGIN)
                    {
                        if (this.statement.Any<SyntaxHighlighter.TokenBuffer>((Func<SyntaxHighlighter.TokenBuffer, bool>)(i => i.Token == Token.LEFT_PARENTHESIS)))
                        {
                            this.AppendMember(this.statement);
                            this.statement.Clear();
                            this.ReadImplementation();
                        }
                        else if (this.statement.Any<SyntaxHighlighter.TokenBuffer>((Func<SyntaxHighlighter.TokenBuffer, bool>)(i => i.Token == Token.CLASS || i.Token == Token.STRUCT || i.Token == Token.INTERFACE)))
                        {
                            this.AppendDeclaration(this.statement, ref this.textPosition);
                            this.statement.Clear();
                        }
                        else
                        {
                            this.AppendMember(this.statement);
                            this.statement.Clear();
                            this.ReadImplementation();
                        }
                    }
                    else if (token == Token.SCOPE_END)
                    {
                        this.AppendMember(this.statement);
                        this.statement.Clear();
                    }
                    else if (token == Token.SEMI_COLON)
                    {
                        this.AppendMember(this.statement);
                        this.statement.Clear();
                    }
                }
            }
            else
                break;
        }
        if (this.statement.Count <= 0)
            return;
        this.AppendDeclaration(this.statement, ref this.textPosition);
        this.statement.Clear();
    }

    private void ReadImplementation()
    {
        Token token = Token.UNKNOWN;
        while (token != Token.EOF && token != Token.SCOPE_END)
        {
            token = this.tokenizer.GetNextToken();
            this.statement.Add(new SyntaxHighlighter.TokenBuffer(token, this.tokenizer));
        }
        this.AppendImplementation(this.statement);
        this.statement.Clear();
    }

    private void AppendDeclaration(
      List<SyntaxHighlighter.TokenBuffer> statementBuffer,
      ref int prevIndex)
    {
        for (int index = 0; index < statementBuffer.Count; ++index)
        {
            SyntaxHighlighter.TokenBuffer buffer = statementBuffer[index];
            this.AppendWhitespace(buffer.StartIndex);
            if (buffer.Token == Token.IDENTIFIER)
            {
                string str = buffer.GetString(this.tokenizer);
                if (TypeExtensions.IsCSharpKeyword(str))
                    this.Colorize(str, SyntaxHighlighter.KeywordColor);
                else
                    this.Colorize(str, SyntaxHighlighter.IdentifierColor);
                prevIndex = buffer.EndIndex;
            }
            else
                this.AppendToken(buffer);
        }
    }

    private void AppendMember(List<SyntaxHighlighter.TokenBuffer> statement)
    {
        for (int index = 0; index < statement.Count; ++index)
        {
            SyntaxHighlighter.TokenBuffer buffer = statement[index];
            this.AppendWhitespace(buffer.StartIndex);
            if (buffer.Token == Token.IDENTIFIER)
            {
                string str = buffer.GetString(this.tokenizer);
                if (TypeExtensions.IsCSharpKeyword(str))
                {
                    this.Colorize(str, SyntaxHighlighter.KeywordColor);
                }
                else
                {
                    switch (index + 1 < statement.Count ? statement[index + 1].Token : Token.UNKNOWN)
                    {
                        case Token.LEFT_PARENTHESIS:
                        case Token.COMMA:
                        case Token.SEMI_COLON:
                        case Token.SIMPLE_ASSIGNMENT:
                        case Token.SCOPE_BEGIN:
                            this.Append(str);
                            break;
                        default:
                            this.Colorize(str, SyntaxHighlighter.IdentifierColor);
                            break;
                    }
                }
                this.textPosition = buffer.EndIndex;
            }
            else
                this.AppendToken(buffer);
        }
    }

    private void AppendImplementation(List<SyntaxHighlighter.TokenBuffer> statement)
    {
        for (int index = 0; index < statement.Count; ++index)
        {
            SyntaxHighlighter.TokenBuffer buffer = statement[index];
            this.AppendWhitespace(buffer.StartIndex);
            if (buffer.Token == Token.IDENTIFIER)
            {
                string str = buffer.GetString(this.tokenizer);
                if (TypeExtensions.IsCSharpKeyword(str))
                    this.Colorize(str, SyntaxHighlighter.KeywordColor);
                else
                    this.result.Append(str);
                this.textPosition = buffer.EndIndex;
            }
            else
                this.AppendToken(buffer);
        }
    }

    private void AppendToken(SyntaxHighlighter.TokenBuffer buffer)
    {
        this.AppendWhitespace(buffer.StartIndex);
        switch (buffer.Token)
        {
            case Token.SIGNED_INT32:
            case Token.UNSIGNED_INT32:
            case Token.SIGNED_INT64:
            case Token.UNSIGNED_INT64:
            case Token.FLOAT32:
            case Token.FLOAT64:
            case Token.DECIMAL:
                this.Colorize(buffer.StartIndex, buffer.Length, SyntaxHighlighter.LiteralColor);
                break;
            case Token.IDENTIFIER:
                string str = this.tokenizer.ExpressionString.Substring(buffer.StartIndex, buffer.Length);
                if (TypeExtensions.IsCSharpKeyword(str))
                {
                    this.Colorize(str, SyntaxHighlighter.KeywordColor);
                    break;
                }
                this.Colorize(str, SyntaxHighlighter.IdentifierColor);
                break;
            case Token.SIZEOF:
            case Token.TRUE:
            case Token.FALSE:
            case Token.RELATIONAL_IS:
            case Token.RELATIONAL_AS:
            case Token.NEW:
            case Token.THIS:
            case Token.BASE:
            case Token.CHECKED:
            case Token.UNCHECKED:
            case Token.DEFAULT:
            case Token.NULL:
            case Token.TYPEOF:
            case Token.VOID:
            case Token.REF:
            case Token.OUT:
            case Token.IN:
            case Token.CLASS:
            case Token.STRUCT:
            case Token.INTERFACE:
            case Token.RETURN:
                this.Colorize(buffer.StartIndex, buffer.Length, SyntaxHighlighter.KeywordColor);
                break;
            case Token.CHAR_CONSTANT:
            case Token.STRING_CONSTANT:
                this.Colorize(buffer.StartIndex, buffer.Length, SyntaxHighlighter.StringLiteralColor);
                break;
            case Token.EOF:
                return;
            case Token.COMMENT:
                this.Colorize(buffer.StartIndex, buffer.Length, SyntaxHighlighter.CommentColor);
                break;
            default:
                this.result.Append(this.tokenizer.ExpressionString, buffer.StartIndex, buffer.Length);
                break;
        }
        this.textPosition = buffer.EndIndex;
    }

    private void Colorize(string text, Color color)
    {
        this.result.Append("<color=#");
        this.result.Append(ColorUtility.ToHtmlStringRGBA(color));
        this.result.Append(">");
        this.Append(text);
        this.result.Append("</color>");
    }

    private void Colorize(int start, int length, Color color)
    {
        this.result.Append("<color=#");
        this.result.Append(ColorUtility.ToHtmlStringRGBA(color));
        this.result.Append(">");
        this.Append(start, length);
        this.result.Append("</color>");
    }

    private void Append(int start, int length)
    {
        this.result.Append(this.tokenizer.ExpressionString, start, length);
        this.textPosition = start + length;
    }

    private void Append(string text)
    {
        this.result.Append(text);
        this.textPosition += text.Length;
    }

    private void AppendWhitespace(int position)
    {
        if (position - this.textPosition <= 0)
            return;
        this.result.Append(this.tokenizer.ExpressionString, this.textPosition, position - this.textPosition);
        this.textPosition = position;
    }

    private struct TokenBuffer
    {
        public Token Token;
        public int StartIndex;
        public int EndIndex;

        public int Length => this.EndIndex - this.StartIndex;

        public TokenBuffer(Token token, Tokenizer tokenizer)
        {
            this.Token = token;
            this.StartIndex = tokenizer.TokenStartedStringPosition;
            this.EndIndex = tokenizer.ExpressionStringPosition;
        }

        public string GetString(Tokenizer tokenizer) => tokenizer.ExpressionString.Substring(this.StartIndex, this.Length);

        public override string ToString() => this.Token.ToString();
    }
}
#endif