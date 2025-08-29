using System;
using System.Collections.Generic;
using System.Linq;


public class Program
{
    private static readonly Random random = new Random();

    public static Mao AvaliarMao(List<Carta> cartas)
    {
        if (cartas.Count != 7)
            throw new ArgumentException($"Avaliação de mão requer exatamente 7 cartas, recebidas: {cartas.Count}");

        // Ordena as cartas por valor (Ás pode ser 1 ou 14)
        var cartasOrdenadas = cartas.OrderBy(c => c.Valor).ToList();

        // Agrupa por valor
        var grupos = cartasOrdenadas.GroupBy(c => c.Valor).OrderByDescending(g => g.Count()).ThenByDescending(g => g.Key).ToList();
        var contagens = grupos.Select(g => g.Count()).ToList();

        // Verifica flush
        var gruposNaipe = cartasOrdenadas.GroupBy(c => c.Naipe).ToList();
        bool temFlush = gruposNaipe.Any(g => g.Count() >= 5);

        // Verifica sequência
        bool temSequencia = VerificarSequencia(cartasOrdenadas);
        bool temSequenciaAs = VerificarSequenciaComAs(cartasOrdenadas);

        // Determina a melhor mão
        if (temFlush && (temSequencia || temSequenciaAs))
        {
            // Verifica Royal Flush (10, J, Q, K, A do mesmo naipe)
            var cartasFlush = gruposNaipe.First(g => g.Count() >= 5).OrderBy(c => c.Valor).ToList();
            if (cartasFlush.Any(c => c.Valor == 1) && cartasFlush.Any(c => c.Valor == 10) &&
                cartasFlush.Any(c => c.Valor == 11) && cartasFlush.Any(c => c.Valor == 12) &&
                cartasFlush.Any(c => c.Valor == 13))
            {
                return Mao.RoyalFlush;
            }
            return Mao.StraightFlush;
        }

        if (contagens[0] == 4) return Mao.FourOfAKind;
        if (contagens[0] == 3 && contagens[1] == 2) return Mao.FullHouse;
        if (temFlush) return Mao.Flush;
        if (temSequencia || temSequenciaAs) return Mao.Straight;
        if (contagens[0] == 3) return Mao.ThreeOfAKind;
        if (contagens[0] == 2 && contagens[1] == 2) return Mao.TwoPair;
        if (contagens[0] == 2) return Mao.OnePair;

        return Mao.HighCard;
    }

    private static bool VerificarSequencia(List<Carta> cartas)
    {
        var valores = cartas.Select(c => c.Valor).Distinct().OrderBy(v => v).ToList();
        int sequencia = 1;

        for (int i = 1; i < valores.Count; i++)
        {
            if (valores[i] == valores[i - 1] + 1)
            {
                sequencia++;
                if (sequencia >= 5) return true;
            }
            else
            {
                sequencia = 1;
            }
        }

        return false;
    }

    private static bool VerificarSequenciaComAs(List<Carta> cartas)
    {
        // Verifica sequência A-2-3-4-5 (wheel)
        var temAs = cartas.Any(c => c.Valor == 1);
        var tem2 = cartas.Any(c => c.Valor == 2);
        var tem3 = cartas.Any(c => c.Valor == 3);
        var tem4 = cartas.Any(c => c.Valor == 4);
        var tem5 = cartas.Any(c => c.Valor == 5);

        return temAs && tem2 && tem3 && tem4 && tem5;
    }

    public static Carta[] Distribuir(List<Jogador> jogadores)
    {
        HashSet<Carta> cartasUsadas = new HashSet<Carta>();
        Carta[] mesa = new Carta[5];

        // Distribui 2 cartas para cada jogador ativo
        foreach (Jogador jogador in jogadores.Where(j => j.Estado != Estado.Folded && j.Fichas > 0))
        {
            for (int i = 0; i < 2; i++)
            {
                Carta carta;
                do
                {
                    Naipe naipe = (Naipe)random.Next(0, 4);
                    int valor = random.Next(1, 14);
                    carta = new Carta(naipe, valor);
                } while (cartasUsadas.Contains(carta));

                cartasUsadas.Add(carta);
                jogador.SetMao(carta, i);
            }
        }

        // Distribui 5 cartas para a mesa
        for (int i = 0; i < 5; i++)
        {
            Carta carta;
            do
            {
                Naipe naipe = (Naipe)random.Next(0, 4);
                int valor = random.Next(1, 14);
                carta = new Carta(naipe, valor, true);
            } while (cartasUsadas.Contains(carta));

            cartasUsadas.Add(carta);
            mesa[i] = carta;
        }

        return mesa;
    }

    public static bool TemJogadoresAtivos(List<Jogador> jogadores)
    {
        return jogadores.Count(j => j.Estado != Estado.Folded && j.Fichas > 0) > 1;
    }

    public static int RodadaDeApostas(List<Jogador> jogadores, Carta[] mesa, int pot, int apostaMinima, int indiceInicial)
    {
        var jogadoresOrdenados = Utils.OrdenarJogadores(jogadores, indiceInicial);
        bool rodadaCompleta = false;
        int maiorAposta = jogadores.Where(j => j.Estado != Estado.Folded).Max(j => j.ApostaAtual);

        if (maiorAposta < apostaMinima) maiorAposta = apostaMinima;

        while (!rodadaCompleta && TemJogadoresAtivos(jogadores))
        {
            rodadaCompleta = true;

            foreach (var jogador in jogadoresOrdenados)
            {
                if (jogador.Estado == Estado.Folded || jogador.Estado == Estado.AllIn)
                    continue;

                if (jogador.ApostaAtual < maiorAposta)
                {
                    rodadaCompleta = false;

                    // Transição entre jogadores
                    Utils.ImprimirTransicao(jogador.Nome);

                    Utils.ImprimirMesa(mesa, jogador, jogadores, pot, maiorAposta);

                    int valorParaIgualar = maiorAposta - jogador.ApostaAtual;

                    Console.WriteLine("\nEscolha sua ação:");
                    if (valorParaIgualar > 0)
                    {
                        Console.WriteLine($"1. Call (igualar ${valorParaIgualar})");
                        Console.WriteLine($"2. Raise (aumentar)");
                        Console.WriteLine("3. Fold (desistir)");
                    }
                    else
                    {
                        Console.WriteLine("1. Check (passar)");
                        Console.WriteLine("2. Bet (apostar)");
                        Console.WriteLine("3. Fold (desistir)");
                    }

                    Console.Write("Sua escolha: ");
                    if (!int.TryParse(Console.ReadLine(), out int escolha))
                    {
                        Console.WriteLine("Entrada inválida!");
                        continue;
                    }

                    switch (escolha)
                    {
                        case 1: // Call/Check
                            if (valorParaIgualar > 0)
                            {
                                int pagamento = jogador.PagaMesa(valorParaIgualar);
                                pot += pagamento;
                                Console.WriteLine($"{jogador.Nome} igualou com ${pagamento}");
                            }
                            else
                            {
                                Console.WriteLine($"{jogador.Nome} passou (check)");
                            }
                            break;

                        case 2: // Raise/Bet
                            Console.Write("Quanto você quer apostar? $");
                            if (int.TryParse(Console.ReadLine(), out int aposta))
                            {
                                if (valorParaIgualar > 0)
                                {
                                    aposta += valorParaIgualar; // Soma o valor para igualar
                                }

                                if (aposta > jogador.Fichas)
                                {
                                    Console.WriteLine("Você não tem fichas suficientes! Indo all-in...");
                                    aposta = jogador.Fichas;
                                }

                                int pagamento = jogador.PagaMesa(aposta);
                                pot += pagamento;

                                if (jogador.ApostaAtual > maiorAposta)
                                {
                                    maiorAposta = jogador.ApostaAtual;
                                    rodadaCompleta = false; // Reinicia a rodada
                                }

                                Console.WriteLine($"{jogador.Nome} apostou ${pagamento}");
                            }
                            break;

                        case 3: // Fold
                            jogador.Estado = Estado.Folded;
                            Console.WriteLine($"{jogador.Nome} desistiu");
                            break;

                        default:
                            Console.WriteLine("Escolha inválida!");
                            continue;
                    }

                    Console.WriteLine("Pressione Enter para continuar...");
                    Console.ReadLine();
                }
            }
        }

        return pot;
    }

    public static void DeterminarVencedor(List<Jogador> jogadores, Carta[] mesa, int pot)
    {
        var jogadoresAtivos = jogadores.Where(j => j.Estado != Estado.Folded).ToList();

        if (jogadoresAtivos.Count == 1)
        {
            var vencedor = jogadoresAtivos[0];
            vencedor.Fichas += pot;
            Console.WriteLine($"\n🎉 {vencedor.Nome} venceu por W.O. e ganhou ${pot}!");
            return;
        }

        Console.WriteLine("\n=== SHOWDOWN ===");

        var resultados = new List<(Jogador jogador, Mao melhorMao, List<Carta> cartas)>();

        foreach (var jogador in jogadoresAtivos)
        {
            var todasCartas = new List<Carta>();
            todasCartas.AddRange(jogador.GetMao().Where(c => c != null));
            todasCartas.AddRange(mesa.Where(c => c != null));

            var melhorMao = AvaliarMao(todasCartas);
            resultados.Add((jogador, melhorMao, todasCartas));

            Console.WriteLine($"{jogador.Nome}: {jogador.GetMao()[0]} {jogador.GetMao()[1]} - {melhorMao}");
        }

        var vencedores = resultados
            .GroupBy(r => r.melhorMao)
            .OrderByDescending(g => (int)g.Key)
            .First()
            .ToList();

        int premioParaCada = pot / vencedores.Count;
        int resto = pot % vencedores.Count;

        Console.WriteLine($"\n🎉 VENCEDOR(ES):");
        foreach (var (jogador, mao, _) in vencedores)
        {
            int premio = premioParaCada + (resto > 0 ? 1 : 0);
            resto--;
            jogador.Fichas += premio;
            Console.WriteLine($"{jogador.Nome} - {mao} - Ganhou ${premio}!");
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("=== BEM-VINDO AO POKER TEXAS HOLD'EM ===\n");

        List<Jogador> jogadores = new List<Jogador>();

        Console.Write("Quantos jogadores (2-10)? ");
        if (!int.TryParse(Console.ReadLine(), out int numJogadores) || numJogadores < 2 || numJogadores > 10)
        {
            Console.WriteLine("Número inválido! Usando 4 jogadores.");
            numJogadores = 4;
        }

        for (int i = 0; i < numJogadores; i++)
        {
            Console.Write($"Nome do jogador {i + 1}: ");
            string nome = Console.ReadLine() ?? $"Jogador {i + 1}";
            if (string.IsNullOrWhiteSpace(nome))
                nome = $"Jogador {i + 1}";

            jogadores.Add(new Jogador(nome, 1000));
        }

        int indiceDealer = random.Next(0, numJogadores);
        int rodada = 0;

        while (jogadores.Count(j => j.Fichas > 0) > 1)
        {
            rodada++;
            Console.Clear();
            Console.WriteLine($"\n=== RODADA {rodada} ===");

            // Define posições
            jogadores[indiceDealer].Estado = Estado.Dealer;
            jogadores[(indiceDealer + 1) % jogadores.Count].Estado = Estado.SmallBlind;
            jogadores[(indiceDealer + 2) % jogadores.Count].Estado = Estado.BigBlind;

            // Limpa apostas anteriores
            foreach (var j in jogadores)
            {
                j.ApostaAtual = 0;
            }

            // Distribui cartas
            Carta[] mesa = Distribuir(jogadores);

            // Blinds
            int pot = 0;
            var smallBlind = Utils.EncontraEstado(jogadores, Estado.SmallBlind);
            var bigBlind = Utils.EncontraEstado(jogadores, Estado.BigBlind);

            if (smallBlind.Fichas > 0)
            {
                pot += smallBlind.PagaMesa(10);
                Console.WriteLine($"{smallBlind.Nome} pagou Small Blind: $10");
            }

            if (bigBlind.Fichas > 0)
            {
                pot += bigBlind.PagaMesa(20);
                Console.WriteLine($"{bigBlind.Nome} pagou Big Blind: $20");
            }

            Console.WriteLine("Pressione Enter para começar...");
            Console.ReadLine();
            Console.Clear();

            Console.WriteLine("\n=== PRE-FLOP ===");
            int proximoJogador = (indiceDealer + 3) % jogadores.Count;
            foreach (var j in jogadores.Where(j => j.Estado != Estado.Folded))
            {
                j.ApostaAtual = 0;
            }

            smallBlind.ApostaAtual = Math.Min(10, smallBlind.Fichas + 10);
            bigBlind.ApostaAtual = Math.Min(20, bigBlind.Fichas + 20);

            pot = RodadaDeApostas(jogadores, mesa, pot, 20, proximoJogador);

            if (!TemJogadoresAtivos(jogadores))
            {
                DeterminarVencedor(jogadores, mesa, pot);
            }
            else
            {
                // Flop
                mesa[0].MudaVirada(true);
                mesa[1].MudaVirada(true);
                mesa[2].MudaVirada(true);

                Console.WriteLine("\n=== FLOP ===");
                Console.WriteLine($"Mesa: {mesa[0]} {mesa[1]} {mesa[2]}");
                Console.WriteLine("Pressione Enter para começar as apostas do Flop...");
                Console.ReadLine();

                // Reset apostas para nova rodada
                foreach (var j in jogadores.Where(j => j.Estado != Estado.Folded && j.Estado != Estado.AllIn))
                {
                    j.ApostaAtual = 0;
                }

                pot = RodadaDeApostas(jogadores, mesa, pot, 0, (indiceDealer + 1) % jogadores.Count);

                if (TemJogadoresAtivos(jogadores))
                {
                    // Turn
                    mesa[3].MudaVirada(true);

                    Console.WriteLine("\n=== TURN ===");
                    Console.WriteLine($"Mesa: {mesa[0]} {mesa[1]} {mesa[2]} {mesa[3]}");
                    Console.WriteLine("Pressione Enter para começar as apostas do Turn...");
                    Console.ReadLine();

                    // Reset apostas para nova rodada
                    foreach (var j in jogadores.Where(j => j.Estado != Estado.Folded && j.Estado != Estado.AllIn))
                    {
                        j.ApostaAtual = 0;
                    }

                    pot = RodadaDeApostas(jogadores, mesa, pot, 0, (indiceDealer + 1) % jogadores.Count);

                    if (TemJogadoresAtivos(jogadores))
                    {
                        // River
                        mesa[4].MudaVirada(true);

                        Console.WriteLine("\n=== RIVER ===");
                        Console.WriteLine($"Mesa: {mesa[0]} {mesa[1]} {mesa[2]} {mesa[3]} {mesa[4]}");
                        Console.WriteLine("Pressione Enter para começar as apostas do River...");
                        Console.ReadLine();

                        // Reset apostas para nova rodada
                        foreach (var j in jogadores.Where(j => j.Estado != Estado.Folded && j.Estado != Estado.AllIn))
                        {
                            j.ApostaAtual = 0;
                        }

                        pot = RodadaDeApostas(jogadores, mesa, pot, 0, (indiceDealer + 1) % jogadores.Count);

                        // Showdown
                        DeterminarVencedor(jogadores, mesa, pot);
                    }
                    else
                    {
                        DeterminarVencedor(jogadores, mesa, pot);
                    }
                }
                else
                {
                    DeterminarVencedor(jogadores, mesa, pot);
                }
            }

            // Limpa estados e mãos
            foreach (var j in jogadores)
            {
                j.LimparMao();
                if (j.Estado != Estado.Folded)
                {
                    j.Estado = Estado.Padrao;
                }
            }

            // Remove jogadores sem fichas
            jogadores.RemoveAll(j => j.Fichas <= 0);

            if (jogadores.Count <= 1)
            {
                if (jogadores.Count == 1)
                {
                    Console.WriteLine($"\n🏆 {jogadores[0].Nome} é o GRANDE VENCEDOR!");
                }
                break;
            }

            // Próximo dealer
            indiceDealer = (indiceDealer + 1) % jogadores.Count;

            Console.WriteLine("\nFichas atuais:");
            foreach (var j in jogadores.OrderByDescending(x => x.Fichas))
            {
                Console.WriteLine($"{j.Nome}: ${j.Fichas}");
            }

            Console.WriteLine("\nPressione Enter para a próxima rodada...");
            Console.ReadLine();
        }

        Console.WriteLine("\nObrigado por jogar!");
        Console.ReadLine();
    }
}