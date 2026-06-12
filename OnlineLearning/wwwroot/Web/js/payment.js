let statusModalInstance = null;
 
document.addEventListener("DOMContentLoaded", function () {
    const modalElement = document.getElementById('paymentStatusModal');
    if (modalElement) {
        statusModalInstance = new bootstrap.Modal(modalElement, {
            backdrop: 'static',
            keyboard: false
        });
    }
});

function startPayment(planId, courseId) {
    fetch('/Payment/CreateOrder', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            planId: parseInt(planId),
            courseId: parseInt(courseId)
        })
    })
        .then(res => {
            if (!res.ok) { 
                return res.json().then(err => {
                    throw new Error(err.message || "Failed to create order");
                });
            }
            return res.json();
        })
        .then(data => {
            var options = {
                key: data.key,
                amount: data.amount,
                currency: "INR",
                name: "Online Learning",
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
                theme: { color: "#111827" }
            };
            var rzp = new Razorpay(options);
            rzp.open();
        })
        .catch(err => {
            alert(err.message);
        });
}

function verifyPayment(response, courseId, planId) {
    if (statusModalInstance) {
        statusModalInstance.show();
    }

    document.getElementById("paymentStatusBody").innerHTML = ` 
         <div class="text-center"> 
             <div class="spinner-border text-primary mb-4"></div> 
             <h4 class="fw-bold">Verifying Payment...</h4> 
             <p class="text-muted">Please wait while we confirm your payment</p> 
         </div>
    `;

    fetch('/Payment/Success', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
            planId: planId,
            courseId: courseId,
            razorpayOrderId: response.razorpay_order_id,
            razorpayPaymentId: response.razorpay_payment_id,
            razorpaySignature: response.razorpay_signature
        })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                document.getElementById("paymentStatusBody").innerHTML = ` 
              <div class="text-center">
                  <div class="success-icon mb-4 fs-1">✅</div>
                  <h3 class="fw-bold text-success">Payment Successful</h3>
                  <p class="text-muted mb-4">Your course has been unlocked successfully.</p>
                  <button class="btn btn-success px-5 py-3 rounded-pill" onclick="location.reload()">
                      Start Learning 🚀
                  </button>
              </div>
            `;
            } else {
                document.getElementById("paymentStatusBody").innerHTML = ` 
               <div class="text-center"> 
                   <div class="mb-4 fs-1">❌</div>
                   <h3 class="fw-bold text-danger">Verification Failed</h3> 
                   <p class="text-muted mb-4">Payment could not be verified.</p> 
                   <button class="btn btn-dark px-5 rounded-pill" onclick="location.reload()">
                       Try Again 
                   </button> 
               </div>
            `;
            }
        })
        .catch(() => {
            document.getElementById("paymentStatusBody").innerHTML = ` 
           <div class="text-center">
               <div class="mb-4 fs-1">⚠️</div>
               <h3 class="fw-bold">Something went wrong</h3>
               <p class="text-muted mb-4">Please try again later.</p>
               <button class="btn btn-dark px-5 rounded-pill" onclick="location.reload()">
                   Reload
               </button>
           </div>
       `;
        });
}