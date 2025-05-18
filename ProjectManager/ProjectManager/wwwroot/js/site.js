// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

$(document).ready(function () {
    setTimeout(function () {
        $(".fade-message").fadeOut("slow");
    }, 3000);
});


const sidebar = document.querySelector(".side_nav");
function toggleMenu() {
    sidebar.classList.toggle("close");
}

const sideNav = document.querySelector('.side_nav');

function togglePictureMenu() {
    const info = document.getElementById('account_info');
    info.classList.toggle('show');
}


function adjustSidebarHeight() {
    const footer = document.querySelector('footer');
    const footerRect = footer.getBoundingClientRect();
    const windowHeight = window.innerHeight;

    if (footerRect.top < windowHeight - 50) {
        // Footer is visible

        sideNav.style.height = "calc(100vh - 170px)";
    } else {
        // Footer not visible 
        sideNav.style.height = "100vh";
    }
}

window.addEventListener('scroll', adjustSidebarHeight);
window.addEventListener('resize', adjustSidebarHeight);

// Adjust immediately
adjustSidebarHeight();



