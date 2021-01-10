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

## DacDeploymentOptions SqlCommandVariableValues

If you require a SqlCommandVariable to be set while deploying you can specify this by passing a command line parameter to the DacTools.Deployment tool.

Any SqlCommandVariable can be set by passing the name of the variable as well as the desired value via the command line. These values should be prefixed with `-variable:` or `/variable:`, for example to set the _CommandExample_ variable to a value of _ExampleValue_ you would add the following argument to the command line: `/variable:CommandExample ExampleValue`.

## DacDeploymentOptions Arguments

A number of parameters are supported to customize the DacDeploymentOptions that are passed into DacServices by the DacTools.Deployment tool.

All of these parameters can be set by passing the name of the argument as well as the desired value/values via the command line. These parameters should be prefixed with either `-p:` or `/p:`, for example to set the BlockOnPossibleDataLoss option to true you would add the following argument to the command line: `-p:BlockOnPossibleDataLoss true`.

The members of [DacDeployOptions](https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.dac.dacdeployoptions) which are currently serviced are listed below along with their default values. If an option parameter is not currently serviced and isn't listed below, create a feature request or feel free to submit a pull request adding the option.

### BlockOnPossibleDataLoss

Defaults to false.

### DropIndexesNotInSource

Defaults to false.

### IgnorePermissions

Defaults to true.

### IgnoreRoleMembership

Defaults to true.

### GenerateSmartDefaults

Defaults to true.

### DropObjectsNotInSource

Defaults to true.

### DoNotDropObjectTypes

Accepts a list of values in the [ObjectType](https://docs.microsoft.com/en-us/dotnet/api/microsoft.sqlserver.dac.objecttype) enum. Defaults to; `ObjectType.Logins`, `ObjectType.Users`, `ObjectType.Permissions`, `ObjectType.RoleMembership`, `ObjectType.Filegroups`.

### CommandTimeout

This option **cannot** be set via the command line arguments. However, this is set to 0 for an infinate timeout by default.
