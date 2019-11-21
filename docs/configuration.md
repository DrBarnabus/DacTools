# Configuration

DacTools is mainly configured using a set of command-line arguments when running the application.

## Arguments

All of the specified arguments can be specified with either a `/` or a `-` depending on your prefered argument prefix.

### /version

Displays the current version of the application.

### /help, /h or /?

Shows the Help Text for the application. This can also be accessed by using a single question mark as the first and only argument.

### /verbosity or /v

Sets the Verbosity Level on the logging. The value can be one of; Debug, Info, Warn, Error or None. The default value if not set is Info.

### /dacpac or /d

The path to the .dacpac file to deploy.

### /masterconnectionstring or /S

The Microsoft SQL Server Connection String for the master database on the server where the databases to deploy to are. This has to be a connection string to the built-in system master database for this application to work correctly and detect all databases.

### /databases or /D

A list of databases to either whitelist (default) or blacklist when generating the list of databases to deploy to. The list can either be provided as a set of comma separated values or space separated values.

### /blacklist or /b

Configures the application to use the value of the '/databases' or '/D' option as a blacklist instead of the default whitelist functionality.

### /threads or /t

Configures the maximum number of threads to use while deploying the dacpac to the databases. Deployments will be run in parallel up till the specified number of threads concurrently. If set to -1 then the value of `Environment.ProcessorCount` will be used instead. The default value if not set is 1.
