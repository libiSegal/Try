using JsonApiDotNetCore.Resources;
using JsonApiDotNetCore.Resources.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace com.Instant.Mishor.Data.Entities
{
    public partial class HotelDestination : Identifiable<int>
    {
        [Attr]
        public int HotelId { get; set; }

        [Attr]
        public int DestinationId { get; set; }

        [Attr]
        public int Surroundings { get; set; }

        [HasOne]
        public Hotel? Hotel { get; set; }

        [HasOne]
        public Destination? Destination { get; set; }
    }
}