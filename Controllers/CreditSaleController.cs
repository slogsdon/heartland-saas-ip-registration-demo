using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using SaasIPRegistration.Demo.Models;
using SaasIPRegistration.Demo.Models.Requests;
using SaasIPRegistration.Demo.Models.Responses;

using GlobalPayments.Api.Entities;
using GlobalPayments.Api.Services;
using GlobalPayments.Api.Terminals;

namespace SaasIPRegistration.Demo.Controllers
{
    [Route("api/[controller]")]
    public class CreditSaleController : Controller
    {
        /**
         * Simple map to show station to device mapping
         *
         * In production, this should be driven by a more robust
         * data store.
         */
        public readonly string[] StationDeviceMap = {
            "51245851"
        };
        private readonly IMemoryCache _memoryCache;

        public CreditSaleController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        // POST: api/creditSale
        [HttpPost]
        public DeviceInteractionResponse Post([FromBody] CreditSaleRequest request)
        {
            var serialNumber = StationDeviceMap[request.StationId] as string;
            var deviceDetails = GetDeviceDetails(serialNumber);

            return new DeviceInteractionResponse
            {
                IsHttps = false,
                Device = deviceDetails,
                Commands = new string[] {
                    // Requests.AllowCors,
                    Requests.Initialize,
                    Requests.Sale(request.RefNumber, request.Amount),
                }
            };
        }

        protected DeviceDetails GetDeviceDetails(string serialNumber)
        {
            var sessionKey = DeviceDetails.GetSessionKey(serialNumber);
            return _memoryCache.Get(sessionKey) as DeviceDetails;
        }
    }

    internal class Requests
    {
        public static string AllowCors =
            Convert.ToBase64String(TerminalUtilities.BuildRequest(
                "A04", // FOR PAX THE FIRST ELEMENT SHOULD ALWAYS BE THE MESSAGE ID
                "00",
                ControlCodes.FS,
                "accessControlAllowed",
                ControlCodes.FS,
                "Y"
            ).GetSendBuffer());

        public static string Initialize =
            Convert.ToBase64String(TerminalUtilities.BuildRequest("A00").GetSendBuffer());

        public static string Sale(string refNumber, string amount)
        {
            return Convert.ToBase64String(TerminalUtilities.BuildRequest(
                "T00",
                "01", // SALE_REDEEM
                ControlCodes.FS,
                amount, // $1.00
                ControlCodes.FS,
                ControlCodes.FS,
                refNumber, // reference number. needs to increment to prevent device-level duplicate checks
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS,
                ControlCodes.FS
            ).GetSendBuffer());
        }
    }
}
