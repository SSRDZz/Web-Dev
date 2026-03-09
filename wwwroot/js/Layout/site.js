// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.


function toggleSidebar() {
    const sidebar = document.getElementById('sidebar');
    sidebar.classList.toggle('show');
}

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
    })
    
}

function submit_searchBar(){
    const icon = document.querySelector("#submit-button");
    const form = document.querySelector(".search-container");
    icon.addEventListener('click',function(){
        form.submit();
    });
}

// suggestion declare
const search_input = document.querySelector(".search-input");
const suggestion_box = document.querySelector(".suggestion-box");

// suggestion for input search box
function suggestion(){
    let timeout = 0;
    search_input.addEventListener("input", function(){
        clearTimeout(timeout);
        timeout = setTimeout(fetch_suggestion,300);
    });
}

// query data from back-end using fetch api
async function fetch_suggestion(){
    const keyword = search_input.value.trim();

    if (keyword.length >= 1){
        const url = `/Home/GetSuggestion?keyword=${keyword}`;
        try{
            const response = await fetch(url);
            if(!response.ok){
                throw new Error("Get not found");
            }

            const data = await response.json();
            renderSuggestion(data);

        } catch(error) {
            console.log("error when get suggestion",error);
        }
    }
    else {
        suggestion_box.style.display = "none";
    }
}

// render result suggestion
function renderSuggestion(data){
    if(data.length == 0){
        suggestion_box.style.display = "none";
        return;
    }

    suggestion_box.innerHTML = data.map(item => `<li class="suggestion-item">${item}</li>`).join('');
    suggestion_box.style.display = 'block';

    // make when user click suggestion -> change search-input value
    const suggestion_item = document.querySelectorAll(".suggestion-item");

    suggestion_item.forEach(function(item){
        item.addEventListener("click", function(){
            search_input.value = this.textContent;
            suggestion_box.style.display = 'none';
            search_input.focus();
        })   
    });
}


// when click other component that not suggestion box 
document.addEventListener('click', (e) => {
    const suggestion_box = document.querySelector(".suggestion-box");
    const search_input = document.querySelector(".search-input")

    if (!search_input.contains(e.target)) {
        // console.log("click at element -> ",e);
        suggestion_box.style.display = 'none';
    }
});

window.addEventListener('load', function(){
    active_page_button();
});

submit_searchBar();
suggestion();