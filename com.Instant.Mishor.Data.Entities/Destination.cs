using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;

namespace com.Instant.Mishor.Data.Entities
{
    public partial class Destination : Identifiable<int>
    {
        [Attr]
        public string? Name { get; set; }

        [Attr]
        public string? Type { get; set; }

        [Attr]
        public decimal Latitude { get; set; }

        [Attr]
        public decimal Longitude { get; set; }

        [Attr]
        public string? CountryId { get; set; }

        [Attr]
        public bool Searchable { get; set; }

        [Attr]
        public string? SeoName { get; set; }

        [Attr]
        public string? State { get; set; }

        [Attr]
        public string? Contains { get; set; }

        [HasOne]
        public Country? Country { get; set; }

        [HasMany]
        public List<Hotel>? Hotels { get; set; }

        [HasMany]
        public List<HotelDestination>? HotelDestinations { get; set; }
    }
}