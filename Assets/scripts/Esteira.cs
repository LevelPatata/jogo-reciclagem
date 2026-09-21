using UnityEngine;

public class Esteira : MonoBehaviour
{
    public static Esteira instance;

    [Header("Geometria (coordenadas de mundo)")]
    public float xEntrada = -10f;
    public float zLixo = 0f;

    [Header("Velocidade de chegada do lixo")]
    public float velocidade = 4f;
    public float aumentoPorLeva = 0.6f;
    public float espacamento = 2.2f;

    [Header("Visual — rolagem da esteira")]
    public Renderer faixaRolante;
    public float velocidadeTextura = 0.6f;
    public float aumentoTexturaPorLeva = 0.25f;

    private Material matFaixa;
    private float offset;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (faixaRolante != null)
            matFaixa = faixaRolante.material;
    }

    void Update()
    {
        if (matFaixa == null)
            return;

        int leva = GameManager.instance != null ? GameManager.instance.levaAtual : 1;
        float velTex = velocidadeTextura + Mathf.Max(0, leva - 1) * aumentoTexturaPorLeva;

        offset += velTex * Time.deltaTime;
        matFaixa.mainTextureOffset = new Vector2(offset, 0f);
    }

    public float VelocidadeAtual(int leva)
    {
        return velocidade + Mathf.Max(0, leva - 1) * aumentoPorLeva;
    }

    public Vector3 PosicaoEntrada(int index, float y)
    {
        return new Vector3(xEntrada - index * espacamento, y, zLixo);
    }
}
