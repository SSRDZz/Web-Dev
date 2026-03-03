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
function renderResult(data){                            
    document.querySelector(".search-topic").innerText = data.message;
    const result_box = document.querySelector(".result-box");

    let html_data = '<div class="list-group">';

     // add user
    console.log(data,"==================\n", data.Result ,"==================\n", data.Result_User)
    data.result_User.forEach(item =>{                  
        html_data += `
            <div class="list-group-item" userid="${item.id}" >
                <a href="/User/ProfileOther/${item.id}">
                    <h2 class="search-tag-detail">${item.name}</h2>
                    <img src="${item.image}" style="width:30px;"/>
                </a>
            </div>
        `;
    });
    
    // add activity
    data.result_Activity.forEach(item =>{               
        html_data += `
        <div class="list-group-item">
            <strong>${item.title}</strong><br>
            <small>Date: ${item.date} | Place: ${item.location}</small>      
        </div><hr>
        `;
    });

    html_data += '</div>';

    result_box.innerHTML = html_data;           // insert data in element box
        
}

document.querySelectorAll(".type").forEach(button => {
    button.addEventListener('click',function(){
        const type = button.getAttribute("name");
        fetch_data(type);
    })

})

window.addEventListener('load', function(){
    fetch_data();
});