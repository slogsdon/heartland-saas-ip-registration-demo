using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using SaasIPRegistration.Demo.Hubs;
using SaasIPRegistration.Demo.Models.Requests;

using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;

namespace SaasIPRegistration.Demo.Controllers
{
    [Route("api/[controller]")]
    public class PaxAutoRegisterController : Controller
    {
        private readonly ILogger _logger;
        private readonly IHubContext<DeviceHub> _deviceHubContext;
        private readonly IMemoryCache _memoryCache;

        public PaxAutoRegisterController(
            ILogger<PaxAutoRegisterController> logger,
            IHubContext<DeviceHub> deviceHubContext,
            IMemoryCache memoryCache
        ) {
            _logger = logger;
            _deviceHubContext = deviceHubContext;
            _memoryCache = memoryCache;
        }

        /**
         * Handles POST from Pax semi-integrated devices on IP address change
         *
         * The return value will be presented on the device to the end-user.
         */
        // POST: api/paxAutoRegister
        [HttpPost]
        public string Post([FromForm] PaxAutoRegisterRequest request)
        {
            var deviceDetails = request.ToDeviceDetails();

            // Merchant location identifiers, keys, etc. can be included in
            // the configuration for a device, either via the request path or
            // appended to the request path as a query string. The can help
            // associate a device with a merchant location without knowing
            // this association before receiving the first IP registration
            // request from the device.
            deviceDetails.Token = HttpContext.Request.Query["token"];

            // Use of `_memoryCache` is only for demonstration purposes. In
            // production, this would need to be persisted long-term.
            _memoryCache.Set(deviceDetails.SessionKey, deviceDetails);

            // SignalR is being used to notify the connected clients of the new device.
            // This can be leveraged to prompt end-users to associate the device with a
            // particular clerk station / location (not implemented in this project).
            _deviceHubContext.Clients.All.SendAsync("ReceiveDeviceRegistration", deviceDetails);

            // Perform simple logging for verification
            _logger.LogInformation("IP Registered for device S/N `{1}` and IP address `{2}`", request.SerialNo, request.IPaddress);

            return "Device Registered";
        }
    }
}
