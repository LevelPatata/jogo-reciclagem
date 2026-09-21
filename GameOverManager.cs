using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Gerencia a tela de Game Over.
/// Pode ser usada de duas formas:
///   1. Cena separada: anexe a um GameObject com UIDocument na cena "GameOver".
///   2. Overlay na cena do jogo: use um UIDocument com Display.None e chame Show().
/// Elementos esperados no UXML: "RestartButton", "MenuButton" e (opcional) "ScoreLabel".
/// </summary>
public class GameOverManager : MonoBehaviour
{
    [SerializeField] private string gameSceneName  = "Game";
    [SerializeField] private string menuSceneName  = "MainMenu";

    // Preencha se este script viver na mesma cena do jogo (overlay)
    [SerializeField] private bool isOverlay = false;

    private VisualElement _root;

    private void OnEnable()
    {
        _root = GetComponent<UIDocument>().rootVisualElement;

        var restartButton = _root.Q<Button>("RestartButton");
        var menuButton    = _root.Q<Button>("MenuButton");

        restartButton?.RegisterCallback<ClickEvent>(_ => RestartGame());
        menuButton?.RegisterCallback<ClickEvent>(_ => GoToMenu());

        // Se for overlay, começa escondido
        if (isOverlay)
            _root.style.display = DisplayStyle.None;
    }

    /// <summary>
    /// Exibe o painel de Game Over e opcionalmente mostra a pontuação.
    /// Chame este método a partir do GameManager quando o jogador perder.
    /// </summary>
    public void Show(int score = -1)
    {
        if (isOverlay)
            _root.style.display = DisplayStyle.Flex;

        if (score >= 0)
        {
            var scoreLabel = _root.Q<Label>("ScoreLabel");
            if (scoreLabel != null)
                scoreLabel.text = $"Pontuação: {score}";
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // Garante que o jogo não fique pausado
        SceneManager.LoadScene(gameSceneName);
    }

    private void GoToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }
}
