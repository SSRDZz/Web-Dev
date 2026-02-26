let timeout = null;
document.getElementById("search-bar").addEventListener("keyup", async () => {
	clearTimeout(timeout);
	timeout = setTimeout(async () => {
		let value = document.getElementById("search-bar").value;

		await fetch(`/Search/SearchByUsername?Username=${value}`)
		.then(response => response.text())
		.then(html => { document.getElementById("activity-grid").innerHTML = html; })
		.catch(err => console.warn("Something went wrong.", err));
	}, 800);
});