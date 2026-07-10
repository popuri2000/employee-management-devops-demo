// Shared site-wide helpers: CSRF-protected AJAX and toast notifications.

function getAntiForgeryToken() {
    return $('#antiForgeryForm input[name="__RequestVerificationToken"]').val();
}

$(function () {
    var token = getAntiForgeryToken();
    if (token) {
        $.ajaxSetup({
            beforeSend: function (xhr, settings) {
                var method = (settings.type || 'GET').toUpperCase();
                if (method !== 'GET' && method !== 'HEAD') {
                    xhr.setRequestHeader('X-CSRF-TOKEN', token);
                }
            }
        });
    }
});

function showToast(message, variant) {
    variant = variant || 'success';
    var toastId = 'toast-' + Date.now();
    var toastHtml =
        '<div id="' + toastId + '" class="toast align-items-center text-bg-' + variant + ' border-0" role="alert" aria-live="assertive" aria-atomic="true">' +
        '  <div class="d-flex">' +
        '    <div class="toast-body">' + $('<div>').text(message).html() + '</div>' +
        '    <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>' +
        '  </div>' +
        '</div>';

    var $toast = $(toastHtml);
    $('#toastContainer').append($toast);
    var toast = new bootstrap.Toast($toast[0], { delay: 4000 });
    toast.show();
    $toast.on('hidden.bs.toast', function () { $(this).remove(); });
}
