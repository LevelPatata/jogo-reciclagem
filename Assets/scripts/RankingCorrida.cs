using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class RankingCorrida : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI textoAlvo;
    public TextMeshProUGUI textoAviso;
    public float duracaoAviso = 1.5f;

    private List<Ranking.Entrada> lista;
    private int alvoIndex;
    private Coroutine avisoRotina;

    void Start()
    {
        lista = Ranking.Obter();
        alvoIndex = lista.Count - 1;

        int meu = GameManager.instance != null ? GameManager.instance.score : 0;
        while (alvoIndex >= 0 && meu > lista[alvoIndex].score)
            alvoIndex--;

        if (textoAviso != null)
            textoAviso.gameObject.SetActive(false);

        Atualizar();
    }

    void Update()
    {
        if (GameManager.instance == null || lista == null)
            return;

        int meu = GameManager.instance.score;

        while (alvoIndex >= 0 && meu > lista[alvoIndex].score)
        {
            MostrarAviso("Você passou " + lista[alvoIndex].nome + "!");
            alvoIndex--;
            Atualizar();
        }
    }

    void Atualizar()
    {
        if (textoAlvo == null)
            return;

        if (lista.Count == 0)
        {
            textoAlvo.text = "Seja o 1º do ranking!";
            return;
        }

        if (alvoIndex < 0)
        {
            textoAlvo.text = "Você está em 1º!";
            return;
        }

        Ranking.Entrada alvo = lista[alvoIndex];
        textoAlvo.text = "Próximo: " + (alvoIndex + 1) + "º  " + alvo.nome + "  (" + alvo.score + ")";
    }

    void MostrarAviso(string msg)
    {
        if (textoAviso == null)
            return;

        if (avisoRotina != null)
            StopCoroutine(avisoRotina);

        avisoRotina = StartCoroutine(RotinaAviso(msg));
    }

    IEnumerator RotinaAviso(string msg)
    {
        textoAviso.text = msg;
        textoAviso.gameObject.SetActive(true);

        yield return new WaitForSecondsRealtime(duracaoAviso);

        textoAviso.gameObject.SetActive(false);
        avisoRotina = null;
    }
}
