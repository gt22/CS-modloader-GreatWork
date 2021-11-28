namespace GreatWorkIvory.Entities
{
    public interface IBeachcomberEntity<out T>
    {
        T Value { get; }
    }
}