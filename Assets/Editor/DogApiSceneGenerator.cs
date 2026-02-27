#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public static class DogApiSceneGenerator
{
    private static readonly Color BgColor      = new Color(0.07f, 0.08f, 0.12f);
    private static readonly Color PanelColor   = new Color(0.12f, 0.14f, 0.20f);
    private static readonly Color HeaderColor  = new Color(0.15f, 0.17f, 0.25f);
    private static readonly Color EntryBg      = new Color(0.16f, 0.19f, 0.28f);
    private static readonly Color AccentOrange = new Color(0.97f, 0.55f, 0.12f);
    private static readonly Color AccentBlue   = new Color(0.25f, 0.60f, 1.00f);
    private static readonly Color DisabledColor= new Color(0.35f, 0.35f, 0.40f);
    private static readonly Color TextPrimary  = new Color(0.93f, 0.93f, 0.93f);
    private static readonly Color TextSecondary= new Color(0.65f, 0.68f, 0.78f);
    private static readonly Color TextStats    = new Color(0.45f, 0.80f, 0.60f);

    [MenuItem("Tools/Dog API/Generate Dog Browser Scene")]
    public static void Generate()
    {
        var camGO = new GameObject("Main Camera");
        var cam = camGO.AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = BgColor;
        cam.orthographic = false;
        camGO.tag = "MainCamera";
        camGO.transform.position = new Vector3(0, 0, -10);

        var esGO = new GameObject("EventSystem");
        esGO.AddComponent<EventSystem>();
        esGO.AddComponent<InputSystemUIInputModule>();

        var canvasGO = new GameObject("DogAPICanvas");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode  = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor  = 1f;
        canvasGO.AddComponent<GraphicRaycaster>();

        var outerPanel = CreatePanel(canvasGO.transform, "OuterPanel", PanelColor);
        SetStretch(outerPanel, 0, 0, 0, 0);

        var header = CreatePanel(outerPanel.transform, "Header", HeaderColor);
        SetAnchored(header, new Vector2(0, 1), new Vector2(1, 1), new Vector2(0, -80), new Vector2(0, 0));
        var headerTitle = CreateTMP(header.transform, "HeaderTitle", "Dog Breed Browser", 36, FontStyles.Bold, TextPrimary);
        SetStretch(headerTitle, 20, 0, 20, 0);
        var headerSub = CreateTMP(header.transform, "HeaderSub", "DogAPI.dog  -  283 breeds  -  10 per page", 16, FontStyles.Normal, TextSecondary);
        SetAnchored(headerSub, new Vector2(0, 0), new Vector2(1, 0), new Vector2(20, 2), new Vector2(-20, 22));
        headerTitle.alignment = TextAlignmentOptions.MidlineLeft;
        headerSub.alignment   = TextAlignmentOptions.MidlineLeft;

        var statusTMP = CreateTMP(header.transform, "StatusText", "", 16, FontStyles.Italic, AccentOrange);
        SetAnchored(statusTMP, new Vector2(0.5f, 0), new Vector2(1, 1), new Vector2(0, 0), new Vector2(-20, 0));
        statusTMP.alignment = TextAlignmentOptions.MidlineRight;

        var scrollGO = new GameObject("BreedScrollView");
        scrollGO.transform.SetParent(outerPanel.transform, false);
        var scrollRect  = scrollGO.AddComponent<ScrollRect>();
        var scrollImage = scrollGO.AddComponent<Image>();
        scrollImage.color = new Color(0, 0, 0, 0);
        var scrollRt = scrollGO.GetComponent<RectTransform>();
        scrollRt.anchorMin = new Vector2(0, 0); scrollRt.anchorMax = new Vector2(1, 1);
        scrollRt.offsetMin = new Vector2(0, 100); scrollRt.offsetMax = new Vector2(0, -80);

        var viewportGO = CreatePanel(scrollGO.transform, "Viewport", new Color(0, 0, 0, 0));
        SetStretch(viewportGO, 0, 0, 0, 0);
        viewportGO.AddComponent<RectMask2D>();

        var contentGO = new GameObject("Content");
        contentGO.transform.SetParent(viewportGO.transform, false);
        var contentRt = contentGO.AddComponent<RectTransform>();
        contentRt.anchorMin = new Vector2(0, 1); contentRt.anchorMax = new Vector2(1, 1);
        contentRt.pivot     = new Vector2(0.5f, 1);
        contentRt.sizeDelta = new Vector2(0, 0);
        var vlg = contentGO.AddComponent<VerticalLayoutGroup>();
        vlg.spacing               = 6;
        vlg.padding               = new RectOffset(12, 12, 8, 8);
        vlg.childAlignment        = TextAnchor.UpperCenter;
        vlg.childControlWidth     = true;
        vlg.childControlHeight    = false;
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        var csf = contentGO.AddComponent<ContentSizeFitter>();
        csf.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

        scrollRect.content  = contentRt;
        scrollRect.viewport = viewportGO.GetComponent<RectTransform>();
        scrollRect.horizontal = false;
        scrollRect.vertical   = true;
        scrollRect.scrollSensitivity = 30f;

        var paginationBar = CreatePanel(outerPanel.transform, "PaginationBar", HeaderColor);
        SetAnchored(paginationBar, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 0), new Vector2(0, 100));

        var pageInfoTMP = CreateTMP(paginationBar.transform, "PageInfoText", "Page - / -", 22, FontStyles.Bold, TextPrimary);
        SetAnchored(pageInfoTMP, new Vector2(0.5f, 0), new Vector2(0.5f, 1), new Vector2(-300, 0), new Vector2(300, 0));
        pageInfoTMP.alignment = TextAlignmentOptions.Center;

        var firstBtn = CreateButton(paginationBar.transform, "FirstPageBtn", "First",    AccentBlue);
        var prevBtn  = CreateButton(paginationBar.transform, "PrevPageBtn",  "Previous", AccentBlue);
        var nextBtn  = CreateButton(paginationBar.transform, "NextPageBtn",  "Next",     AccentOrange);
        var lastBtn  = CreateButton(paginationBar.transform, "LastPageBtn",  "Last",     AccentOrange);

        SetAnchored(firstBtn, new Vector2(0, 0), new Vector2(0, 1), new Vector2(10,  10), new Vector2(140, -10));
        SetAnchored(prevBtn,  new Vector2(0, 0), new Vector2(0, 1), new Vector2(150, 10), new Vector2(290, -10));
        SetAnchored(lastBtn,  new Vector2(1, 0), new Vector2(1, 1), new Vector2(-140, 10), new Vector2(-10, -10));
        SetAnchored(nextBtn,  new Vector2(1, 0), new Vector2(1, 1), new Vector2(-295, 10), new Vector2(-150, -10));

        var entryPrefabGO = BuildBreedEntryPrefab();

        string prefabDir = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(prefabDir))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        string prefabPath = $"{prefabDir}/DogBreedEntry.prefab";
        GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(entryPrefabGO, prefabPath);
        GameObject.DestroyImmediate(entryPrefabGO);

        var managerGO = new GameObject("DogApiManager");
        var manager   = managerGO.AddComponent<DogApiManager>();
        manager.breedListContent  = contentGO.transform;
        manager.breedEntryPrefab  = savedPrefab;
        manager.prevButton        = prevBtn.GetComponent<Button>();
        manager.nextButton        = nextBtn.GetComponent<Button>();
        manager.firstPageButton   = firstBtn.GetComponent<Button>();
        manager.lastPageButton    = lastBtn.GetComponent<Button>();
        manager.pageInfoText      = pageInfoTMP.GetComponent<TMP_Text>();
        manager.statusText        = statusTMP.GetComponent<TMP_Text>();

        Undo.RegisterCreatedObjectUndo(camGO,     "Generate Dog API Scene");
        Undo.RegisterCreatedObjectUndo(esGO,      "Generate Dog API Scene");
        Undo.RegisterCreatedObjectUndo(canvasGO,  "Generate Dog API Scene");
        Undo.RegisterCreatedObjectUndo(managerGO, "Generate Dog API Scene");

        Selection.activeGameObject = canvasGO;
        Debug.Log("Dog API Browser scene generated!");
        EditorUtility.DisplayDialog("Done!", "Scene generated!\n\nPress Play to run the browser.", "OK");
    }

    private static GameObject BuildBreedEntryPrefab()
    {
        var root = new GameObject("DogBreedEntry");
        var rootImg = root.AddComponent<Image>();
        rootImg.color = EntryBg;
        var rootEntry = root.AddComponent<DogBreedEntry>();

        var rt = root.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(0, 130);

        var accentBar = CreatePanel(root.transform, "AccentBar", AccentOrange);
        SetAnchored(accentBar, new Vector2(0, 0), new Vector2(0, 1), new Vector2(0, 0), new Vector2(5, 0));

        var nameTMP = CreateTMP(root.transform, "NameText", "Breed Name", 22, FontStyles.Bold, TextPrimary);
        SetAnchored(nameTMP, new Vector2(0, 1), new Vector2(0.65f, 1), new Vector2(16, -8), new Vector2(-8, -44));
        nameTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var statsTMP = CreateTMP(root.transform, "StatsText", "Life: - | Weight M: - | Weight F: - | Hypo: -", 14, FontStyles.Normal, TextStats);
        SetAnchored(statsTMP, new Vector2(0, 1), new Vector2(0.65f, 1), new Vector2(16, -50), new Vector2(-8, -90));
        statsTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var descTMP = CreateTMP(root.transform, "DescriptionText", "...", 13, FontStyles.Normal, TextSecondary);
        SetAnchored(descTMP, new Vector2(0.65f, 0), new Vector2(1, 1), new Vector2(10, 8), new Vector2(-12, -10));
        descTMP.alignment = TextAlignmentOptions.MidlineLeft;
        descTMP.overflowMode = TextOverflowModes.Ellipsis;

        rootEntry.nameText        = nameTMP.GetComponent<TMP_Text>();
        rootEntry.statsText       = statsTMP.GetComponent<TMP_Text>();
        rootEntry.descriptionText = descTMP.GetComponent<TMP_Text>();

        return root;
    }

    private static GameObject CreatePanel(Transform parent, string name, Color color)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        var img = go.AddComponent<Image>();
        img.color = color;
        go.AddComponent<RectTransform>();
        return go;
    }

    private static TMP_Text CreateTMP(Transform parent, string name, string text, float size, FontStyles style, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text            = text;
        tmp.fontSize        = size;
        tmp.fontStyle       = style;
        tmp.color           = color;
        tmp.textWrappingMode = TextWrappingModes.Normal;
        return tmp;
    }

    private static GameObject CreateButton(Transform parent, string name, string label, Color bgColor)
    {
        var go  = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        var img = go.AddComponent<Image>();
        img.color = bgColor;
        var btn = go.AddComponent<Button>();

        var colors = btn.colors;
        colors.normalColor      = bgColor;
        colors.highlightedColor = new Color(bgColor.r + 0.1f, bgColor.g + 0.1f, bgColor.b + 0.1f);
        colors.pressedColor     = new Color(bgColor.r - 0.15f, bgColor.g - 0.15f, bgColor.b - 0.15f);
        colors.disabledColor    = DisabledColor;
        btn.colors = colors;

        var txtGO = new GameObject("Text");
        txtGO.transform.SetParent(go.transform, false);
        var rt = txtGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
        var tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text      = label;
        tmp.fontSize  = 18;
        tmp.fontStyle = FontStyles.Bold;
        tmp.color     = Color.white;
        tmp.alignment = TextAlignmentOptions.Center;

        return go;
    }

    private static void SetStretch(GameObject go, float left, float bottom, float right, float top)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = new Vector2(left,   bottom);
        rt.offsetMax = new Vector2(-right, -top);
    }

    private static void SetAnchored(GameObject go, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = anchorMin; rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin; rt.offsetMax = offsetMax;
    }

    private static void SetStretch(Component c, float left, float bottom, float right, float top)
        => SetStretch(c.gameObject, left, bottom, right, top);

    private static void SetAnchored(Component c, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        => SetAnchored(c.gameObject, anchorMin, anchorMax, offsetMin, offsetMax);
}
#endif
