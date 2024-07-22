// dropdown
let arrow = document.querySelectorAll(".arrow");
for (var i = 0; i < arrow.length; i++) {
    arrow[i].addEventListener("click", (e) => {
        let arrowParent = e.target.parentElement.parentElement;
        console.log(arrowParent);
        arrowParent.classList.toggle("showMenu");
    });
}

document.querySelectorAll(".arrow").forEach(arrow => {
    arrow.addEventListener("click", (e) => {
        let arrowParent = e.target.closest(".navbar-item");
        arrowParent.classList.toggle("showMenu");
    });
});

function toggleMenuDropdown(event) {
    event.stopPropagation();
    var dropdown = document.getElementById("menuDropdown");
    dropdown.style.display = (dropdown.style.display === "block") ? "none" : "block";
    event.target.closest(".navbar-item").classList.toggle("showMenu");
}

function toggleUserDropdown(event) {
    event.stopPropagation();
    var dropdown = document.getElementById("userDropdown");
    dropdown.style.display = (dropdown.style.display === "block") ? "none" : "block";
    event.target.closest(".navbar-item").classList.toggle("showMenu");
}

window.onclick = function (event) {
    if (!event.target.closest('.btnAdmin')) {
        var dropdowns = document.getElementsByClassName("dropdown-content");
        for (var i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.style.display === "block") {
                openDropdown.style.display = "none";
                openDropdown.closest(".navbar-item").classList.remove("showMenu");
            }
        }
    }
    if (!event.target.closest('.icon-link')) {
        var dropdowns = document.getElementsByClassName("sub-menu");
        for (var i = 0; i < dropdowns.length; i++) {
            var openDropdown = dropdowns[i];
            if (openDropdown.style.display === "block") {
                openDropdown.style.display = "none";
                openDropdown.closest(".navbar-item").classList.remove("showMenu");
            }
        }
    }
}