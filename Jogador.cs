
public enum Estado
{
    Padrão,
    Dealer,
    SmallBlind,
    BigBlind,
    Folded
}
public class Jogador
{
    public string nome { get; set; }
    public int fichas { get; set; }
    private Carta[] mão = new Carta[2];
    public Estado estado { get; set; }

    public Jogador(string nome, int fichas, Estado estado)
    {
        this.nome = nome;
        this.fichas = fichas;
        this.estado = Estado.Padrão;
    }

    public Carta[] getMão()
    {
        return mão;
    }

    public int pagaMesa(int valor)
    {
            if (fichas >= valor)
            {
                fichas -= valor;
                return valor;
            }
            else
            {
                return -1; // Indica que o jogador não tem fichas suficientes
            }
    }

}