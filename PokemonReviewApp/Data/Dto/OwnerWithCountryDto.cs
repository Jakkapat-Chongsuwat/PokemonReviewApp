namespace PokemonReviewApp.Data.Dto
{
    public class OwnerWithCountryDto
    {
        // Properties from Owner
        public int OwnerId { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerGym { get; set; }

        // Properties from Country
        public int CountryId { get; set; }
        public string CountryName { get; set; }
    }
}
