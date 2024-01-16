using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace com.Instant.Mishor.Data.Entities
{
    public partial class Country : Identifiable<string>
    {
        [Attr]
        public string? Name { get; set; }

        [Attr]
        public string? Continent { get; set; }

        [Attr]
        public string? Region { get; set; }

        [Attr]
        public string? Currency { get; set; }

        [HasMany]
        public ICollection<Destination>? Destinations { get; set; }
    }
}