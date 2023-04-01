namespace outlay_mvc.Entities;

public class EntityBase
{
    public int Id { get; set; }
    public DateTime CreateAt { get; set; }
    public DateTime? LastUpdateAt { get; set; }
    public string? Description { get; set; }
}
