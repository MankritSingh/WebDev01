
        function submitForm() {
            const form = document.getElementById("myForm");
            const username = form.elements.name.value;
            const password = form.elements.password.value;

            // Base64 encode your username and password for basic authentication
            const credentials = window.btoa(username + ':' + password);

            // Define the URL to send the request to
            const url = 'http://localhost:8080/';
			const request = new XMLHttpRequest();
			// Set up the request
            request.open("POST", url, true);

            // Set the Authorization header for basic authentication
            request.setRequestHeader('Authorization', 'Basic ' + credentials);

            // Define the request payload (data to send)
            const data = JSON.stringify({ password, username });

            // Set the request headers
            request.setRequestHeader('Content-Type', 'application/json');

            // Send the request
            request.send(data);
			// Prepare the request headers, including the Authorization header for basic authentication
           // const headers = new Headers({
            //    'Authorization': 'Basic ' + credentials
           // });
		    //const headers = new Headers();
			//headers.append('Authorization', 'Basic ' + btoa('username:password'));

            // Create the request object
            //const request = new Request(url, {
            //    method: 'POST', // Adjust the HTTP method as needed
           //     headers: headers,
           //     body: JSON.stringify({ username, password}) // Modify the data format as needed
           // });

            // Send the request
           // fetch(request)
           //     .then(response => {
            //        // Handle the response from the server
           //         if (response.ok) {
           //             // Request was successful
           //             // You can handle the response data here
            //        } else {
            //            // Request had an error, handle the error here
            //        }
            //    })
             //   .catch(error => {
            //        // Handle any network or request errors here
            //    });
        }
