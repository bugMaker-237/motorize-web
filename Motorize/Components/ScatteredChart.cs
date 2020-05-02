using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Motorize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Motorize.Components
{
  public class ScatteredChart : LineChart<Models.Point>
  {
    [Inject]
    private IJSRuntime JSRuntime { get; set; }

    private static object CreateDotNetObjectRefSyncObj = new object();
    private DotNetObjectReference<ChartAdapter> dotNetObjectRef;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        await Initialize();
      }
      else
      {
        await Update();
      }
    }

    private async Task Initialize()
    {
      if (dotNetObjectRef == null)
      {
        dotNetObjectRef = CreateDotNetObjectRef(new ChartAdapter(this));
      }
      await InitializeChart(JSRuntime, dotNetObjectRef, Clicked.HasDelegate, Hovered.HasDelegate, base.ElementId, Data, Options, DataJsonString, OptionsJsonString);
    }

    private ValueTask<bool> InitializeChart(IJSRuntime jSRuntime, DotNetObjectReference<ChartAdapter> dotNetObjectReference, bool hasClickEvent, bool hasHoverEvent, string canvasId, ChartData<Point> data, LineChartOptions options, string dataJsonString, string optionsJsonString)
    {
      return JSRuntimeExtensions.InvokeAsync<bool>(jSRuntime, "blazoriseCharts.initialize", new object[9]
      {
        dotNetObjectReference,
        hasClickEvent,
        hasHoverEvent,
        canvasId,
        "scatter",
        ToChartDataSet(data),
        options,
        dataJsonString,
        optionsJsonString
      });
    }

    private object ToChartDataSet<T>(ChartData<T> data)
    {
      return new
      {
        Labels = data.Labels,
        Datasets = ((IEnumerable<object>)data.Datasets).ToList()
      };
    }

    private DotNetObjectReference<ChartAdapter> CreateDotNetObjectRef(ChartAdapter adapter)
    {
      lock (CreateDotNetObjectRefSyncObj)
      {
        return DotNetObjectReference.Create(adapter);
      }
    }
  }
}
