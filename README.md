# Bank node
## Program's purpose
This application serves as a bank accounts storage. It can store data in a MySQL database, a JSON file or RAM.
## Requirements
- [.net8.0](https://www.oracle.com/java/technologies/downloads/#jdk21-windows)
- [MySQL 8.0.44](https://dev.mysql.com/downloads/installer/) 
  - The user whose credentials are filled in the *bin/Debug/net8.0/appsettings.json* file must have privileges for reading, writing, updating and deleting to use the app as intended.
## How to run program
**CREATE RELEASE**
### Configuration
The app loads credentials from *bin/Debug/net8.0/appsettings.json*. The content of the file may look something like this:
```json
{
  "Storage":{
    "PrefferedStrategy": "mysql"
  },
  "Server":{
    "IPAddress": "127.0.0.1",
    "Port": 65525,
    "Timeout": 10000
  },
  "Peers": {
    "PortStart": 65525,
    "PortEnd": 65535,
    "Timeout": 10000
  },
  "Database": {
    "Server": "localhost",
    "Port": 3306,
    "Database": "bank",
    "User": "root",
    "Password": ""
  }
}
```
Do not change the location of this config file, or it's name. If you do, the program will end with an error.
Do not change the JSON keys. If you do, the program will end with an error.
### Database setup
Use the *database_setup/generation-script.sql* to create the database and it's table on your MySQL server.
### Running the binary file
Double-click on the *bin/Debug/net8.0/P2P.exe*.
## Contact
If you have any questions regarding this project, or you'd like to contribute, do not hesitate to contact us.
- e-mail address: relich@post.cz
- Discord: boootyshaker9000, anicka.m
