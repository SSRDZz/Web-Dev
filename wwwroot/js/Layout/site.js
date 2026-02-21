// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function addText(txt) {
    const div = document.createElement("div");
    div.innerText = txt;
    const body = document.getElementsByTagName("body");
    body[0].appendChild(div);
}

function loggedin() {
    addText("You are logged");
}