using System.ComponentModel.DataAnnotations;

namespace SharedBL.Database;

public abstract class PostgresBase
{
    [Key] public int Id { get; set; }
}