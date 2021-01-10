# DacTools

DacTools is a tool that can be used to deploy DacPac files to Microsoft SQL Server. This tool expands upon DacFx by providing the ability to run multiple deployments to multiple databases simultaniously in parallel.

It was originally designed for use in Multi-Tenant scenarios where you may have a database per Customer (or Tenant) but you want to use a sqlproj to keep the schema of those databases in line.

|                                        | Stable (master)                                                                | Preview & Beta (develop)                                                       |
| :------------------------------------: | :----------------------------------------------------------------------------: | :----------------------------------------------------------------------------: |
|                               **Docs** | [![Docs][docs-badge]][docs]                                                    | [![Docs][docs-pre-badge]][docs-pre]                                            |
|                     **GitHub Release** | [![GitHub Release][gh-rel-badge]][gh-rel]                                      |                                                                                |
|                              **Build** | [![Build Status master][az-pipeline-master-badge]][az-pipeline-master]         | [![Build Status develop][az-pipeline-develop-badge]][az-pipeline-develop]      |
|                           **Coverage** | [![Codecov master][codecov-master-badge]][codecov-master]                      | [![Codecov develop][codecov-develop-badge]][codecov-develop]                   |
|           **DacTools.Deployment.Tool** | [![NuGet][dtdt-badge]][dtdt]                                                   | [![NuGet][dtdt-pre-badge]][dtdt-pre]                                           |
|           **DacTools.Deployment.Core** | [![NuGet][dtdc-badge]][dtdc]                                                   | [![NuGet][dtdc-pre-badge]][dtdc-pre]                                           |

## Compatibility

DacTools.Deployment works on Windows, Linux and Mac. It is supported on .NET 5, .NET Core 3.1, .NET Core 2.1 and .NET Framework 4.7.2+.

DacTools.Deployment currently makes use of Microsoft.SqlServer.DACFx v150.x and as such it can target SQL Server 2008, 2008R2, 2012, 2014, 2016, 2017, 2019 and Microsoft Azure SQL Databases.

## Quick Links

- [Documentation⇗][docs]
- [Configuration⇗][configuration]
- [Changelog⇗](./CHANGELOG.md)

# License

Licensed under [MIT](./LICENSE)

Copyright (c) 2021 DrBarnabus

**Notice**: This isn't an official Microsoft repository or tool, this is an open source project designed to assist in deploying dacpac Data-teir Applications.

[docs]:                         https://dactools.readthedocs.io/en/stable/?badge=stable
[docs-badge]:                   https://readthedocs.org/projects/dactools/badge/?version=stable
[docs-pre]:                     https://dactools.readthedocs.io/en/develop/?badge=develop
[docs-pre-badge]:               https://readthedocs.org/projects/dactools/badge/?version=develop
[gh-rel]:                       https://github.com/DrBarnabus/DacTools/releases/latest
[gh-rel-badge]:                 https://img.shields.io/github/release/DrBarnabus/DacTools.svg
[az-pipeline-master-badge]:     https://dev.azure.com/DrBarnabus/DacTools/_apis/build/status/DrBarnabus.DacTools?branchName=master
[az-pipeline-master]:           https://dev.azure.com/DrBarnabus/DacTools/_build/latest?definitionId=5&branchName=master
[az-pipeline-develop-badge]:    https://dev.azure.com/DrBarnabus/DacTools/_apis/build/status/DrBarnabus.DacTools?branchName=develop
[az-pipeline-develop]:          https://dev.azure.com/DrBarnabus/DacTools/_build/latest?definitionId=5&branchName=develop
[codecov-master-badge]:         https://codecov.io/gh/DrBarnabus/DacTools/branch/master/graph/badge.svg
[codecov-master]:               https://codecov.io/gh/DrBarnabus/DacTools/branch/master
[codecov-develop-badge]:        https://codecov.io/gh/DrBarnabus/DacTools/branch/develop/graph/badge.svg
[codecov-develop]:              https://codecov.io/gh/DrBarnabus/DacTools/branch/develop
[dtdt]:                         https://www.nuget.org/packages/DacTools.Deployment.Tool
[dtdt-badge]:                   https://img.shields.io/nuget/v/DacTools.Deployment.Tool
[dtdt-pre]:                     https://www.nuget.org/packages/DacTools.Deployment.Tool/absoluteLatest
[dtdt-pre-badge]:               https://img.shields.io/nuget/vpre/DacTools.Deployment.Tool
[dtdc]:                         https://www.nuget.org/packages/DacTools.Deployment.Core
[dtdc-badge]:                   https://img.shields.io/nuget/v/DacTools.Deployment.Core
[dtdc-pre]:                     https://www.nuget.org/packages/DacTools.Deployment.Core/absoluteLatest
[dtdc-pre-badge]:               https://img.shields.io/nuget/vpre/DacTools.Deployment.Core
[configuration]:                https://drbarnabus-dactools.readthedocs-hosted.com/en/stable/configuration
