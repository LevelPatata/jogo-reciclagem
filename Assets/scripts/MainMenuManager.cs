using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    [Header("Botões")]
    public Button botaoJogar;
    public Button botaoSair;

    [Header("Nome do jogador")]
    public TMP_InputField campoNome;

    [Header("Ranking")]
    public TextMeshProUGUI textoRanking;
    public Button botaoLimparRanking;

    [SerializeField] private string cenaJogo = "Game";

    void Awake()
    {
        if (botaoJogar != null)
            botaoJogar.onClick.AddListener(Jogar);
        if (botaoSair != null)
            botaoSair.onClick.AddListener(Sair);
        if (botaoLimparRanking != null)
            botaoLimparRanking.onClick.AddListener(LimparRanking);
    }

    void Start()
    {
        if (campoNome != null)
            campoNome.text = Ranking.ObterNome();

        AtualizarRanking();
    }

    void Jogar()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.TocarClique();

        if (campoNome != null)
            Ranking.SalvarNome(campoNome.text);

        SceneManager.LoadScene(cenaJogo);
    }

    void LimparRanking()
    {
        if (AudioManager.instance != null)
            AudioManager.instance.TocarClique();

        Ranking.Limpar();
        AtualizarRanking();
    }

    void AtualizarRanking()
    {
        if (textoRanking != null)
            textoRanking.text = Ranking.TextoFormatado();
    }

    void Sair()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
