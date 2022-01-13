# Changelog

All notable changes to this project will be automatically documented in this file.


## [0.5.0](https://github.com/DrBarnabus/DacTools/compare/v0.4.0...v0.5.0) (2022-01-13)


### ⚠ BREAKING CHANGES

* **core:** upgrade to netstandard2.1
* **deployment:** remove support for dotnet framework runtime
* **deployment:** remove support for netcoreapp2.1

### Bug Fixes

* **deployment:** fix issue with database list generation and owner_sid ([66cde19](https://github.com/DrBarnabus/DacTools/commit/66cde19cda56da8f879e63321fa6bf259ca37a6b))


### build

* **core:** upgrade to netstandard2.1 ([21a214a](https://github.com/DrBarnabus/DacTools/commit/21a214a91cc86d3b4b969921b1d29363e02d6e0c))
* **deployment:** remove support for dotnet framework runtime ([4a9a8bb](https://github.com/DrBarnabus/DacTools/commit/4a9a8bb2abf8f33bb4dcde745e86618b0afffb89))
* **deployment:** remove support for netcoreapp2.1 ([3a5faed](https://github.com/DrBarnabus/DacTools/commit/3a5faed55a23ac4a2538b705c5dc69dc58d56a1c))

## [0.4.0](https://github.com/DrBarnabus/DacTools/compare/v0.3.0...v0.4.0) (2022-07-12)


### Features

* **deployment:** add command line argument to enable azpipelines integration ([bfbd76a](https://github.com/DrBarnabus/DacTools/commit/bfbd76ad5a445401aed3656147385060871f4a6f))
* **deployment:** add files object type to default do not drop list ([5940742](https://github.com/DrBarnabus/DacTools/commit/59407422d55b54e9d2d9e4a83402b22ecac83e69))
* **deployment:** add ignorefileandlogfilepath as a serviced dacdeployoptions value ([c111815](https://github.com/DrBarnabus/DacTools/commit/c11181544802d7e39a36d412d3d5f0b8bd3c3fc4))


### Bug Fixes

* **deployment:** azure pipelines task.complete messages now use correct format ([d1226be](https://github.com/DrBarnabus/DacTools/commit/d1226be9974639e2a0f147c3fdaffc3e8cee23b0))
* **deployment:** change to fix case sensitive command line arguments ([602dae2](https://github.com/DrBarnabus/DacTools/commit/602dae20965a9b20194e09fd844261ce58659ac9))
* **deployment:** fixes issue with active build server ([bad6570](https://github.com/DrBarnabus/DacTools/commit/bad65707dbe9f59059e2ebec8dd998b597616e6d))

## [0.3.0](https://github.com/DrBarnabus/DacTools/compare/v0.2.0...v0.3.0) (2022-01-10)


### ⚠ BREAKING CHANGES

* **deployment:** Removed netcoreapp3.0 as a TFM and replaced it with netcoreapp3.1, all native builds now target .NET Core 3.1.

Ensure you are using .NET Core 2.1 or preferably .NET Core 3.1 if you are currently using the .NET Core 3.0 build of the project.

### Features

* **deployment:** add command line argument parsing for sqlcmd variables ([6f805e8](https://github.com/DrBarnabus/DacTools/commit/6f805e8bcdbc5269fe5f8ad3f71f8883d7e84d2f))
* **deployment:** add file logging option ([755285c](https://github.com/DrBarnabus/DacTools/commit/755285c90a68d761c7464d1fefcc918070e1cff9)), closes [#2](https://github.com/DrBarnabus/DacTools/issues/2)
* **deployment:** add progress, warning and error build server messages ([4f20600](https://github.com/DrBarnabus/DacTools/commit/4f206001fde5eb9cf4e64cc58c94cdfb11ba08ee)), closes [#6](https://github.com/DrBarnabus/DacTools/issues/6)
* **deployment:** output build server messages on task completion ([9042f7a](https://github.com/DrBarnabus/DacTools/commit/9042f7addcda6fb501049e7e0e31b9104129af1a)), closes [#6](https://github.com/DrBarnabus/DacTools/issues/6)


### build

* **deployment:** Updated from .NET Core 3.0 to 3.1 ([531da17](https://github.com/DrBarnabus/DacTools/commit/531da17fd40ef814ad605365382ed83f198b0669)), closes [#10](https://github.com/DrBarnabus/DacTools/issues/10)

## [0.2.0](https://github.com/DrBarnabus/DacTools/compare/v0.1.0...v0.2.0) (2020-01-12)


### Bug Fixes

* **deployment:** updated HelpWriter with updated Help Text ([d8ee2c3](https://github.com/DrBarnabus/DacTools/commit/d8ee2c3d14490f301f3b8f4a7778424516e3bc49)), closes [#3](https://github.com/DrBarnabus/DacTools/issues/3)


### Features

* **deployment:** added command line parameters to configure DacDelpoyOptions sent to Microsoft DacServices ([3b56e1b](https://github.com/DrBarnabus/DacTools/commit/3b56e1b37175266e065378f628e6639a12747402)), closes [#1](https://github.com/DrBarnabus/DacTools/issues/1)



## 0.1.0 (2019-11-21)


### Bug Fixes

* **deployment:** added missing call to RegisterDatabaseListGenerators ([6230124](https://github.com/DrBarnabus/DacTools/commit/6230124140964eae0cde482bea190d78b1730b93))
* **deployment:** added missing parameter into DacDeployAsyncTask ProgressChanged Log Message ([599fff8](https://github.com/DrBarnabus/DacTools/commit/599fff81f9167972bc7a17a10ed3f3f4218f8a20))
* **deployment:** fixed blacklist argument parsing ([677f91c](https://github.com/DrBarnabus/DacTools/commit/677f91c5e97cd67cf1cbb033df43b47dab1ff53d))
* **deployment:** fixed DatabaseListGenerators ([afb64cd](https://github.com/DrBarnabus/DacTools/commit/afb64cda8c04e84781df5abfa6967075d6c2ba4c))
* **deployment:** fixed LogLevel parsing in ArgumentParser ([f91b778](https://github.com/DrBarnabus/DacTools/commit/f91b778291a4dbd3f87066634169e6287120bead))


### Features

* **deployment:** added basic argument parsing ([81d8e04](https://github.com/DrBarnabus/DacTools/commit/81d8e0414731d516290b5287c4a2d59f925228b9))
* **deployment:** added comma separated list parsing for databases argument ([6e68c6f](https://github.com/DrBarnabus/DacTools/commit/6e68c6febcd174433146296e084d12910f2da805))
* **deployment:** added DacDeployAsyncTask functionality, the tool can now successfully deploy DacPac's ([700e8d5](https://github.com/DrBarnabus/DacTools/commit/700e8d55e2923306742fbafed983dbdf6fef9784))
* **deployment:** added DatabaseListGenerators ([b118a05](https://github.com/DrBarnabus/DacTools/commit/b118a051dc50d25b24aaaaa827132a7d1c1e84fe))
* **deployment:** added ExecCommand to DacTools.Deployment ([75eab65](https://github.com/DrBarnabus/DacTools/commit/75eab65726f9da08848fdf29b0f952b60f57197f))
* **deployment:** added functionality to multiple tasks to deploy databases asynchronously ([8b2b6aa](https://github.com/DrBarnabus/DacTools/commit/8b2b6aa10ecb8b86399ae9f40838bd3430479527))
* **deployment:** added the initial set of arguments for deployment ([84f7101](https://github.com/DrBarnabus/DacTools/commit/84f7101f4d9f1b78bd27b42346fdc797dca78d8c))
* **deployment:** setup DacTools.Deployment project ([3c23f3a](https://github.com/DrBarnabus/DacTools/commit/3c23f3a9d750ac336688034ad16c4125e5e0d115))
* **deployment:** started the groundwork for Build Server detection ([d92fb8f](https://github.com/DrBarnabus/DacTools/commit/d92fb8f6cb7de4a6e21170b45c8134083990d222))
* implemented a basic logging system ([945acf8](https://github.com/DrBarnabus/DacTools/commit/945acf83a25df4997cff33ff9c3c2537a817791e))
