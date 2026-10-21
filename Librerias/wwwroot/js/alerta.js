


$('#btnAlertaOkCerrar').on('click', function () {
     
    $("#alertaOk").hide();
             
});

function MostrarAlertaOk(texto) {
        
    $('#tituloAlerta').html(texto)
    
    $('#alertaOk').removeClass("alert-warning");
    $('#alertaOk').addClass("alert-success");
    $('#alertaOk').show()
}
function MostrarAlertaWarning(texto) {

    $('#tituloAlerta').html(texto)
    $('#alertaOk').removeClass("alert-success");
    $('#alertaOk').addClass("alert-warning");
    $('#alertaOk').show()
}