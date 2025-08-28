public enum Naipe
{
    Copas,
    Ouros,
    Paus,
    Espadas
}

public enum Estado
{
    Padrao,
    Dealer,
    SmallBlind,
    BigBlind,
    Folded,
    AllIn
}

public enum Mao
{
    RoyalFlush = 10,
    StraightFlush = 9,
    FourOfAKind = 8,
    FullHouse = 7,
    Flush = 6,
    Straight = 5,
    ThreeOfAKind = 4,
    TwoPair = 3,
    OnePair = 2,
    HighCard = 1
}

public class Carta
{
    public Naipe Naipe { get; private set; }
    public int Valor { get; private set; }
    public bool NaMesa { get; private set; }
    public bool Virada { get; private set; }

    public Carta(Naipe naipe, int valor, bool naMesa = false)
    {
        this.Naipe = naipe;
        this.Valor = valor;
        this.NaMesa = naMesa;
        this.Virada = false;
    }

    public void MudaVirada(bool virada)
    {
        this.Virada = virada;
    }

    public override string ToString()
    {
        string valorStr = Valor switch
        {
            1 => "A",
            11 => "J",
            12 => "Q",
            13 => "K",
            _ => Valor.ToString()
        };

        return Naipe switch
        {
            Naipe.Copas => valorStr + "♥",
            Naipe.Ouros => valorStr + "♦",
            Naipe.Paus => valorStr + "♣",
            Naipe.Espadas => valorStr + "♠",
            _ => "??"
        };
    }

    public override bool Equals(object obj)
    {
        if (obj is Carta carta)
        {
            return this.Naipe == carta.Naipe && this.Valor == carta.Valor;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Naipe, Valor);
    }
}