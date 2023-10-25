

	let options={
	method:"POST",
	headers:{
		"Content-type":"application/json",
		"Authorization":"Basic TWFuOlBhc3Mx"
	},
	body:JSON.stringify({
		title:'Harry',
		body:'bhai',
		userId:1100		
	}),	
	}
	let p=fetch('http://localhost:8080/',options)

