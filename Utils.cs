using System.Runtime.CompilerServices;

public class Utils
{
    public static void imprimirMesa(Carta[] mesa, Jogador jogador, List<Jogador> jogadores)
    {
        //Vai imprimir baseado em qual jogador está jogando
        //Ele vai imprimir a rodada do jogador, e então vai imprimir a transição de jogador e encerrar, sem limpar o terminal
    }

    public static void limparMesa(Carta[] mesa, List<Jogador> jogadores)
    {
        for (int i = 0; i < mesa.Length; i++)
        {
            mesa[i] = null; // Remove a referência às cartas na mesa
        }
    }

    public static Jogador encontraEstado(List<Jogador> jogadores, Estado estado)
    {
        foreach (Jogador jogador in jogadores)
        {
            if (jogador.estado == estado)
            {
                return jogador;
            }
        }

        return null;
    }

    public static Jogador proximo(List<Jogador> jogadores, Jogador atual, int numJogadores)
    {
        int index = jogadores.IndexOf(atual);
        if (index == numJogadores - 1)
        {
            return jogadores[0];
        }
        return jogadores[index + 1];
    }

    public static void ordenar(List<Jogador> jogadores, int i)
    {
        List<Jogador> aux = new List<Jogador>();
        int n = jogadores.Count;
        aux.Add(jogadores[i]);
        for (int j = 0; j < n; j++)
        {
            int indice = (i + j) % n; // isso garante rotação circular
            aux.Add(jogadores[indice]);
        }
    }

}