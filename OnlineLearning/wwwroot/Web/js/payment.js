let statusModalInstance = null;

$(document).ready(function () {
    const modalElement = document.getElementById('paymentStatusModal');
    if (modalElement) {
        statusModalInstance = new bootstrap.Modal(modalElement, {
            backdrop: 'static',
            keyboard: false
        });
    }
});

function startPayment(planId, courseId) { 
    const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val(); 
    $.ajax({
        url: '/Payment/CreateOrder',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            planId: parseInt(planId),
            courseId: parseInt(courseId)
        }),
        headers: { 
            'RequestVerificationToken': antiForgeryToken
        },
        success: function (data) {
            var options = {
                key: data.key,
                amount: data.amount,
                currency: "INR",
                name: "Manika Learning Academy",
                description: "Course Subscription",
                order_id: data.orderId,
                handler: function (response) {
                    verifyPayment(response, courseId, planId);
                },
                prefill: {
                    name: data.name,
                    email: data.email,
                    contact: data.phone
                },
                theme: { color: "#1c2541" } 
            };
            var rzp = new Razorpay(options);
            rzp.open();
        },
        error: function (xhr) {
            let errorMsg = "Failed to create order";
            try {
                let responseJson = JSON.parse(xhr.responseText);
                errorMsg = responseJson.message || errorMsg;
            } catch (e) { }

            if (typeof showNotification === 'function') {
                showNotification('Error', errorMsg, 'error');
            } else {
                alert(errorMsg);
            }
        }
    });
}

function verifyPayment(response, courseId, planId) {
    if (statusModalInstance) {
        statusModalInstance.show();
    }

    $("#paymentStatusBody").html(` 
         <div class="text-center py-4"> 
             <div class="spinner-border mb-4" role="status" style="color: #1c2541;"></div> 
             <h4 class="fw-bold">Verifying Payment...</h4> 
             <p class="text-muted">Please wait while we securely confirm your payment</p> 
         </div>
    `);

    const antiForgeryToken = $('input[name="__RequestVerificationToken"]').val();

    $.ajax({
        url: '/Payment/Success',
        type: 'POST',
        contentType: 'application/json; charset=utf-8',
        data: JSON.stringify({
            planId: parseInt(planId),
            courseId: parseInt(courseId),
            razorpayOrderId: response.razorpay_order_id,
            razorpayPaymentId: response.razorpay_payment_id,
            razorpaySignature: response.razorpay_signature
        }),
        headers: {
            'RequestVerificationToken': antiForgeryToken
        },
        success: function (data) {
            if (data.success) {
                $("#paymentStatusBody").html(` 
                  <div class="text-center py-3">
                      <div class="success-icon mb-3 fs-1 text-success">✅</div>
                      <h3 class="fw-bold text-success">Payment Successful</h3>
                      <p class="text-muted mb-4">Your course has been unlocked successfully.</p>
                      <button class="btn text-white px-5 py-2 rounded-pill fw-bold shadow-sm" style="background-color: #1c2541;" onclick="location.reload()">
                          Start Learning 🚀
                      </button>
                  </div>
                `);
            } else {
                $("#paymentStatusBody").html(` 
                   <div class="text-center py-3"> 
                       <div class="mb-3 fs-1 text-danger">❌</div>
                       <h3 class="fw-bold text-danger">Verification Failed</h3> 
                       <p class="text-muted mb-4">Payment could not be cryptographically verified.</p> 
                       <button class="btn btn-dark px-5 py-2 rounded-pill fw-semibold" onclick="location.reload()">
                           Try Again 
                       </button> 
                   </div>
                `);
            }
        },
        error: function () {
            $("#paymentStatusBody").html(` 
               <div class="text-center py-3">
                   <div class="mb-3 fs-1 text-warning">⚠️</div>
                   <h3 class="fw-bold">Something went wrong</h3>
                   <p class="text-muted mb-4">Server validation error occurred. Please try again later.</p>
                   <button class="btn btn-dark px-5 py-2 rounded-pill fw-semibold" onclick="location.reload()">
                       Reload
                   </button>
               </div>
           `);
        }
    });
}