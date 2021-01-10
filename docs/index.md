# DacTools Docs
DacTools is a tool that can be used to deploy DacPac files to Microsoft SQL Server. This tool expands upon DacFx by providing the ability to run multiple deployments to multiple databases simultaniously in parallel.

It was originally designed for use in Multi-Tenant scenarios where you may have a database per Customer (or Tenant) but you want to use a sqlproj to keep the schema of those databases in line.

## Compatibility

DacTools works on Windows, Linux and Mac. It is supported on .NET 5, .NET Core 3.1, .NET Core 2.1 and .NET Framework 4.7.2+.

DacTools currently makes use of Microsoft.SqlServer.DacFx v150.x and as such supports the same Microsoft SQL Server versions supported by that package. Currently that includes; Microsoft SQL Server 2008, 2008R2, 2012, 2014, 2016, 2017, 2019 and Microsoft Azure SQL Databases.

## Configuration

DacTools has a number of configuration options to allow you to customize it to your workflow.

Read more about the configuration options [here⇗](configuration.md)

## Changelog

The project uses conventional commits as well as standard-version to automatically generate a changelog. The changelog is available in the repository.
