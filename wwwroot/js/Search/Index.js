async function search_req(type){
    await Get_search(type);
}


// API http get request using ajax
async function fetch_data(type = "All"){
    // const urlParameter = new URLSearchParams(window.location.search);
    // const keyword = urlParameter.get('keyword') || "";
    const keyword = document.querySelector(".search-input").value; // use this because when user input in search bar but not enter -> then press button -> so it will got value from both button and search bar
    // const type = urlParameter.get('type') || "All";


    const url = `/Search/GetData?keyword=${keyword}&type=${type}`;
    console.log(url);
    
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

}

// for update html with new result
function renderResult(data){
    document.querySelector(".search-topic").innerText = data.message;

    const result_box = document.querySelector(".result-box");
    console.log("Render something finished == nothing");

    let html_data = '<div class="list-group">';
    data.activity.forEach(item =>{               // add data เรียงตัว
        html_data += `
        <div class="list-group-item">
            <strong>${item.title}</strong><br>
            <small>Date: ${item.date} | Place: ${item.location}</small>      
        <hr>
        `;
    });
    html_data += '</div>';

    result_box.innerHTML = html_data;           // insert data in element box
        
}

document.querySelectorAll(".type").forEach(button => {
    button.addEventListener('click',function(){
        const type = button.getAttribute("name");
        console.log(type)
        fetch_data(type);
    })

})

window.addEventListener('load', function(){
    fetch_data();
});