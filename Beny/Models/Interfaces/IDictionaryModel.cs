namespace Beny.Models.Interfaces
{
    public interface IDictionaryModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}
