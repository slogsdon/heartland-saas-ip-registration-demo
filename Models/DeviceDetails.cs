using System;

namespace SaasIPRegistration.Demo.Models
{
    public class DeviceDetails
    {
        public bool IsActive { get; set; }
        public string SerialNumber { get; set; }
        public string Token { get; set; }
        public string TerminalId { get; set; }
        public string IpAddress { get; set; }
        public string Port { get; set; }
        public string MacAddress { get; set; }
        public string SessionKey { get { return GetSessionKey(SerialNumber); } }

        public static string GetSessionKey(string serialNumber)
        {
            return String.Format("DeviceDetails-{0}", serialNumber);
        }
    }
}