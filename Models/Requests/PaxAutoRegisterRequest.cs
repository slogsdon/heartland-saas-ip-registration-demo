using SaasIPRegistration.Demo.Models;

namespace SaasIPRegistration.Demo.Models.Requests
{
    public class PaxAutoRegisterRequest
    {
        public string SerialNo { get; set; }
        public string Token { get; set; }
        public string TerminalId { get; set; }
        public string IPaddress { get; set; }
        public string Port { get; set; }
        public string MacAddress { get; set; }

        public DeviceDetails ToDeviceDetails()
        {
            return new DeviceDetails
            {
                IsActive = true,
                SerialNumber = this.SerialNo,
                Token = this.Token,
                TerminalId = this.TerminalId,
                IpAddress = this.IPaddress,
                Port = this.Port,
                MacAddress = this.MacAddress
            };
        }
    }
}
