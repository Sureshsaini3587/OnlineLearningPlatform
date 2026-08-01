document.addEventListener("DOMContentLoaded", function () {
    const navbar = document.getElementById("appNavbar"); 
    function toggleNavbarState() {
        if (!navbar) return;

        if (window.scrollY > 20) {
            navbar.classList.add("scrolled");
        } else {
            navbar.classList.remove("scrolled");
        }
    }

    toggleNavbarState();
    window.addEventListener("scroll", toggleNavbarState, { passive: true });
});

function openGlobalModal(title, url) {
    const modalEl = document.getElementById('globalModal');
    const modalTitle = document.getElementById('globalModalLabel');
    const modalBody = document.getElementById('globalModalBody');
     
    modalTitle.innerText = title;
     
    modalBody.innerHTML = `
        <div class="text-center py-5">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>`;
         
    const myModal = new bootstrap.Modal(modalEl);
    myModal.show();
     
    $.get(url, function (data) {
        modalBody.innerHTML = data;
    })
    .fail(function () {
        modalBody.innerHTML = `
        <div class="alert alert-danger m-3">
            <i class="bi bi-exclamation-triangle-fill"></i> Unable to load content. Please try again.
        </div>`;
    });
}
function showConfirm(title, message, onConfirm) {
    const modalEl = document.getElementById('confirmModal');
    const modal = new bootstrap.Modal(modalEl);
     
    document.getElementById('confirmTitle').innerText = title;
    document.getElementById('confirmMessage').innerText = message;
     
    document.getElementById('confirmYesBtn').onclick = function () {
        onConfirm();  
        modal.hide();
    };

    modal.show();
}
function showNotification(title, message, type) {
     
    let typeClass = 'cine-alert-info';
    let iconClass = 'bi-info-circle-fill';

    if (type === 'success') {
        typeClass = 'cine-alert-success';
        iconClass = 'bi-check-circle-fill';
    } else if (type === 'error') {
        typeClass = 'cine-alert-error';
        iconClass = 'bi-exclamation-circle-fill';
    }
     
    const alertHtml = `
    <div class="cine-alert ${typeClass} alert-dismissible fade show" role="alert">
        <div class="cine-alert-icon"> 
            <i class="bi ${iconClass}"></i>
        </div>
        <div class="cine-alert-content">
            <h4 class="cine-alert-title">✦ ${title}</h4>
            <p class="cine-alert-text">${message}</p>
        </div>
        <button type="button" class="btn-close btn-close-white cine-close-btn" data-bs-dismiss="alert" aria-label="Close"></button>
    </div>`;
     
    $('.cine-alert-wrapper').append(alertHtml);
     
    setTimeout(function () {
        $('.cine-alert-wrapper .cine-alert').first().fadeOut(500, function () {
            $(this).remove();
        });
    }, 5000);
}