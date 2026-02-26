async function search_req(type){
    await Get_search(type);
}

async function Get_search(type){
    const keyword = "Pokemon";
    const result_box = document.querySelector(".result-box");

    const params = new URLSearchParams({        // make para for url
        keyword: keyword,
        type: type
    });
    const url = `/Search/GetData?${params.toString()}`;

    try{
        const response = await fetch(url);     // using get http method to recieve data
        if (!response.ok){          // error when not ok
            throw new Error('Get not found');
        } 

        const data = await response.json();         // decode
        
        let html_data = '<div class="list-group">';
        data.forEach(item =>{               // add data เรียงตัว
            html_data += `
            <div class="list-group-item">
                <strong>${item.title}</strong><br>
                <small>Date: ${item.date} | Place: ${item.location}</small>      
            <hr>
            `;
        });
        html_data += '</div>';

        result_box.innerHTML = html_data;           // insert data in element box
        
    } catch (error) {
        console.error('Error:', error);
        resultArea.innerHTML = '<p class="text-danger">Error when fetch data</p>';
    }
}