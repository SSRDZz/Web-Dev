// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    sidebar.classList.toggle('show');
};

// show what page are user in -> by check what button should have .active class
function active_page_button(){
    const button_page = document.querySelectorAll(".nav-item");
    const currentPath = window.location.pathname;
    button_page.forEach(function(button){          
        const targetPath = button.getAttribute("href");
        if (currentPath.includes(targetPath)) {
            button.className = "nav-item active";        // make only that click button active
        }
        else{
            button.className = "nav-item";     // make it all nav default
        }
        console.log(currentPath, targetPath, currentPath.includes(targetPath) )
    })
    
}

window.onload = active_page_button;







let timeout = null;
document.getElementById("search-bar").addEventListener("keyup", () => {
    clearTimeout(timeout);
    timeout = setTimeout(liveSearch, 800);
});

function liveSearch() {
    console.log("doing");
    let value = document.getElementById("search-bar").value;
    
    $.ajax({
        type: "POST",
        url: '@Url.Action(controller: "Search", action: "SearchByUsername")',
        data: { Username: value },
        datatype: "html",
        success: (data) => {
            $("#activity-grid").html(data);
        } 
    });
}