public class Utils
{
    public static void ImprimirTransicao(string proximoJogador)
    {
        Console.Clear();
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine($"      PASSANDO PARA: {proximoJogador.ToUpper()}");
        Console.WriteLine(new string('=', 50));
        Console.WriteLine("\n🔄 Passe o computador para o próximo jogador");
        Console.WriteLine("📱 Pressione ENTER quando estiver pronto...");
        Console.ReadLine();
        Console.Clear();
    }

    public static void ImprimirMesa(Carta[] mesa, Jogador jogadorAtual, List<Jogador> jogadores, int pot, int apostaAtual)
    {
        Console.WriteLine($"\n=== VEZ DE {jogadorAtual.Nome.ToUpper()} ===");
        Console.WriteLine($"Pot: ${pot}");
        Console.WriteLine($"Aposta atual para igualar: ${apostaAtual - jogadorAtual.ApostaAtual}");
        
        Console.WriteLine("\nMesa:");
        foreach (Carta carta in mesa)
        {
            if (carta != null && carta.Virada)
                Console.Write(carta + " ");
            else if (carta != null)
                Console.Write("** ");
        }
        
        Console.WriteLine($"\n\nSuas cartas: {jogadorAtual.GetMao()[0]} {jogadorAtual.GetMao()[1]}");
        Console.WriteLine($"Suas fichas: ${jogadorAtual.Fichas}");
        Console.WriteLine($"Sua aposta atual: ${jogadorAtual.ApostaAtual}");
        
        Console.WriteLine("\nOutros jogadores:");
        foreach (Jogador j in jogadores)
        {
            if (j != jogadorAtual)
            {
                string status = j.Estado == Estado.Folded ? " (FOLD)" : 
                               j.Estado == Estado.AllIn ? " (ALL-IN)" : "";
                Console.WriteLine($"{j.Nome}: ${j.Fichas} fichas (apostou ${j.ApostaAtual}){status}");
            }
        }
    }

    public static Jogador EncontraEstado(List<Jogador> jogadores, Estado estado)
    {
        return jogadores.FirstOrDefault(j => j.Estado == estado);
    }

    public static List<Jogador> OrdenarJogadores(List<Jogador> jogadores, int indiceInicial)
    {
        List<Jogador> ordenados = new List<Jogador>();
        int count = jogadores.Count;
        
        for (int i = 0; i < count; i++)
        {
            ordenados.Add(jogadores[(indiceInicial + i) % count]);
        }
        
        return ordenados;
    }
}