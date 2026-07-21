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