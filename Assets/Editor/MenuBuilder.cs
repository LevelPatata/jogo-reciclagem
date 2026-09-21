using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Monta o Menu Principal automaticamente (Canvas, título, campo de nome,
/// botões e painel de ranking), já ligado ao MainMenuManager.
/// Use o menu: Tools > RECICLA > Construir Menu Principal.
/// Pode rodar de novo à vontade — ele refaz o canvas do zero.
/// </summary>
public static class MenuBuilder
{
    // ---- paleta (tema eco) ----
    static readonly Color VERDE_ESCURO = new Color(0.10f, 0.35f, 0.19f);
    static readonly Color VERDE_BOTAO = new Color(0.29f, 0.66f, 0.32f);
    static readonly Color VERDE_BOTAO_HOVER = new Color(0.34f, 0.74f, 0.37f);
    static readonly Color CLARO = new Color(0.93f, 0.97f, 0.90f);
    static readonly Color PAINEL = new Color(1f, 1f, 1f, 0.85f);
    static readonly Color BRANCO = Color.white;

    const string CANVAS_NOME = "MenuCanvas (auto)";
    const string BG_PATH = "Assets/imagens/background/fundo_menu.png";

    [MenuItem("Tools/RECICLA/Construir Menu Principal")]
    public static void ConstruirMenu()
    {
        // remove um canvas gerado anteriormente (idempotente)
        var antigo = GameObject.Find(CANVAS_NOME);
        if (antigo != null)
            Object.DestroyImmediate(antigo);

        // ---- Canvas ----
        var canvasGO = new GameObject(CANVAS_NOME, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        // ---- EventSystem (se não houver) ----
        if (Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include) == null)
        {
            var es = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        var root = canvasGO.transform;

        // ---- Fundo ----
        var bg = NewUI("Fundo", root);
        Stretch(RT(bg));
        var bgImg = bg.AddComponent<Image>();
        var bgSprite = LoadSprite(BG_PATH);
        if (bgSprite != null) { bgImg.sprite = bgSprite; bgImg.type = Image.Type.Simple; bgImg.preserveAspect = false; }
        else bgImg.color = new Color(0.55f, 0.78f, 0.58f);

        // ---- Título ----
        var titulo = NewUI("Titulo", root);
        Place(RT(titulo), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -55), new Vector2(1400, 180));
        var tTit = AddText(titulo, "RECICLA", 128, VERDE_ESCURO, TextAlignmentOptions.Center);
        tTit.fontStyle = FontStyles.Bold;

        var tag = NewUI("Tagline", root);
        Place(RT(tag), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -215), new Vector2(1400, 60));
        AddText(tag, "Separe o lixo o mais rápido que puder", 36, VERDE_ESCURO, TextAlignmentOptions.Center);

        // ---- Coluna esquerda: nome + botões ----
        float colX = -430f;

        var lblNome = NewUI("LabelNome", root);
        Place(RT(lblNome), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(colX, 60), new Vector2(560, 54));
        AddText(lblNome, "Seu nome", 38, VERDE_ESCURO, TextAlignmentOptions.Center);

        var campoNome = CreateInputField(root, "CampoNome", "Digite aqui...",
            new Vector2(colX, -15), new Vector2(560, 86));

        var btnJogar = CreateButton(root, "BotaoJogar", "JOGAR",
            new Vector2(colX, -150), new Vector2(560, 120), VERDE_BOTAO, VERDE_BOTAO_HOVER, BRANCO, 62);

        var btnSair = CreateButton(root, "BotaoSair", "Sair",
            new Vector2(colX, -295), new Vector2(560, 80), CLARO, BRANCO, VERDE_ESCURO, 40);

        // ---- Coluna direita: painel de ranking ----
        var painel = NewUI("PainelRanking", root);
        Place(RT(painel), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(430, -70), new Vector2(680, 640));
        var painelImg = painel.AddComponent<Image>();
        painelImg.sprite = UISprite();
        painelImg.type = Image.Type.Sliced;
        painelImg.color = PAINEL;

        var painelTitulo = NewUI("TituloRanking", painel.transform);
        Place(RT(painelTitulo), new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -26), new Vector2(620, 76));
        var tRank = AddText(painelTitulo, "Ranking", 50, VERDE_ESCURO, TextAlignmentOptions.Center);
        tRank.fontStyle = FontStyles.Bold;

        var textoRanking = NewUI("TextoRanking", painel.transform);
        var trt = RT(textoRanking);
        trt.anchorMin = new Vector2(0, 0); trt.anchorMax = new Vector2(1, 1);
        trt.offsetMin = new Vector2(36, 92); trt.offsetMax = new Vector2(-36, -116);
        AddText(textoRanking, "1. Ana - 45\n2. Bruno - 32\n3. Você - 28\n...", 34, VERDE_ESCURO, TextAlignmentOptions.TopLeft);

        var btnLimpar = CreateButton(painel.transform, "BotaoLimparRanking", "Limpar ranking",
            new Vector2(0, 0), new Vector2(360, 66), CLARO, BRANCO, VERDE_ESCURO, 28);
        var brt = RT(btnLimpar.gameObject);
        brt.anchorMin = brt.anchorMax = new Vector2(0.5f, 0f);
        brt.pivot = new Vector2(0.5f, 0f);
        brt.anchoredPosition = new Vector2(0, 28);

        // ---- Liga tudo no MainMenuManager ----
        var mgr = Object.FindFirstObjectByType<MainMenuManager>(FindObjectsInactive.Include);
        if (mgr == null)
        {
            var mgo = new GameObject("MenuManager");
            mgr = mgo.AddComponent<MainMenuManager>();
        }
        mgr.botaoJogar = btnJogar;
        mgr.botaoSair = btnSair;
        mgr.botaoLimparRanking = btnLimpar;
        mgr.campoNome = campoNome;
        mgr.textoRanking = textoRanking.GetComponent<TextMeshProUGUI>();
        EditorUtility.SetDirty(mgr);

        Selection.activeGameObject = canvasGO;
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("<color=green>Menu Principal construído!</color> Se aparecer um Canvas antigo duplicado, pode apagá-lo.");
    }

    // ======================= helpers =======================

    static RectTransform RT(GameObject go) { return go.GetComponent<RectTransform>(); }

    static GameObject NewUI(string nome, Transform parent)
    {
        var go = new GameObject(nome, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
    }

    static void Place(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static TextMeshProUGUI AddText(GameObject go, string txt, float size, Color cor, TextAlignmentOptions align)
    {
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = txt;
        t.fontSize = size;
        t.color = cor;
        t.alignment = align;
        return t;
    }

    static Button CreateButton(Transform parent, string nome, string label, Vector2 pos, Vector2 size,
        Color bg, Color hover, Color corTexto, float fonte)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);
        var img = go.AddComponent<Image>();
        img.sprite = UISprite();
        img.type = Image.Type.Sliced;
        img.color = bg;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var cores = btn.colors;
        cores.normalColor = Color.white;
        cores.highlightedColor = Color.Lerp(Color.white, hover, 0.25f);
        cores.pressedColor = new Color(0.88f, 0.88f, 0.88f, 1f);
        btn.colors = cores;

        var txtGO = NewUI("Texto", go.transform);
        Stretch(RT(txtGO));
        var t = AddText(txtGO, label, fonte, corTexto, TextAlignmentOptions.Center);
        t.fontStyle = FontStyles.Bold;
        return btn;
    }

    static TMP_InputField CreateInputField(Transform parent, string nome, string placeholder, Vector2 pos, Vector2 size)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, size);
        var img = go.AddComponent<Image>();
        img.sprite = UISprite();
        img.type = Image.Type.Sliced;
        img.color = Color.white;

        var input = go.AddComponent<TMP_InputField>();

        var area = NewUI("Text Area", go.transform);
        var art = RT(area);
        art.anchorMin = Vector2.zero; art.anchorMax = Vector2.one;
        art.offsetMin = new Vector2(16, 8); art.offsetMax = new Vector2(-16, -8);
        area.AddComponent<RectMask2D>();

        var ph = NewUI("Placeholder", area.transform);
        Stretch(RT(ph));
        var phTxt = AddText(ph, placeholder, 32, new Color(0.45f, 0.45f, 0.45f, 0.7f), TextAlignmentOptions.Left);
        phTxt.fontStyle = FontStyles.Italic;

        var textGO = NewUI("Text", area.transform);
        Stretch(RT(textGO));
        var textComp = AddText(textGO, "", 32, VERDE_ESCURO, TextAlignmentOptions.Left);
        textComp.richText = false;

        input.textViewport = art;
        input.textComponent = textComp;
        input.placeholder = phTxt;
        input.targetGraphic = img;
        input.text = "";
        input.characterLimit = 12;
        return input;
    }

    static Sprite UISprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    static Sprite LoadSprite(string path)
    {
        var imp = AssetImporter.GetAtPath(path) as TextureImporter;
        if (imp != null && imp.textureType != TextureImporterType.Sprite)
        {
            imp.textureType = TextureImporterType.Sprite;
            imp.SaveAndReimport();
        }
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }
}
