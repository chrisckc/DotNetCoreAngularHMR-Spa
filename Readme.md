# .NET Core Angular HMR Spa

Created using dotnet new angular

Removed "ASPNETCORE_ENVIRONMENT": "Development" entry from Properties/launchSettings.json to allow running in production mode by using env vars.

Hot Module Replacement (HMR) was then enabled enabled by following:
https://github.com/angular/angular-cli/wiki/1-x-stories-configure-hmr

Hot Module Replacement (HMR) is a WebPack feature to update code in a running app without rebuilding it. This results in faster updates and less full page-reloads.

Startup.cs modified to use HMR:
```
spa.UseAngularCliServer(npmScript: "hmr");
```

Added an MVC index route using the default HomeController from the standard MVC template.
This was added to test running the Spa from a sub-path in another branch of this repo.

To run in Development mode

```sh
ASPNETCORE_ENVIRONMENT=Development dotnet run
```

To run in Production mode first build the Angular app in prod mode:

```sh
cd ClientApp
npm run build -- --prod
cd ..
ASPNETCORE_ENVIRONMENT=Production dotnet run
```