using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

/// <summary>
/// Gerencia a tela de início do jogo.
/// Anexe este script a um GameObject na cena do Menu Principal.
/// Requer um componente UIDocument com os elementos: "PlayButton" e "QuitButton".
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // Altere para o nome exato da sua cena de jogo
    [SerializeField] private string gameSceneName = "Game";

    private void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        var playButton = root.Q<Button>("PlayButton");
        var quitButton = root.Q<Button>("QuitButton");

        playButton?.RegisterCallback<ClickEvent>(_ => StartGame());
        quitButton?.RegisterCallback<ClickEvent>(_ => QuitGame());
    }

    private void StartGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
