// API http get request using ajax
async function fetch_data(type = "All"){

    const keyword = document.querySelector(".search-input").value; // use this because when user input in search bar but not enter -> then press button -> so it will got value from both button and search bar
   
    const url = `/Search/GetData?keyword=${keyword}&type=${type}`;
    // console.log(url);
    
    try{
        const response = await fetch(url);           // using get http method to recieve data
        if(!response.ok){
            throw new Error('Get not found');
        }

        const data = await response.json();

        renderResult(data)

    }   catch (error) {
        console.error('Fetch error:', error);
    }

    const newurl = `${window.location.pathname}?keyword=${keyword}&type=${type}`;
    window.history.pushState({path: newurl }, '', newurl);          // for history browser (not necessary)
}


// for update html with new result
async function renderResult(data){  
    const keyword_result = data.message.keyword == "" 
    ? "All Activity"
    : data.message.keyword;

    document.querySelector(".search-topic").innerHTML = `
        Showing results for <span class="highlight">${keyword_result}</span> 
        in <span class="type-tag">${data.message.type}</span>
    `;
    const result_box = document.querySelector(".result-box");

    let html_data = '<div class="list-group">';

     // add user
    // console.log(data,"==================\n", data.Result ,"==================\n", data.Result_User)

    const mergeJson = [...data.result_User,...data.result_Activity];
    mergeJson.sort((a,b) =>  {
        const nameA = a.name.toUpperCase(); // ignore upper and lowercase
        const nameB = b.name.toUpperCase(); 

        if (nameA < nameB) {
            return -1; // A
        }
        if (nameA > nameB) {
            return 1; // B
        }
        return 0; 
    });

    for (const item of mergeJson) {
        // console.log(item);
        if("location" in item){         // this mean it activity
            // console.log("it activity");
            html_data += `
            <div class="list-group-item" id="activity-item">
                <a href="/Activity/Detail/${item.id}">
                    <h3>${item.name}</h3>
                    
                    <small>Date: ${item.date} | Place: ${item.location}</small>      
                </a>
            </div>
            <hr>
            `;
        }
        else{
            // console.log("It user");
            let rep = 0 ;
            try {
                const url = `/User/FindReputation?TargetID=${item.id}`;
                rep = await fetch(url);
                rep = await rep.text();

            } catch (error){
                console.log("error when get rep",error);
            }

            // console.log(rep);
            html_data += `
            <div class="list-group-item" id="user-item" userid="${item.id}" >
                <a href="/User/ProfileOther/${item.id}">
                    <img src="${item.image}" />
                    <div class="detail">
                        <h3 class="search-tag-detail">${item.name}</h3>
                        <small> ${rep} Like</small>
                    </div>
                </a>
            </div>
            <hr>
        `;
        }
    };

    html_data += '</div>';

    result_box.innerHTML = html_data;           // insert data in element box
        
}


// show what page are user in -> by check what button should have .active class
function active_button(active_button){
    const button_page = document.querySelectorAll(".Header-type button");
    // console.log(button_page);
    button_page.forEach(function(button){          
        button.className = "type";     // make it all default
    })
    // console.log(active_button);
    active_button.className = "type active";  // make only that click button active
}


document.querySelectorAll(".type").forEach(button => {
    button.addEventListener('click',function(){
        const type = button.getAttribute("name");
        active_button(button);
        // console.log(button);
        fetch_data(type);
    })

})

window.addEventListener('load', function(){
    fetch_data();
});