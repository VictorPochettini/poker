public enum Naipe {
    Copas,
    Ouros,
    Paus,
    Espadas
}
public class Carta
{
    //Preciso reescrever o toString para imprimir a carta, convertendo o naipe no caracter correspondente
    private Naipe naipe { get; set; }
    private int valor { get; set; }
    private Boolean naMesa { get; set; }
    private Boolean virada { get; set; }
    private int ordem { get; set; }
    public Carta(Naipe naipe, int valor)
    {
        this.naipe = naipe;
        this.valor = valor;
    }
    public Carta(Naipe naipe, int valor, Boolean naMesa)
    {
        this.naipe = naipe;
        this.valor = valor;
        this.naMesa = naMesa;
    }
    public void mudaVirada(Boolean virada)
    {
        this.virada = virada;
    }
    public int getValor()
    {
        return valor;
    }
    public Naipe getNaipe()
    {
        return naipe;
    }
    public Boolean getNaMesa()
    {
        return naMesa;
    }
    public Boolean getVirada()
    {
        return virada;
    }
    public int getOrdem()
    {
        return ordem;
    }
    
}