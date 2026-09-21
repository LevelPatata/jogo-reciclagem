using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Música")]
    public AudioClip musica;
    [Range(0f, 1f)] public float volumeMusica = 0.5f;

    [Header("Efeitos")]
    public AudioClip acerto;
    public AudioClip erro;
    public AudioClip clique;
    public AudioClip gameOver;
    [Range(0f, 1f)] public float volumeEfeitos = 1f;

    private AudioSource fonteMusica;
    private AudioSource fonteEfeitos;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        fonteMusica = gameObject.AddComponent<AudioSource>();
        fonteMusica.loop = true;
        fonteMusica.playOnAwake = false;
        fonteMusica.volume = volumeMusica;

        fonteEfeitos = gameObject.AddComponent<AudioSource>();
        fonteEfeitos.playOnAwake = false;

        AudioListener.volume = PlayerPrefs.GetInt("mudo", 0) == 1 ? 0f : 1f;

        if (musica != null)
        {
            fonteMusica.clip = musica;
            fonteMusica.Play();
        }
    }

    public void TocarAcerto() { Tocar(acerto); }
    public void TocarErro() { Tocar(erro); }
    public void TocarClique() { Tocar(clique); }
    public void TocarGameOver() { Tocar(gameOver); }

    void Tocar(AudioClip clip)
    {
        if (clip != null && fonteEfeitos != null)
            fonteEfeitos.PlayOneShot(clip, volumeEfeitos);
    }

    public bool AlternarMudo()
    {
        bool mudo = AudioListener.volume > 0f;
        AudioListener.volume = mudo ? 0f : 1f;
        PlayerPrefs.SetInt("mudo", mudo ? 1 : 0);
        PlayerPrefs.Save();
        return mudo;
    }
}
