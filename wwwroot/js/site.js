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

document.addEventListener('DOMContentLoaded', () => {
    const button = document.getElementById('start-pax-sale');

    if (button) {
        button.addEventListener('click', async (e) => {
            e.preventDefault();

            const amountField = document.getElementById('sale-amount');
            const refNumberField = document.getElementById('sale-ref-number');

            const amount = amountField ? amountField.value : '1.00';
            // this should be unique per request per semi-integrated peripheral
            const refNumber = refNumberField ? refNumberField.value : "1";

            try {
                // the below request serves two purposes:
                //
                // - get the connection details for station #0's associated
                //   semi-integrated peripheral
                // - get the Base64 encoded request(s) for completing the
                //   CreditSale transaction
                const response = await fetch('/api/creditSale', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        stationId: 0,
                        refNumber: refNumber,
                        amount: amount * 100,
                    }),
                });

                const json = await response.json();
                const host = `http${json.isHttps ? 's' : ''}://${json.device.ipAddress}:${json.device.port}`;

                for (const i in json.commands) {
                    // get the device response for the current command
                    const resp = await load(`${host}/?${json.commands[i]}`);
                    // the Base64 encoded response data can be parsed locally or
                    // by the remote SaaS service
                    console.log(resp.byteLength, arrayBufferToBase64(resp));
                    console.log(atob(arrayBufferToBase64(resp)));
                }
            } catch (e) {
                console.error(e);
            }
        });

        /**
         * Issues request to `url` as URL-encoded form data
         * @param {string} url 
         * @return {Promise<ArrayBuffer>}
         */
        async function load(url) {
            const response = await fetch(url, {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/x-www-form-urlencoded',
                },
            });
            return await response.arrayBuffer();
        }

        /**
         * Converts a buffer to a Uint8Array and encodes as Base64
         * @param {ArrayBuffer} buffer
         * @return {string}
         */
        function arrayBufferToBase64(buffer) {
            var binary = '';
            var bytes = new Uint8Array(buffer);
            var len = bytes.byteLength;
            for (var i = 0; i < len; i++) {
                binary += String.fromCharCode(bytes[i]);
            }
            return btoa(binary);
        }
    }

    /**
     * SignalR integration to watch for new device registrations
     */
    
    const connection = new signalR.HubConnectionBuilder()
        .withUrl('/deviceHub')
        .build();
    
    connection.on('ReceiveDeviceRegistration', (deviceDetails) => {
        console.log('DeviceRegistration', deviceDetails);
        const list = document.getElementById('device-list');
        if (!list) {
            return;
        }
    
        const li = document.createElement('li');
        li.textContent = JSON.stringify(deviceDetails);
        list.appendChild(li);
    });
    
    connection.start()
        .catch((err) => console.error(err.toString()));
});
