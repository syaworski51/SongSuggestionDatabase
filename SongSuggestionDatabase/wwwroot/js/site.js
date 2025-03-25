document.addEventListener("DOMContentLoaded", function () {
    // Start a SignalR connection for real-time updates
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/live-data")
        .build();

    // When an update is received, refresh the screen
    connection.on("ReceiveUpdate", function () {
        refresh();
    });

    // If an error occurs, return its message
    connection.start().catch(function (errorMessage) {
        return console.error(errorMessage.toString());
    });
})

/**
 * Refresh the page.
 */
function refresh() {
    location.replace(location.href);
}

/**
 * When adding a new request, switch the title and artist around when necessary.
 */
function SwitchTitleAndArtist() {
    // Temporarily hold the title in this variable
    let hold = $("#title-field").val;

    // Set the title field to the artist
    $("#title-field").val = $("#artist-field");

    // Set the artist field to the value in the 'hold' variable
    $("#artist-field").val = hold;
}

/**
 * Get the conversion rate between two currencies.
 * @param {string} key The API key.
 * @param {string} fromCode The currency to convert an amount from.
 * @param {string} toCode The currency to convert an amount to.
 */
function FetchConversionRate(key, fromCode, toCode) {
    // The amount of money to convert
    let amount = $("#amount").val;

    // Let the user know that we are in the process of converting the amount to their desired currency
    $("#result").text = "Calculating...";

    // To minimize calls to the API, wait 1 second before calling the API
    setTimeout(1000);

    // Call the pair conversion endpoint with the key, currency codes and the amount to be converted
    fetch(`https://v6.exchangerate-api.com/v6/${key}/pair/${fromCode}/${toCode}/${amount}`)
        // Convert the response to JSON
        .then(response => {
            return response.json();
        })
        // Display the result on the screen
        .then(conversionRate => {
            let result = conversionRate["conversion_rate"];
            $("#result").text = `${toCode} ${result}`;
        });
}