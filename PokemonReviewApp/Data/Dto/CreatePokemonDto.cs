namespace PokemonReviewApp.Data.Dto
{
    public class CreatePokemonDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime BirthDate { get; set; }
        public int OwnerId { get; set;}
        public int CategoryId { get; set;}
    }
}
