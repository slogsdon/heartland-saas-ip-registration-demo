/**
  * Issues request to `url` as URL-encoded form data
  * @param {string} url 
  * @return {Promise<ArrayBuffer>}
  */
async function load(url) {
    const response = await fetch(url, {
        method: "GET",
        headers: {
            "Content-Type": "application/x-www-form-urlencoded",
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
    var binary = "";
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return btoa(binary);
}

document.addEventListener("DOMContentLoaded", async () => {
    const responses = [];
    const json = JSON.parse(atob(window.location.hash.replace("#", "")));
    const host = `http${json.isHttps ? "s" : ""}://${json.device.ipAddress}:${json.device.port}`;

    for (const i in json.commands) {
        // get the device response for the current command
        const resp = await load(`${host}/?${json.commands[i]}`);
        // the Base64 encoded response data can be parsed locally or
        // by the remote SaaS service
        console.log(resp.byteLength, arrayBufferToBase64(resp));
        console.log(atob(arrayBufferToBase64(resp)));
        responses.push(arrayBufferToBase64(resp));
    }

    // pass the response back to the parent window for parsing
    opener.postMessage(responses, json.origin);
});
