using System.Collections.Generic;
using SaasIPRegistration.Demo.Models;

namespace SaasIPRegistration.Demo.Models.Responses
{
    public class DeviceInteractionResponse
    {
        public bool IsHttps { get; set; }
        public DeviceDetails Device { get; set; }
        public IEnumerable<string> Commands { get; set; }
    }
}
