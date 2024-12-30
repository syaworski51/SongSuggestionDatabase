document.addEventListener("DOMContentLoaded", function () {
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/live-data")
        .build();

    connection.on("ReceiveUpdate", function () {
        refresh();
    });

    connection.start().catch(function (errorMessage) {
        return console.error(errorMessage.toString());
    });
})

function refresh() {
    location.replace(location.href);
}

/**
 * When adding a new request, switch the title and artist around when necessary.
 */
function SwitchTitleAndArtist() {
    let hold = $("#title-field").val;
    $("#title-field").val = $("#artist-field");
    $("#artist-field").val = hold;
}

function UpdateTargetCurrencies(currencies, baseCurrency) {

}

function FetchConversionRate(key, fromCode, toCode) {
    let amount = $("#amount").val;

    $("#result").text = "Calculating...";

    setTimeout(750);

    fetch(`https://v6.exchangerate-api.com/v6/${key}/pair/${fromCode}/${toCode}/${amount}`)
        .then(response => {
            return response.json();
        })
        .then(conversionRate => {
            let result = conversionRate["conversion_rate"];
            $("#result").text = `${toCode} ${result}`;
        });
}