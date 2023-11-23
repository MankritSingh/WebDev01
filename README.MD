Basic Authentication is one of the simplest methods of authentication. In this method, the request includes the username and password added in an Authorization Header that is part of the request.

The project is implemented in C# and is a console application. Currently, there is only a backend, and it has to be used with Postman or a similar client.

To add a user, simply append the credentials to the "db.txt"(present in the Validation folder) file in the format: username:password. The password is expected not to contain ":".

To perform Basic Auth, run the application and initiate a POST request on http://localhost:8080/ with a Basic Authentication Header. The database file currently has the following entry: username=admin password=admin123.