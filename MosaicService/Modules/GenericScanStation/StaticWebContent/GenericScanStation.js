$(window).load(function () {
    retrieveVersion();
    retrieveTitle();
    $("#barcodeInput").focus();
});

$("#barcodeInput").blur(function () {
    var barcodeInput = $(this);

    setTimeout(function() {
        barcodeInput.focus();
    }, 100);
});

$("#barcodeInput").keyup(function(event) {
    if (event.keyCode == 13) {
        postBarcode($("#barcodeInput").val(),"");
        $("#barcodeInput").val("");
        $("#boxTypeInput").focus();
    }
});

// This adds a history message bar for each call. Maximum 4 bars
function AddHistoryEntry(barcode, message, classType) {
    var historyElements = document.getElementById('comHistory');
    var historyElement = document.createElement('div');

    historyElement.id = barcode + 'ElementId';
    historyElement.className = 'alert ' + classType + ' fade in';
    historyElement.innerHTML = '<strong>Barcode:</strong> ' + barcode + ' - ' + message;

    var historyLink = document.createElement('a');
    historyLink.href = '#';
    historyLink.className = 'close';
    historyLink.setAttribute('aria-label', 'close');
    historyLink.setAttribute('data-dismiss', 'alert');
    historyLink.innerHTML = '&times;';

    historyElement.appendChild(historyLink);

    if (historyElements.childElementCount == 0) {
        historyElements.appendChild(historyElement);
    } else {
        historyElements.insertBefore(historyElement, historyElements.childNodes[0]);
    }


    if (historyElements.childElementCount > 3) {
        historyElements.removeChild(historyElements.childNodes[historyElements.childElementCount - 1]);
    }
}

function AddErrorHistory(barcode, message) {
    AddHistoryEntry(barcode, message, 'alert-danger');
}

function AddInfoHistory(barcode, message) {
    AddHistoryEntry(barcode, message, 'alert-info');
}

function AddSuccessHistory(barcode, message) {
    AddHistoryEntry(barcode, message, 'alert-success');
}

// Post a barcode the the server. The response is used to display the result + Message
// See the type BarcodeResultJsonType.cs for the result object definition
function postBarcode(barcode, boxType) {
    $.ajax({
        type: 'post',
        url: window.location.protocol + '//' + window.location.host + '/rest/barcode',
        data: JSON.stringify({ Barcode: barcode, BoxType: boxType }),
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, jqXHR) {
            var response = $.parseJSON(data);
            if (response.Result == 0) {
                AddSuccessHistory(barcode, response.Message);
            } else {
                AddErrorHistory(barcode, response.Message);
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var response = $.parseJSON(data);

            AddErrorHistory(barcode, jqXHR.responseJSON.Message);
        }
    });
}

// Displays the version
function displayVersion(version) {
    
}

// Displays the title and subtitle
// title object = {Title (string); Station (string)}
function displayTitle(title) {
    titleLink = document.getElementById('titleLink');
    titleLink.firstChild.data = title.Title;
    stationName = document.getElementById('stationId');
    stationName.firstChild.data = "Station - " + title.Station;
}

// Gets the Version from the server asyncronously and calls the function to diplay it
function retrieveVersion() {
    $.ajax({
        type: 'get',
        url: window.location.protocol + '//' + window.location.host + '/rest/barcode/version',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, jqXHR) {
            var response = $.parseJSON(data);

            displayVersion(response.Version);
        },
        error: function (jqXHR, textStatus, errorThrown) {
            var response = $.parseJSON(data);

            displayVersion("Version Error");
        }
    });
}

// Gets the title and subtitle from the server asyncronously and calls the function to diplay it
// response object = {Title (string); Station (string)}
function retrieveTitle() {
    $.ajax({
        type: 'get',
        url: window.location.protocol + '//' + window.location.host + '/rest/barcode/title',
        dataType: 'json',
        contentType: 'application/json; charset=utf-8',
        success: function (data, textStatus, jqXHR) {
            var response = $.parseJSON(data);

            displayTitle(response);
        },
        error: function(jqXHR, textStatus, errorThrown) {
            var response = $.parseJSON(data);

            displayTitle({Title:"Error", Station:"Error"});
        }
    });
}

