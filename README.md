# BostonCodeCamp36Demo
A project containing a .NET MAUI application and workflows that automatically build all the platform the projects for you.

The project itself is not relevant, so we use a simple app that is just to prove it is expected to run:

![app and code](https://github.com/LanceMcCarthy/BostonCodeCamp36Demo/assets/3520532/f7ba35ec-db24-4172-8290-8611da3e165d)

## Workflows

What is important are the steps inside the GitHub Actions workflow files. The key takaway in this demonstration are there are two approaches usually required by dev teams:

1. What do we do for normal builds? I just want to verify the  just verifying the code builds after day-to-day work?
2. What do we do for release builds, when we want a package to install/publish?

My presentation today will show you both approaches.

To do normal builds for #1, we have the [main.yml](./.github/main.yml) workflow. It listens for pushes to `main` branch (and issue or feature branches) desired in #1, we can use a single workflow that runs all quick builds in parallel. This is demonstrated in 

However, for #2, we need a little more complixity because release builds usually need code signing secrets. The [releases.yml](./.github/releases.yml) workflow is triggered when a commit is pushed to a branch named `releases/*` (also works if a new brnach is created that matches that name pattern). That top-level workflow will then trigger four separate actions (in the .releases/ subfolder), that will run in parallel build the code in release mode, packaging/code signing, and upload the artifact to the action's result.

> [!NOTE]
> You can also publish these artifacts to dev portals in the workflow (Test Flight, Google Play Console, Microsoft Partner Center, etc). However, this topic is outside the scope of today's presentation, as it is a different conversation about endpoint authentication with developer credentials.


## Automatic Switching in csproj

There is also a nice little trick you can do with `Condition` statments inside the csproj file that will automaticlaly set the platform-specific properties based on the build's target platform.

For example, let's start with Android.

```xml
<!-- Notice the Condition for '-android' -->

  <PropertyGroup Condition="$(TargetFramework.Contains('-android')) and '$(Configuration)' == 'Release'">
    <AndroidKeyStore>True</AndroidKeyStore>
    <AndroidSigningKeyStore>myapp.keystore</AndroidSigningKeyStore>
    <AndroidSigningStorePass>$(android_keystore_password)</AndroidSigningStorePass>
    <AndroidSigningKeyAlias>myappkeystore.alias</AndroidSigningKeyAlias>
    <AndroidSigningKeyPass>$(android_keystore_alias_password)</AndroidSigningKeyPass>
  </PropertyGroup>

```

For iOS this can be a little trickier because you may need to switch out the certificate you are signing with. 

- local iphone uses a development cert
- Adhoc distribution
- TestFlight
- AppStore

```xml
<!-- Notice the Condition for '-ios' -->

  <!-- Debug / Simulator -->
  <PropertyGroup Condition="$(TargetFramework.Contains('-ios')) and '$(Configuration)' == 'Debug'">
    <ProvisioningType>manual</ProvisioningType>
    <RuntimeIdentifier>iossimulator-x64</RuntimeIdentifier>
    <CodesignKey>Apple Development: Lance McCarthy (xxxxx)</CodesignKey>
    <CodesignProvision>MyApp_iOS_Development_2023.mobileprovision</CodesignProvision>
  </PropertyGroup>

  <!-- Debug / Local iPhone --> 
  <PropertyGroup Condition="$(TargetFramework.Contains('-ios')) and '$(Configuration)' == 'Debug'">
    <ProvisioningType>manual</ProvisioningType>
    <RuntimeIdentifier>ios-arm64</RuntimeIdentifier>
    <CodesignKey>Apple Development: Lance McCarthy (xxxxx)</CodesignKey>
    <CodesignProvision>MyApp_iOS_Development_2023.mobileprovision</CodesignProvision>
  </PropertyGroup>

  <!-- Release / AdHoc -->
  <PropertyGroup Condition="$(TargetFramework.Contains('-ios')) and '$(Configuration)' == 'Release'">
    <ProvisioningType>manual</ProvisioningType>
    <RuntimeIdentifier>ios-arm64</RuntimeIdentifier>
    <CodesignKey>Apple Distribution: Lancelot Software, LLC (xxxxx)</CodesignKey>
    <CodesignProvision>MyApp_AdHoc_Distribution_2023.mobileprovision</CodesignProvision>
  </PropertyGroup>

  <!-- Release / App Store -->
  <PropertyGroup Condition="$(TargetFramework.Contains('-ios')) and '$(Configuration)' == 'Release'">
    <ProvisioningType>manual</ProvisioningType>
    <RuntimeIdentifier>ios-arm64</RuntimeIdentifier>
    <CodesignKey>Apple Distribution: Lancelot Software, LLC (xxxxx)</CodesignKey>
    <CodesignProvision>MyApp_AppStore_Distribution_2023.mobileprovision</CodesignProvision>
  </PropertyGroup>
```

For MacCatalyst, it is a bit easier, we only need two situations to cover.

```xml
<!-- Notice the Condition for '-maccatalyst' -->

  <!-- Debug Local -->
  <PropertyGroup Condition="$(TargetFramework.Contains('-maccatalyst')) and '$(Configuration)' == 'Debug'">
    <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
    <CodesignKey>Apple Development: Lance McCarthy (xxxxx)</CodesignKey>
    <CodesignProvision>MyApp_Dev_MacOS_2022.mobileprovision</CodesignProvision>
  </PropertyGroup>

  <!-- Release - App Store -->
  <PropertyGroup Condition="$(TargetFramework.Contains('-maccatalyst')) and '$(Configuration)' == 'Release'">
    <RuntimeIdentifier>maccatalyst-x64</RuntimeIdentifier>
    <CodesignKey>Apple Distribution: Lancelot Software, LLC (xxxxx)</CodesignKey>
    <CodesignProvision>MyApp_MacStore_2022.provisionprofile</CodesignProvision>
  </PropertyGroup>
```
