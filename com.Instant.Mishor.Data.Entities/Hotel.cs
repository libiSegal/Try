using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using System.Xml.Serialization;

namespace com.Instant.Mishor.Data.Entities
{
    public partial class Hotel : Identifiable<int>
    {
        [Attr]
        public string? Name { get; set; }

        [Attr]
        public string? Address { get; set; }

        [Attr]
        public int Status { get; set; }

        [Attr]
        public string? ZipCode { get; set; }

        [Attr]
        public string? Phone { get; set; }

        [Attr]
        public string? Fax { get; set; }

        [Attr]
        public decimal Latitude { get; set; }

        [Attr]
        public decimal Longitude { get; set; }

        [Attr]
        public float Stars { get; set; }

        [Attr]
        public string? SeoName { get; set; }

        [HasMany]
        public List<Destination>? Destinations { get; set; }

        [HasMany]
        public List<HotelDestination>? HotelDestinations { get; set; }
    }
}