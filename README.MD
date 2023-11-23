Jwt Auth/Token Based Authentication
Token based auth tackles the scalabilty shortcoming of session auth.

*The project is built using Asp.net web api.We can test the application using postman or
see the Swagger/openapi page
To send a request using Postman
1)Set Request Type and URL:The path is simply the hostIP:port/api/[controllerName(Auth here)]/[method]
2)Set the Header:Add a new header with key Content-Type and value application/json
3)Set Request Body:raw option,{"username": "your_username","password": "your_password"}
4)Similary we can do the other requests

Database
1)To create the database,run the TableDesign.sql file.
2)In appsettings.json, we have the following Connection String 
string connectionStringUserInformation = @"Data Source = DESKTOP-8JBEJ91\SQLEXPRESS; Initial Catalog=UserInformation; Integrated Security=true";
Update it as follows connectionStringUserInformation = @"Data Source = [Server Name]; Initial Catalog=[Database Name]; Integrated Security=true";