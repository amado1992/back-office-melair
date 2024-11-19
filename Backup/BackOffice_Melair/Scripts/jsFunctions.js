function setFormats() {
    $(".flightDate").datepicker();
    $(".flightDate").datepicker("option", $.datepicker.regional["pt"]);

    $(".flightHour").timepicker();
}

function showImagesPopup() {
    document.getElementById("fade").style = "display: block;";
    document.getElementById("imagesPopup").style = "display: block;";
}

function showPopup() {
    $('#popup').modal('show');
}

function showConfirm() {
    $('#confirm').modal('show');
}