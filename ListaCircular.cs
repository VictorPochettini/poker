public class ListaCircular<T> : LinkedList<T>
{
    public LinkedListNode<T> Next(LinkedListNode<T> no)
    {
        return no.Next ?? this.First;
    }
    public LinkedListNode<T> Previous(LinkedListNode<T> no)
    {
        return no.Previous ?? this.Last;
    }
}