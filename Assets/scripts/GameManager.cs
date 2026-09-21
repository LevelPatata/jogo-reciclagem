using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Tempo e Pontuação")]
    public float tempo = 30f;
    public int score = 0;

    [Header("Balanceamento")]
    public float tempoGanhoAcerto = 0.5f;
    public float tempoPerdaErro = 5f;
    public float tempoBonusLeva = 2f;

    [Header("Velocidade do Tempo")]
    public float velocidadeTempoBase = 1f;
    public float aumentoPorLeva = 0.1f;

    [Header("Spawn")]
    public GameObject[] prefabsLixo;
    public Transform[] pontosSpawn;

    [Header("Levas")]
    public int levaAtual = 1;
    public int lixosPorLeva = 4;

    public Lixo lixoSelecionado;

    private int lixosNaCena = 0;
    private int lixosNaLeva = 0;
    private int lixosParados = 0;
    private bool levaPronta = false;
    private bool jogoAcabou = false;

    private readonly List<Lixeira> lixeirasTristes = new List<Lixeira>();

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        IniciarLeva();
    }

    void Update()
    {
        if (jogoAcabou || !levaPronta)
            return;

        float velocidadeTempo = velocidadeTempoBase + (levaAtual * aumentoPorLeva);
        tempo -= Time.deltaTime * velocidadeTempo;

        if (tempo <= 0)
        {
            tempo = 0;
            GameOver();
        }
    }

    void GameOver()
    {
        jogoAcabou = true;

        Ranking.Adicionar(Ranking.ObterNome(), score);

        if (AudioManager.instance != null)
            AudioManager.instance.TocarGameOver();

        Time.timeScale = 0f;

        if (UIManager.instance != null)
            UIManager.instance.MostrarGameOver(score);
    }

    public void ReiniciarJogo()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    void IniciarLeva()
    {
        if (jogoAcabou)
            return;

        lixosNaCena = 0;
        lixosParados = 0;
        levaPronta = false;

        Esteira esteira = Esteira.instance;

        int quantidade = Mathf.Min(lixosPorLeva, pontosSpawn.Length);
        lixosNaLeva = quantidade;

        List<Vector3> destinos = new List<Vector3>();
        for (int i = 0; i < quantidade; i++)
            destinos.Add(pontosSpawn[i].position);
        destinos.Sort((a, b) => a.x.CompareTo(b.x));

        for (int k = 0; k < quantidade; k++)
        {
            int indexPrefab = Random.Range(0, prefabsLixo.Length);
            Vector3 destino = destinos[k];

            Vector3 posInicial = esteira != null
                ? esteira.PosicaoEntrada(quantidade - 1 - k, destino.y)
                : destino;

            GameObject go = Instantiate(prefabsLixo[indexPrefab], posInicial, Quaternion.identity);

            Lixo lixo = go.GetComponent<Lixo>();
            if (lixo != null)
                lixo.DefinirDestino(destino);

            lixosNaCena++;
        }
    }

    public void LixoChegou()
    {
        lixosParados++;
        if (lixosParados >= lixosNaLeva)
            levaPronta = true;
    }

    public void SelecionarLixo(Lixo lixo)
    {
        if (jogoAcabou || !levaPronta)
            return;

        if (lixoSelecionado != null)
            lixoSelecionado.ResetarCor();

        lixoSelecionado = lixo;
        lixoSelecionado.Destacar();

        if (AudioManager.instance != null)
            AudioManager.instance.TocarClique();
    }

    public void VerificarResposta(Lixeira lixeira)
    {
        if (jogoAcabou || lixoSelecionado == null)
            return;

        if (lixoSelecionado.tipo == lixeira.tipo)
        {
            score++;
            tempo += tempoGanhoAcerto;

            VoltarLixeirasFeliz();

            if (AudioManager.instance != null)
                AudioManager.instance.TocarAcerto();

            if (UIManager.instance != null)
                UIManager.instance.MostrarAcertoScore();
        }
        else
        {
            tempo -= tempoPerdaErro;

            lixeira.MostrarTriste();
            if (!lixeirasTristes.Contains(lixeira))
                lixeirasTristes.Add(lixeira);

            if (AudioManager.instance != null)
                AudioManager.instance.TocarErro();

            if (UIManager.instance != null)
                UIManager.instance.MostrarErroTempo();
        }

        Destroy(lixoSelecionado.gameObject);
        lixoSelecionado = null;
        lixosNaCena--;

        if (lixosNaCena <= 0)
            ProximaLeva();
    }

    void VoltarLixeirasFeliz()
    {
        foreach (Lixeira l in lixeirasTristes)
            if (l != null)
                l.VoltarFeliz();

        lixeirasTristes.Clear();
    }

    void ProximaLeva()
    {
        if (jogoAcabou)
            return;

        levaAtual++;
        lixosPorLeva++;
        tempo += tempoBonusLeva;

        IniciarLeva();
    }
}
