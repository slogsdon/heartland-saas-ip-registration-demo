using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SaasIPRegistration.Demo.Models;

namespace SaasIPRegistration.Demo.Controllers
{
    public class HomeController : Controller
    {
        /**
         * Demo of the Point of Sale functionality
         *
         * - Prompt for sale amount + reference number
         * - Get device details and encoded commands
         * - Issue commands to device
         */
        public IActionResult Index()
        {
            return View();
        }

        /**
         * Demo of the notification functionality
         *
         * - Wait for new device registrations
         * - Add new device to list
         */
        public IActionResult Devices()
        {
            return View();
        }

        /**
         * Unsecure (HTTP) only endpoint for proxying device requests/responses
         * from a secure context (HTTPS).
         */
        public IActionResult Popup()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
