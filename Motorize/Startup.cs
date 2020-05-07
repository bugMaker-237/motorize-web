using Blazorise;
using Blazorise.Bootstrap;
using Blazorise.Icons.FontAwesome;
using Blazorise.Icons.Material;
using Blazorise.Material;
using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using RazorComponentsPreview;

namespace Motorize {
  public class Startup {
    public void ConfigureServices (IServiceCollection services) {
      services
        .AddBlazorise(options =>
        {
          options.ChangeTextOnKeyPress = true;
        }) // from v0.6.0-preview4
           // .AddMaterialProviders ()
           // .AddMaterialIcons ();
        .AddBootstrapProviders()
        .AddSingleton<Motorize.Components.ChartValuesState>()
        .AddSingleton<Motorize.DataService>()
        .AddFontAwesomeIcons();
    }

    public void Configure (IComponentsApplicationBuilder app) {
      app.Services
        // .UseMaterialProviders ()
        // .UseMaterialIcons ();
        .UseBootstrapProviders()
        .UseFontAwesomeIcons();

      app.AddComponent<App> ("app");
    }
  }
}