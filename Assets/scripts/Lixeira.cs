using UnityEngine;

public class Lixeira : MonoBehaviour
{
    public TipoLixo tipo;

    [Header("Animação de erro")]
    public Sprite spriteTriste;

    private SpriteRenderer sr;
    private Sprite spriteFeliz;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            spriteFeliz = sr.sprite;
    }

    void OnMouseDown()
    {
        if (GameManager.instance != null)
            GameManager.instance.VerificarResposta(this);
    }

    public void MostrarTriste()
    {
        if (sr != null && spriteTriste != null)
            sr.sprite = spriteTriste;
    }

    public void VoltarFeliz()
    {
        if (sr != null && spriteFeliz != null)
            sr.sprite = spriteFeliz;
    }
}
