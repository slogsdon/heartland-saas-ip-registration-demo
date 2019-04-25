// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

/**
 * Note: This JavaScript file in combination with the ~/Views/Home/Index.cshtml file
 * are representative of the on-prem client-side application. This can be the UI
 * layer for a SaaS product, a native mobile app, etc. As long as the client-side
 * application is running on the same network as the semi-integrated peripheral,
 * the client-side application should have no problem communicating with the
 * semi-integrated peripheral.
 */

document.addEventListener("DOMContentLoaded", () => {
    const button = document.getElementById("start-pax-sale");

    if (button) {
        button.addEventListener("click", async (e) => {
            e.preventDefault();

            const amountField = document.getElementById("sale-amount");
            const refNumberField = document.getElementById("sale-ref-number");

            const amount = amountField ? amountField.value : "1.00";
            // this should be unique per request per semi-integrated peripheral
            const refNumber = refNumberField ? refNumberField.value : "1";

            try {
                // the below request serves two purposes:
                //
                // - get the connection details for station #0's associated
                //   semi-integrated peripheral
                // - get the Base64 encoded request(s) for completing the
                //   CreditSale transaction
                const response = await fetch("/api/creditSale", {
                    method: "POST",
                    headers: {
                        "Content-Type": "application/json",
                    },
                    body: JSON.stringify({
                        stationId: 0,
                        refNumber: refNumber,
                        amount: amount * 100,
                    }),
                });

                const json = await response.json();
                // scope the eventual `postMessage` call from the popup
                // to the current origin
                json.origin = window.location.origin;
                // general configuration for the popup
                const popupOrigin = "http://localhost:5000";
                const popupOptions = "width=10,height=10,menubar=no,location=no,resizable=no,scrollbars=no,status=no";
                const popup = window.open(
                    // send all of the necessary device connection details and
                    // required commends via the location hash. another option
                    // here would be to wait for the popup window to completely
                    // load and use `postMessage` to send this data
                    `${popupOrigin}/home/popup#${btoa(JSON.stringify(json))}`,
                    "Device Proxy",
                    popupOptions
                );

                // we're waiting for the popup window to complete it's work
                window.addEventListener("message", (e) => {
                    // verify the message is coming from the popup's origin
                    if (e.origin !== popupOrigin) {
                        return;
                    }
                    // close the window since it's not needed anymore
                    popup.close();
                    // handle the response(s) from the device
                    console.log(e);
                });
            } catch (e) {
                console.error(e);
            }
        });
    }

    /**
     * SignalR integration to watch for new device registrations
     */
    
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/deviceHub")
        .build();
    
    connection.on("ReceiveDeviceRegistration", (deviceDetails) => {
        console.log("DeviceRegistration", deviceDetails);
        const list = document.getElementById("device-list");
        if (!list) {
            return;
        }
    
        const li = document.createElement("li");
        li.textContent = JSON.stringify(deviceDetails);
        list.appendChild(li);
    });
    
    connection.start()
        .catch((err) => console.error(err.toString()));
});
