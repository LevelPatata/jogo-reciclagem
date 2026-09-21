using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public TextMeshProUGUI tempoTexto;
    public TextMeshProUGUI scoreTexto;
    public TextMeshProUGUI levaTexto;

    [Header("Barra de tempo")]
    public Image tempoBarra;
    public float tempoBarraMax = 30f;

    [Header("Cores")]
    public Color corNormal = Color.white;
    public Color corErro = Color.red;
    public Color corAcerto = Color.green;

    [Header("Duração efeitos")]
    public float duracaoErro = 1f;
    public float duracaoAcerto = 0.5f;

    [Header("Game Over")]
    public GameObject painelGameOver;
    public TextMeshProUGUI gameOverScoreTexto;
    public TextMeshProUGUI gameOverRankingTexto;
    public Button botaoReiniciar;
    public Button botaoMenu;

    private Coroutine efeitoTempo;
    private Coroutine efeitoScore;

    void Awake()
    {
        instance = this;

        if (painelGameOver != null)
            painelGameOver.SetActive(false);

        if (botaoReiniciar != null)
            botaoReiniciar.onClick.AddListener(() => GameManager.instance.ReiniciarJogo());

        if (botaoMenu != null)
            botaoMenu.onClick.AddListener(() =>
            {
                Time.timeScale = 1f;
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
            });
    }

    void Update()
    {
        AtualizarUI();
    }

    void AtualizarUI()
    {
        if (GameManager.instance == null)
            return;

        if (tempoTexto != null)
            tempoTexto.text = Mathf.CeilToInt(GameManager.instance.tempo).ToString();
        if (scoreTexto != null)
            scoreTexto.text = GameManager.instance.score.ToString();
        if (levaTexto != null)
            levaTexto.text = "Leva " + GameManager.instance.levaAtual;

        if (tempoBarra != null)
            tempoBarra.fillAmount = Mathf.Clamp01(GameManager.instance.tempo / tempoBarraMax);
    }

    public void MostrarErroTempo()
    {
        if (tempoTexto == null)
            return;

        if (efeitoTempo != null)
            StopCoroutine(efeitoTempo);

        efeitoTempo = StartCoroutine(EfeitoTempoErro());
    }

    IEnumerator EfeitoTempoErro()
    {
        tempoTexto.color = corErro;
        yield return new WaitForSeconds(duracaoErro);
        tempoTexto.color = corNormal;
    }

    public void MostrarAcertoScore()
    {
        if (scoreTexto == null)
            return;

        if (efeitoScore != null)
            StopCoroutine(efeitoScore);

        efeitoScore = StartCoroutine(EfeitoScoreAcerto());
    }

    IEnumerator EfeitoScoreAcerto()
    {
        scoreTexto.color = corAcerto;
        yield return new WaitForSeconds(duracaoAcerto);
        scoreTexto.color = corNormal;
    }

    public void MostrarGameOver(int scoreFinal)
    {
        if (painelGameOver != null)
            painelGameOver.SetActive(true);

        if (gameOverScoreTexto != null)
            gameOverScoreTexto.text = "Pontuação Final: " + scoreFinal;

        if (gameOverRankingTexto != null)
            gameOverRankingTexto.text = Ranking.TextoFormatado(5);
    }
}
