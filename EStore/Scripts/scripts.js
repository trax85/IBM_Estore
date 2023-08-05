/*!
* Start Bootstrap - Shop Homepage v5.0.6 (https://startbootstrap.com/template/shop-homepage)
* Copyright 2013-2023 Start Bootstrap
* Licensed under MIT (https://github.com/StartBootstrap/startbootstrap-shop-homepage/blob/master/LICENSE)
*/
// This file is intentionally blank
// Use this file to add JavaScript to your project
function toggleDropdown(id) {
    var dropdown = document.getElementById("dropdown-menu" + id);
    dropdown.style.display = dropdown.style.display === "none" ? "block" : "none";
    console.log("dropdown-menu" + id);
    document.addEventListener("click", function (event) {
        var dropdown = document.getElementById("dropdown" + id);
        if (!dropdown.contains(event.target)) {
            var dropdownMenu = document.getElementById(dropdown);
            dropdownMenu.style.display = "none";
        }
    });
}

function onProductClick(argument) {
    window.location.href = '@Url.Action("Product", "Home")?productType=' + encodeURIComponent(argument);
}

function invokeAddCart(Name, Cost, Category) {
    var count = document.getElementById("addToCartInputBox").value;
    window.location.href = '@Url.Action("AddToCart", "Home")?name=' + encodeURIComponent(name)
        + "&cost=" + encodeURIComponent(Cost) + "&category=" + encodeURIComponent(Category)
        + "&quantity=" + encodeURIComponent(count);
}
