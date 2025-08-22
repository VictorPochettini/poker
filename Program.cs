using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.ConstrainedExecution;
using System.Linq;

enum Mao
{
    RoyalFlush,
    StraightFlush,
    FourOfAKind,
    FullHouse,
    Flush,
    Straight,
    ThreeOfAKind,
    TwoPair,
    OnePair,
    HighCard
}
public class Program
{
    static Mao avaliador(List<Carta> maoFinal)
    {
        maoFinal = maoFinal.OrderBy(n => n.getValor()).ToList();

        var grupos = maoFinal.GroupBy(n => n.getValor());
        int flagNumero = 0;
        foreach (var grupo in grupos)
        {
            if (grupo.Count() == 4)
            {
                return Mao.FourOfAKind;
            }
            else if (grupo.Count() == 2)
                flagNumero += 2;
            else if (grupo.Count() == 3)
                flagNumero += 3;
        }

        bool flagNaipe = maoFinal.All(n => n.getNaipe() == maoFinal[0].getNaipe());

        bool flagSequencia = true;
        if (flagNumero != 0)
        {
            for (int i = 1; i < 7; i++)
            {
                if (maoFinal[i].getValor() != maoFinal[i - 1].getValor() + 1)
                {
                    flagSequencia = false;
                    break;
                }
            }
        }
        switch (flagNumero)
        {
            case 5: return Mao.FullHouse;
            case 4: return Mao.TwoPair;
            case 3: return Mao.ThreeOfAKind;
            case 2: return Mao.OnePair;
        }
        if (flagSequencia && flagNaipe)
        {
            if (maoFinal[0].getValor() == 9)
                return Mao.RoyalFlush;
            else
                return Mao.StraightFlush;
        }
        if (flagSequencia)
        {
            return Mao.Straight;
        }
        if (flagNaipe)
        {
            return Mao.Flush;
        }
        return Mao.HighCard;


    }

    // Lidar com o jogador não tendo fichas o suficiente para pagar a aposta
    static Carta[] distribuidor(List<Jogador> jogadores)
    {
        List<Carta> retirados = new List<Carta>();
        Random random = new Random();
        Carta[] mesa = new Carta[5];

        foreach (Jogador jogador in jogadores)
        {
            if (jogador.fichas == 0)
            {
                continue;
            }
            for (int i = 0; i < 2; i++)
            {
                Carta carta;
                do
                {
                    Naipe naipe = (Naipe)random.Next(0, 4);
                    int valor = random.Next(1, 14);
                    carta = new Carta(naipe, valor);
                } while (retirados.Contains(carta));
                retirados.Add(carta);
                jogador.getMão()[i] = carta;
            }
        }

        for (int i = 0; i < 5; i++)
        {
            Carta carta;
            do
            {
                Naipe naipe = (Naipe)random.Next(0, 4);
                int valor = random.Next(1, 14);
                carta = new Carta(naipe, valor, true);
            } while (retirados.Contains(carta));
            retirados.Add(carta);
            mesa[i] = carta;
        }

        return mesa;
    }

    static Boolean achaFim(List<Jogador> jogadores)
    {
        int numInativos = 0;
        foreach (Jogador jogador in jogadores)
        {
            if (jogador.fichas == 0)
            {
                numInativos++;
            }
        }
        return numInativos >= jogadores.Count - 1;
    }

    static void rodada(Carta[] mesa, List<Jogador> jogadores, int j, int aposta = 20)
    {
        Boolean encerrado = false;
        Utils.ordenar(jogadores, j);
        foreach (Jogador jogador in jogadores)
        {
            if (jogador.fichas == 0 || jogador.estado == Estado.Folded)
            {
                continue;
            }

            while (true)
            {
                Utils.imprimirMesa(mesa, jogadores[j], jogadores);
                int escolha = int.Parse(Console.ReadLine());
                switch (escolha)
                {
                    case 1: // Apostar
                        Console.WriteLine("\nQuanto você quer apostar?");
                        int apostado = int.Parse(Console.ReadLine());
                        if (apostado > jogador.fichas)
                        {
                            Console.WriteLine("\nVocê não tem fichas suficientes para essa aposta.");
                            Console.Clear();
                            continue;
                        }
                        else
                        {
                            jogador.pagaMesa(apostado);
                            Console.WriteLine($"\n{jogador.nome} apostou {apostado} fichas.");
                            if (apostado > aposta)
                            {
                                Console.Clear();
                                rodada(mesa, jogadores, (jogadores.IndexOf(jogador) + 1) % jogadores.Count, apostado);
                                encerrado = true;
                                break;
                            }
                            else if (apostado < aposta)
                            {
                                Console.WriteLine("A aposta não pode ser menor que a aposta atual.");
                                Console.Clear();
                                continue;
                            }
                            break;
                        }
                    case 2: // Verificar
                        Console.WriteLine($"{jogador.nome} verificou a aposta.");
                        Console.Clear();
                        break;
                    case 3: // Desistir
                        jogador.estado = Estado.Folded;
                        Console.WriteLine($"{jogador.nome} desistiu da rodada.");
                        Console.Clear();
                        break;
                    default:
                        Console.WriteLine("Escolha inválida. Tente novamente.");
                        Console.Clear();
                        continue;
                }
                break;
            }

            if (encerrado)
            {
                break;
            }
        }
    }
    static void Main(string[] args)
    {
        int numJogadores;
        List<Jogador> jogadores = new List<Jogador>();
        Random random = new Random();

        Console.WriteLine("Bem-vindo ao jogo de Poker!");
        Console.WriteLine("Quantos jogadores participarão? (4 a 10)");
        numJogadores = int.Parse(Console.ReadLine());
        for (int i = 0; i < numJogadores; i++)
        {
            Console.WriteLine("Digite o nome do jogador " + (i + 1) + ":");
            string nome = Console.ReadLine();
            Jogador jogador = new Jogador(nome, 1000, Estado.Padrão);
            jogadores.Add(jogador);
        }

        Carta[] mesa = distribuidor(jogadores);
        int sorteado = random.Next(0, numJogadores);
        jogadores[sorteado].estado = Estado.Dealer;
        jogadores[(sorteado + 1) % jogadores.Count].estado = Estado.SmallBlind;
        jogadores[(sorteado + 2) % jogadores.Count].estado = Estado.BigBlind;


        while (achaFim(jogadores))
        {
            int montante = 0;
            //Small e Big pagam
            montante += Utils.encontraEstado(jogadores, Estado.SmallBlind).pagaMesa(10);
            montante += Utils.encontraEstado(jogadores, Estado.BigBlind).pagaMesa(20);
            int j = (jogadores.IndexOf(Utils.encontraEstado(jogadores, Estado.BigBlind)) + 1) % jogadores.Count;
            Utils.limparMesa(mesa, jogadores);
            mesa = distribuidor(jogadores);

            for (int i = 0; i < 4; i++)
            {
                if (i == 1)
                {
                    mesa[0].mudaVirada(true);
                    mesa[1].mudaVirada(true);
                    mesa[2].mudaVirada(true);
                }
                if (i > 1)
                {
                    mesa[i + 2].mudaVirada(true);
                }
                rodada(mesa, jogadores, j);
            }

            int dealerIndex = jogadores.IndexOf(Utils.encontraEstado(jogadores, Estado.Dealer));

            foreach (var jogador in jogadores)
                jogador.estado = Estado.Padrão;

            int novoDealer = (dealerIndex + 1) % jogadores.Count;
            int novoSmall = (novoDealer + 1) % jogadores.Count;
            int novoBig = (novoDealer + 2) % jogadores.Count;

            jogadores[novoDealer].estado = Estado.Dealer;
            jogadores[novoSmall].estado = Estado.SmallBlind;
            jogadores[novoBig].estado = Estado.BigBlind;
            j = (j + 1) % jogadores.Count;

        }
    }
}