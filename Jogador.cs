public class Jogador
{
    public string Nome { get; set; }
    public int Fichas { get; set; }
    private Carta[] mao = new Carta[2];
    public Estado Estado { get; set; }
    public int ApostaAtual { get; set; }

    public Jogador(string nome, int fichas)
    {
        this.Nome = nome;
        this.Fichas = fichas;
        this.Estado = Estado.Padrao;
        this.ApostaAtual = 0;
    }

    public Carta[] GetMao()
    {
        return mao;
    }

    public void SetMao(Carta carta, int indice)
    {
        if (indice < 0 || indice >= mao.Length)
        {
            throw new ArgumentOutOfRangeException("Índice fora do intervalo da mão do jogador.");
        }
        this.mao[indice] = carta;
    }

    public int PagaMesa(int valor)
    {
        if (Fichas >= valor)
        {
            Fichas -= valor;
            ApostaAtual += valor;
            if (Fichas == 0)
            {
                Estado = Estado.AllIn;
            }
            return valor;
        }
        else if (Fichas > 0)
        {
            // All-in
            int todasFichas = Fichas;
            Fichas = 0;
            ApostaAtual += todasFichas;
            Estado = Estado.AllIn;
            return todasFichas;
        }
        return 0;
    }

    public void LimparMao()
    {
        mao[0] = null;
        mao[1] = null;
        ApostaAtual = 0;
        if (Estado != Estado.Dealer && Estado != Estado.SmallBlind && Estado != Estado.BigBlind)
        {
            Estado = Estado.Padrao;
        }
    }
}