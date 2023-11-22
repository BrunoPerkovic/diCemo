namespace SharedBL.Database;

public class EntityNotFoundException : Exception
{
    public EntityNotFoundException() : base("Entity not found in the database.")
    {
    }
    
    public EntityNotFoundException(string message) : base(message) {}
}