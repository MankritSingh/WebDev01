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
		title:'Harry',
		body:'bhai',
		userId:1100		
	}),	
	}
	let p=fetch('http://localhost:8080/',options);
}



