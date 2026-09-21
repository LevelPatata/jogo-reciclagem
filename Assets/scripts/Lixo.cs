using UnityEngine;

public enum TipoLixo
{
    Papel,
    Plastico,
    Metal,
    Vidro,
    Organico
}

public class Lixo : MonoBehaviour
{
    public TipoLixo tipo;

    [Header("Visual Seleção")]
    public GameObject contorno;
    public float escalaSelecionado = 1.2f;

    [Header("Tamanho na esteira")]
    public bool normalizarTamanho = true;
    public float tamanhoAlvo = 1.2f;

    private Vector3 escalaOriginal;
    private Vector3 destino;
    private bool temDestino = false;
    private bool chegou = false;

    void Start()
    {
        NormalizarTamanho();

        escalaOriginal = transform.localScale;

        if (contorno != null)
            contorno.SetActive(false);
    }

    void Update()
    {
        if (!temDestino || chegou)
            return;

        Esteira esteira = Esteira.instance;
        if (esteira == null || GameManager.instance == null)
        {
            Chegar();
            return;
        }

        float v = esteira.VelocidadeAtual(GameManager.instance.levaAtual);
        float novoX = transform.position.x + v * Time.deltaTime;

        if (novoX >= destino.x)
            Chegar();
        else
            transform.position = new Vector3(novoX, destino.y, destino.z);
    }

    public void DefinirDestino(Vector3 d)
    {
        destino = d;
        temDestino = true;
        chegou = false;
    }

    void Chegar()
    {
        transform.position = destino;
        chegou = true;

        if (GameManager.instance != null)
            GameManager.instance.LixoChegou();
    }

    void NormalizarTamanho()
    {
        if (!normalizarTamanho)
            return;

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null || sr.sprite == null)
            return;

        Vector2 tam = sr.sprite.bounds.size;
        float maior = Mathf.Max(tam.x, tam.y);
        if (maior > 0.0001f)
        {
            float fator = tamanhoAlvo / maior;
            transform.localScale = new Vector3(fator, fator, 1f);
        }
    }

    void OnMouseDown()
    {
        if (GameManager.instance != null)
            GameManager.instance.SelecionarLixo(this);
    }

    public void Destacar()
    {
        transform.localScale = escalaOriginal * escalaSelecionado;

        if (contorno != null)
            contorno.SetActive(true);
    }

    public void ResetarCor()
    {
        transform.localScale = escalaOriginal;

        if (contorno != null)
            contorno.SetActive(false);
    }
}
