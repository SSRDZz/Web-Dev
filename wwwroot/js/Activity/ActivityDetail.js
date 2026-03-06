const search_bar = document.getElementById("create-comment")
search_bar.addEventListener("keypress", async (e) => {
	if(e.key != "Enter")
		return ;
	const content = e.currentTarget.value;
	const act_id  = e.currentTarget.getAttribute("act-id");
	const response = await fetch(`/Comment/CreateComment`, {
		method: "POST",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify({ ActivityId: act_id, Content: content})
	});

	console.log(response);

	if(!response.ok) 
		return ;

	const html = await response.text();

	document.getElementById("comment-section").innerHTML = html;

	search_bar.value = "";
});