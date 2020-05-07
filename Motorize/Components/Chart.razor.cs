using Blazorise;
using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Motorize.Components
{
  public partial class Chart<TItem>
  {
    ScatteredChart lineChart;
    ElementReference canvasWrapper;
    Modal modalRef;
    string name;
    bool hasBeenDrawn = false;

    [Inject] 
    IJSRuntime JSRuntime { get; set; }
    [Inject]
    ChartValuesState State { get; set; }
    [Inject]
    DataService Service { get; set; }


    protected override void OnInitialized()
    {
    }
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        this.InitializeChart();
        this.State.OnDChanged += this.OnDChanged;
      }
      return base.OnAfterRenderAsync(firstRender);
    }
    private void InitializeChart()
    {
      this.lineChart.Clear();
      this.lineChart.AddLabel("0");
      this.lineChart.AddDataSet(new LineChartDataset<Models.Point>
      {
        Label = "Final Graph",
        Data = new List<Models.Point>()
      });
    }

    async void FullScreen()
    {
      await JSRuntime.InvokeAsync<string>("toggleFullScreen", canvasWrapper);
    }
    

    async void Download()
    {
      await lineChart.Update();
      await JSRuntime.InvokeAsync<string>("window.print");
    }
    async void ShowModal()
    {
      if(hasBeenDrawn)
      modalRef.Show();
    }
    async void HideModal()
    {
      if (!string.IsNullOrWhiteSpace(this.name?.Trim()))
      {
        this.modalRef.Hide();
        await Service.SaveCurrentViewModel(this.name);
      }
    }
    private async void OnDChanged(object sender, List<Tuple<decimal, decimal>>[] e)
    {
      if (e != null)
      {
        this.lineChart.Clear();
        var all = e.SelectMany(x => x).Distinct().Select(i => i.Item1).ToList();
        var max = all.Max() + 10M;
        all.Add(max);
        all.Insert(0, 0);
        this.lineChart.AddLabel(all.Select(x => x.ToString()).ToArray());
        for (var i = 0; i < e.Length; i++)
        {
          this.lineChart.AddDataSet(new LineChartDataset<Models.Point>
          {
            Label = "Rapport " + (i + 1),
            Data = e[i].Select(e=> new Models.Point { x=e.Item1, y= e.Item2 }).ToList(),
            BackgroundColor = this.GetColor(i),
            BorderColor = this.GetColor(i),
            Fill = false,
            PointRadius = 3,
            BorderDash = new List<int> { }
          });
        }
        await this.lineChart.Update();
        hasBeenDrawn = true;
      }
      else
      {
        this.InitializeChart();
        await this.lineChart.Update();
      }
    }

    private List<string> GetColor(int i) => i switch
    {
      0 => new List<string> { "#5B9BD5" },
      1 => new List<string> { "#ED7D31" },
      2 => new List<string> { "#A5A5A5" },
      3 => new List<string> { "#FFC000" },
      4 => new List<string> { "#4472C4" },
      5 => new List<string> { "#70AD47" },
      _ => new List<string> { "#5B9BD5" },
    };

  }
}
