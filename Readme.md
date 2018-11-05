# .NET Core Angular HMR Spa

Created using dotnet new angular.

Updated to run Angular from a sub-path off the root.

https://localhost:5001/app/

Any requests outside of this path are not handled by the Spa so can be used for MVC and Razor pages etc.

This setup works in both development and production modes.

In dev mode the /app/ path segment of a Request is retained before the request is passed to the dev server as the dev server is serving the app from /app/ path to match production mode.
In Production mode the /app/ path is removed before being passed to UseSpaStaticFiles() as the bundle files correctly exist in the root of the 'ClientApp/dist' dir, not an app subdir.

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

Commented out spa.UseAngularCliServer(npmScript: "hmr"); as it can't be used. UseAngularCliServer starts the Angular dev server on a random port but we need to know the port in order to direct the websocket requests that are outside of the /app/ sub-path over to the dev server. See source code in Startup.cs for details

Updated the getBaseUrl() function in main.ts as it needs to request data from
"/api/SampleData/WeatherForecasts" rather than "/app/api/SampleData/WeatherForecasts"

Updated getBaseUrl() function:
```
export function getBaseUrl() {
  //return document.getElementsByTagName('base')[0].href;
  return  `${window.location.origin}/`;
}
```

### Running and Building

To run in Development mode

Open a terminal in ClientApp dir:
```sh
npm run hmr
```

Open a terminal in root dir:
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