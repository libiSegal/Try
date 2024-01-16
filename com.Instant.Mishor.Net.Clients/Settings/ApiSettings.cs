using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.Instant.Mishor.Net.Clients.Settings
{
    public class ApiSettings
    {
        public AetherSettings? Aether { get; set; }

        public string? StaticUrl { get; set; }

        public string? SearchUrl { get; set; }

        public string? BookUrl { get; set; }
    }

    public class AetherSettings
    {
        public string? AccessToken { get; set; }

        public string? ApplicationKey { get; set; }

        public string? AccessTokenStatic { get; set; }
    }
}
