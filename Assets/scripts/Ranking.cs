using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class Ranking
{
    private const string KEY_NOME = "nomeJogador";
    private const string KEY_RANKING = "rankingPontuacoes";
    private const int MAX_ENTRADAS = 10;

    [Serializable]
    public class Entrada
    {
        public string nome;
        public int score;
    }

    [Serializable]
    private class Lista
    {
        public List<Entrada> entradas = new List<Entrada>();
    }

    public static void SalvarNome(string nome)
    {
        PlayerPrefs.SetString(KEY_NOME, string.IsNullOrWhiteSpace(nome) ? "" : nome.Trim());
        PlayerPrefs.Save();
    }

    public static string ObterNome()
    {
        return PlayerPrefs.GetString(KEY_NOME, "");
    }

    public static List<Entrada> Obter()
    {
        string json = PlayerPrefs.GetString(KEY_RANKING, "");
        if (string.IsNullOrEmpty(json))
            return new List<Entrada>();

        Lista lista = JsonUtility.FromJson<Lista>(json);
        return lista != null && lista.entradas != null ? lista.entradas : new List<Entrada>();
    }

    public static void Adicionar(string nome, int score)
    {
        if (string.IsNullOrWhiteSpace(nome))
            nome = "Anônimo";

        List<Entrada> lista = Obter();
        lista.Add(new Entrada { nome = nome.Trim(), score = score });
        lista.Sort((a, b) => b.score.CompareTo(a.score));

        if (lista.Count > MAX_ENTRADAS)
            lista.RemoveRange(MAX_ENTRADAS, lista.Count - MAX_ENTRADAS);

        PlayerPrefs.SetString(KEY_RANKING, JsonUtility.ToJson(new Lista { entradas = lista }));
        PlayerPrefs.Save();
    }

    public static string TextoFormatado(int quantidade = MAX_ENTRADAS)
    {
        List<Entrada> lista = Obter();
        if (lista.Count == 0)
            return "Sem pontuações ainda";

        StringBuilder sb = new StringBuilder();
        int n = Mathf.Min(quantidade, lista.Count);
        for (int i = 0; i < n; i++)
            sb.AppendLine((i + 1) + ". " + lista[i].nome + " - " + lista[i].score);

        return sb.ToString().TrimEnd();
    }

    public static void Limpar()
    {
        PlayerPrefs.DeleteKey(KEY_RANKING);
        PlayerPrefs.Save();
    }
}
