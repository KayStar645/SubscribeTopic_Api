namespace Sieve.Models.Interface
{
    public interface ISievePropertyMetadata
    {
        string Name { get; set; }
        string FullName { get; }
        bool CanFilter { get; set; }
        bool CanSort { get; set; }
    }
}
