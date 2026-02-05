# Bank node
## Authors: Zdeněk Relich, Anna Marie Mičková

## App launch
Refer to [README on our repository](https://github.com/annamickova/P2P?tab=readme-ov-file#bank-node)

## Reused code
Relich
- [Singleton](https://github.com/Boootyshaker9000/D1-library-database/blob/main/src/conn/DatabaseConnector.java)
- [DAO](https://github.com/Boootyshaker9000/D1-library-database/tree/main/src/dao)
- [file reading](https://github.com/Boootyshaker9000/dictionary-attack/tree/main/FileManagers)
- [thread-safety](https://github.com/Boootyshaker9000/dictionary-attack/blob/main/Services/PasswordCrackerService.cs)

## Sources
Relich
- [naming conventions](https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names)
- [Lazy initialization](https://learn.microsoft.com/en-us/dotnet/api/System.Lazy-1?view=net-8.0)
- [Command pattern](https://en.wikipedia.org/wiki/Command_pattern)
- [Singleton pattern](https://cs.wikipedia.org/wiki/Singleton)
- [Strategy pattern](https://en.wikipedia.org/wiki/Strategy_pattern)


## What Our App Does

Our Bank Node is a P2P (peer-to-peer) application that simulates a banking system.

### Main Features
- create and delete bank accounts
- check account balance
- deposit and withdraw money
- see total money in the system
- monitor the server with a web dashboard
- shut down the application


## How the App Works

### 1. Commands
When a client connects, they can send these commands:
- **AC** - create new account
- **AB** - check balance
- **AD** - deposit money
- **AW** - withdraw money
- **AR** - delete account (only if empty)
- **BA** - total money in system
- **BN** - number of accounts
- **BC** - get node IP address


### 2. Storage
The app tries to save data in this order:
1. **MySQL database** (preferred)
2. **JSON file** (if database fails)
3. **Memory** (if everything fails - data is lost on restart)

This design ensures the app always works, even without a database.

### 3. Monitoring Dashboard
Web dashboard at `http://localhost:8080` that shows:
- number of commands executed
- number of errors
- how long the server has been running
- last command and last error
- server status (online/offline)
- buttons to refresh data or shut down

The dashboard updates automatically every 5 seconds.
