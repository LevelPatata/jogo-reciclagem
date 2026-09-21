using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;

/// <summary>
/// Monta a interface do JOGO: HUD com ícones (Tempo + barra, Leva, Score),
/// a barra estilo Subway (quem está à sua frente) e a tela de Game Over.
/// Liga tudo no UIManager e no RankingCorrida.
/// Use: Tools > RECICLA > Construir Interface do Jogo. Rode na cena "Game".
/// </summary>
public static class GameUIBuilder
{
    static readonly Color VERDE_ESCURO = new Color(0.10f, 0.35f, 0.19f);
    static readonly Color VERDE_BOTAO = new Color(0.29f, 0.66f, 0.32f);
    static readonly Color CLARO = new Color(0.90f, 0.94f, 0.88f);
    static readonly Color BRANCO = Color.white;
    static readonly Color HUD_BG = new Color(0.07f, 0.17f, 0.13f, 0.86f);
    static readonly Color PRETO = new Color(0f, 0f, 0f, 0.88f);
    static readonly Color TRILHO = new Color(0f, 0f, 0f, 0.40f);
    static readonly Color AMBAR = new Color(1f, 0.84f, 0.25f);
    static readonly Color DIM = new Color(0f, 0f, 0f, 0.62f);

    const string CANVAS_NOME = "GameCanvas (auto)";
    const string IC_RELOGIO = "Assets/imagens/ui/icone_relogio.png";
    const string IC_ESTRELA = "Assets/imagens/ui/icone_estrela.png";
    const string IC_TROFEU = "Assets/imagens/ui/icone_trofeu.png";
    const string IC_CAMADAS = "Assets/imagens/ui/icone_camadas.png";

    [MenuItem("Tools/RECICLA/Construir Interface do Jogo")]
    public static void ConstruirInterface()
    {
        var antigo = GameObject.Find(CANVAS_NOME);
        if (antigo != null)
            Object.DestroyImmediate(antigo);

        var canvasGO = new GameObject(CANVAS_NOME, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        var canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 100;
        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        scaler.matchWidthOrHeight = 0.5f;

        if (Object.FindFirstObjectByType<EventSystem>(FindObjectsInactive.Include) == null)
            new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));

        var root = canvasGO.transform;

        // ========================= HUD (retangular, na faixa ACIMA da esteira) =========================
        // --- Tempo (canto esquerdo) ---
        var pTempo = PanelRet(root, "HUD_Tempo", new Vector2(0, 1), new Vector2(0, 1), new Vector2(24, -14), new Vector2(270, 88), HUD_BG);
        Icon(pTempo.transform, "Icone", IC_RELOGIO, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(16, 0), new Vector2(52, 52), BRANCO);
        var tempoTxt = Label(pTempo.transform, "Tempo", "30", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(82, 0), new Vector2(176, 66), 52, BRANCO, TextAlignmentOptions.Left);
        tempoTxt.fontStyle = FontStyles.Bold;

        // --- Barra de tempo (logo abaixo do Tempo, ainda acima da esteira) ---
        var trilho = PanelRet(root, "HUD_TempoTrilho", new Vector2(0, 1), new Vector2(0, 1), new Vector2(24, -106), new Vector2(270, 16), TRILHO);
        var fillGO = NewUI("Preenchimento", trilho.transform);
        Stretch(RT(fillGO));
        var fill = fillGO.AddComponent<Image>();
        fill.type = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillOrigin = (int)Image.OriginHorizontal.Left;
        fill.fillAmount = 1f;
        fill.color = VERDE_BOTAO;

        // --- Leva (CENTRO, destaque) ---
        var pLeva = PanelRet(root, "HUD_Leva", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -14), new Vector2(320, 88), HUD_BG);
        Icon(pLeva.transform, "Icone", IC_CAMADAS, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(26, 0), new Vector2(50, 50), BRANCO);
        var levaTxt = Label(pLeva.transform, "Leva", "Leva 1", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(22, 0), new Vector2(250, 66), 46, BRANCO, TextAlignmentOptions.Center);
        levaTxt.fontStyle = FontStyles.Bold;

        // --- Score (canto direito) ---
        var pScore = PanelRet(root, "HUD_Score", new Vector2(1, 1), new Vector2(1, 1), new Vector2(-24, -14), new Vector2(270, 88), HUD_BG);
        Icon(pScore.transform, "Icone", IC_ESTRELA, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(16, 0), new Vector2(50, 50), AMBAR);
        var scoreTxt = Label(pScore.transform, "Score", "0", new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(80, 0), new Vector2(176, 66), 52, BRANCO, TextAlignmentOptions.Left);
        scoreTxt.fontStyle = FontStyles.Bold;

        // ==================== Ranking Subway (quadrado PRETO abaixo da esteira) ====================
        var barra = PanelRet(root, "BarraRanking", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 178), new Vector2(720, 80), PRETO);
        Icon(barra.transform, "Icone", IC_TROFEU, new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(26, 0), new Vector2(50, 50), AMBAR);
        var textoAlvo = Label(barra.transform, "TextoAlvo", "Próximo: 3º João (15)", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(30, 0), new Vector2(620, 64), 38, BRANCO, TextAlignmentOptions.Center);

        var aviso = NewUI("AvisoUltrapasse", root);
        Place(RT(aviso), new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 92), new Vector2(900, 70));
        var avisoTxt = AddText(aviso, "Você passou João!", 44, AMBAR, TextAlignmentOptions.Center);
        avisoTxt.fontStyle = FontStyles.Bold;

        var rcGO = NewUI("RankingCorrida", root);
        var rc = rcGO.AddComponent<RankingCorrida>();
        rc.textoAlvo = textoAlvo;
        rc.textoAviso = avisoTxt;
        aviso.SetActive(false);

        // ==================== Game Over ====================
        var painelGO = NewUI("PainelGameOver", root);
        Stretch(RT(painelGO));
        var dim = painelGO.AddComponent<Image>();
        dim.color = DIM;

        var card = Panel(painelGO.transform, "Card", new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(780, 820), BRANCO);

        var titulo = Label(card.transform, "Titulo", "Fim de jogo", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -40), new Vector2(700, 92), 66, VERDE_ESCURO, TextAlignmentOptions.Center);
        titulo.fontStyle = FontStyles.Bold;

        var scoreFinal = Label(card.transform, "ScoreFinal", "Pontuação Final: 0", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -152), new Vector2(700, 64), 42, VERDE_ESCURO, TextAlignmentOptions.Center);

        var rankTitulo = Label(card.transform, "RankingTitulo", "Ranking", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -234), new Vector2(700, 56), 40, VERDE_ESCURO, TextAlignmentOptions.Center);
        rankTitulo.fontStyle = FontStyles.Bold;

        var rankTexto = Label(card.transform, "RankingTexto", "1. Ana - 45\n2. Bruno - 32\n3. Você - 28", new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0, -300), new Vector2(660, 300), 34, VERDE_ESCURO, TextAlignmentOptions.Top);

        var btnReiniciar = Botao(card.transform, "BotaoReiniciar", "Reiniciar", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(-180, 52), new Vector2(300, 96), VERDE_BOTAO, BRANCO, 40);
        var btnMenu = Botao(card.transform, "BotaoMenu", "Menu", new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(180, 52), new Vector2(300, 96), CLARO, VERDE_ESCURO, 40);

        // ==================== Liga no UIManager ====================
        // inclui inativos para não criar um UIManager duplicado
        var ui = Object.FindFirstObjectByType<UIManager>(FindObjectsInactive.Include);
        if (ui == null)
            ui = new GameObject("UIManager").AddComponent<UIManager>();

        ui.tempoTexto = tempoTxt;
        ui.scoreTexto = scoreTxt;
        ui.levaTexto = levaTxt;
        ui.tempoBarra = fill;
        ui.painelGameOver = painelGO;
        ui.gameOverScoreTexto = scoreFinal;
        ui.gameOverRankingTexto = rankTexto;
        ui.botaoReiniciar = btnReiniciar;
        ui.botaoMenu = btnMenu;
        EditorUtility.SetDirty(ui);

        painelGO.SetActive(false);

        Selection.activeGameObject = canvasGO;
        EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());

        Debug.Log("<color=green>Interface do Jogo construída!</color> HUD com ícones + barra de tempo + ranking Subway + Game Over. Apague o Canvas antigo se estiver duplicado.");
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

    // painel com o arredondamento sutil padrão do Unity (usado no Game Over)
    static GameObject Panel(Transform parent, string nome, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, Color cor)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), anchor, pivot, pos, size);
        var img = go.AddComponent<Image>();
        img.sprite = UISprite();
        img.type = Image.Type.Sliced;
        img.color = cor;
        return go;
    }

    static Sprite UISprite()
    {
        return AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
    }

    // painel retangular (cantos retos, sem sprite = sem arredondamento)
    static GameObject PanelRet(Transform parent, string nome, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, Color cor)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), anchor, pivot, pos, size);
        var img = go.AddComponent<Image>();
        img.color = cor;
        return go;
    }

    static Image Icon(Transform parent, string nome, string path, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, Color cor)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), anchor, pivot, pos, size);
        var img = go.AddComponent<Image>();
        img.sprite = LoadSprite(path);
        img.color = cor;
        img.preserveAspect = true;
        img.raycastTarget = false;
        return img;
    }

    static TextMeshProUGUI AddText(GameObject go, string txt, float size, Color cor, TextAlignmentOptions align)
    {
        var t = go.AddComponent<TextMeshProUGUI>();
        t.text = txt; t.fontSize = size; t.color = cor; t.alignment = align;
        t.raycastTarget = false;
        return t;
    }

    static TextMeshProUGUI Label(Transform parent, string nome, string txt, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, float fonte, Color cor, TextAlignmentOptions align)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), anchor, pivot, pos, size);
        return AddText(go, txt, fonte, cor, align);
    }

    static Button Botao(Transform parent, string nome, string label, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size, Color bg, Color corTexto, float fonte)
    {
        var go = NewUI(nome, parent);
        Place(RT(go), anchor, pivot, pos, size);
        var img = go.AddComponent<Image>();
        img.sprite = UISprite();
        img.type = Image.Type.Sliced;
        img.color = bg;

        var btn = go.AddComponent<Button>();
        btn.targetGraphic = img;
        var cores = btn.colors;
        cores.highlightedColor = new Color(0.92f, 0.96f, 0.92f);
        cores.pressedColor = new Color(0.85f, 0.85f, 0.85f);
        btn.colors = cores;

        var txtGO = NewUI("Texto", go.transform);
        Stretch(RT(txtGO));
        var t = AddText(txtGO, label, fonte, corTexto, TextAlignmentOptions.Center);
        t.fontStyle = FontStyles.Bold;
        return btn;
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
