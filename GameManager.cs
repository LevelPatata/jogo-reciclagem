using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Singleton central que controla o estado do jogo.
/// Anexe a um GameObject chamado "GameManager" na cena do jogo.
///
/// Para acionar o Game Over em qualquer script, use:
///     GameManager.Instance.TriggerGameOver();
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Configurações")]
    // Nome da cena de Game Over (se usar cena separada)
    [SerializeField] private string gameOverSceneName = "GameOver";

    // Se preferir overlay na mesma cena, arraste o GameOverManager aqui
    [SerializeField] private GameOverManager gameOverOverlay;

    [Header("Estado")]
    [SerializeField] private int score = 0;

    public int Score => score;

    private bool _gameEnded = false;

    private void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    /// <summary>
    /// Adiciona pontos à pontuação atual.
    /// </summary>
    public void AddScore(int points)
    {
        if (_gameEnded) return;
        score += points;
        Debug.Log($"Pontuação: {score}");
    }

    /// <summary>
    /// Chame este método quando o jogador perder.
    /// </summary>
    public void TriggerGameOver()
    {
        if (_gameEnded) return;
        _gameEnded = true;

        Time.timeScale = 0f; // Pausa o jogo

        if (gameOverOverlay != null)
        {
            // Modo overlay: exibe o painel na mesma cena
            gameOverOverlay.Show(score);
        }
        else
        {
            // Modo cena separada: carrega a cena de Game Over
            // Retoma o tempo antes de trocar de cena
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameOverSceneName);
        }
    }
}
