namespace Sieve.Models.Interface
{
    public interface ISortTerm
    {
        string Sort { set; }
        bool Descending { get; }
        string Name { get; }
    }
}
