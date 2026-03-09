const comment_bar = document.getElementById("create-comment");
if (comment_bar) {
	comment_bar.addEventListener("keypress", (e) => {
		if (e.key != "Enter")
			return;
		update_comment(e)
	});
}

// for submit comment and update comment section 
async function update_comment(e){
	const content = e.currentTarget.value;
	// if user don't write any comment and press enter || click icon submit
	if(content.length < 1){
		return 
	}
	const act_id = e.currentTarget.getAttribute("act-id");
	const response = await fetch(`/Comment/CreateComment`, {
		method: "POST",
		headers: { "Content-Type": "application/json" },
		body: JSON.stringify({ ActivityId: act_id, Content: content })
	});

	if (!response.ok)
		return;

	const html = await response.text();
	document.getElementById("comment-section").innerHTML = html;
	comment_bar.value = "";
}


function parseLatLngFromMapUrl(raw) {
	if (!raw) return null;
	const value = raw.trim();

	const direct = value.match(/^(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)$/);
	if (direct) {
		return { lat: parseFloat(direct[1]), lng: parseFloat(direct[2]), zoom: 15 };
	}

	try {
		const url = new URL(value);

		const q = url.searchParams.get("q") || url.searchParams.get("query") || url.searchParams.get("ll");
		if (q) {
			const qMatch = q.match(/(-?\d+(?:\.\d+)?)\s*,\s*(-?\d+(?:\.\d+)?)/);
			if (qMatch) {
				return { lat: parseFloat(qMatch[1]), lng: parseFloat(qMatch[2]), zoom: 15 };
			}
		}

		const atMatch = url.pathname.match(/@(-?\d+(?:\.\d+)?),(-?\d+(?:\.\d+)?),([\d.]+)z/);
		if (atMatch) {
			return { lat: parseFloat(atMatch[1]), lng: parseFloat(atMatch[2]), zoom: Math.round(parseFloat(atMatch[3])) };
		}

		const hashMatch = url.hash.match(/map=(\d+)\/(-?\d+(?:\.\d+))\/(-?\d+(?:\.\d+))/);
		if (hashMatch) {
			return { lat: parseFloat(hashMatch[2]), lng: parseFloat(hashMatch[3]), zoom: parseInt(hashMatch[1], 10) };
		}
	} catch {
		return null;
	}

	return null;
}

function initLeafletMap() {
	const mapEl = document.getElementById("activity-map");
	if (!mapEl) return;

	if (typeof L === "undefined") {
		console.error("Leaflet library not loaded");
		return;
	}

	const parsed = parseLatLngFromMapUrl(mapEl.getAttribute("data-map-url"));
	if (!parsed) {
		console.log("Could not parse map URL, hiding map");
		mapEl.style.display = "none";
		return;
	}

	try {
		const map = L.map("activity-map").setView([parsed.lat, parsed.lng], parsed.zoom || 15);
		L.tileLayer("https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png", {
			maxZoom: 19,
			attribution: '&copy; <a href="https://www.openstreetmap.org/">OpenStreetMap</a> contributors'
		}).addTo(map);

		L.marker([parsed.lat, parsed.lng]).addTo(map);
		console.log("Map initialized successfully at:", parsed);
	} catch (error) {
		console.error("Error initializing map:", error);
		mapEl.style.display = "none";
	}
}

// Wait for DOM and Leaflet to be ready
if (document.readyState === 'loading') {
	document.addEventListener('DOMContentLoaded', initLeafletMap);
} else {
	initLeafletMap();
}

function comment_button(){
    const icon = document.querySelector("#comment-button");
    const comment_bar = document.querySelector("#create-comment");
    icon.addEventListener('click',function(){
        update_comment({ currentTarget: comment_bar })
    });
}
comment_button()