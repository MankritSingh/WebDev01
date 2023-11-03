function submitForm(){
	const url = 'http://localhost:8080/';
	const form = document.getElementById("myForm");
    const username = form.elements.name.value;
    const password = form.elements.password.value;
	// Base64 encode your username and password for basic authentication
    const credentials = window.btoa(username + ':' + password);
	const AuthHeaderContent="Basic "+credentials;
	
	let options={
	method:"POST",
	headers:{
		"Content-type":"application/json",
		"Authorization":AuthHeaderContent
	},
	body:JSON.stringify({
		title:'Login Test',
		body:'Test:Body',
		userId:1100		
	}),	
	}
	fetch(url, options)
    .then(response => {
        if (response.ok) {
            // If the response status is in the 200-299 range, it's a successful response
            return response.text(); // Parse the JSON response
        } else {
            throw new Error(`HTTP error! Status: ${response.status}`);
        }
    })
    .then(data => {
        // Process the parsed JSON data from the response
        console.log("Response data:", data);
        // You can do further processing here
    })
    .catch(error => {
        // Handle errors, such as network issues or non-2xx HTTP responses
        console.error("Error:", error);
		setTimeout(10000);
    })
}



