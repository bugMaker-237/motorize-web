﻿using Blazorise.Charts;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Motorize.Components
{
  public partial class Chart<TItem>
  {
    LineChart<Models.Point> lineChart;
    [Inject]
    ChartValuesState State { get; set; }


    protected override void OnInitialized()
    {
      this.State.OnDChanged += this.OnDChanged;
    }
    protected override Task OnAfterRenderAsync(bool firstRender)
    {
      if (firstRender)
      {
        this.InitializeChart();
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
        //this.lineChart.SetOptions(new LineChartOptions
        //{
        //  Scales = new Scales
        //  {
        //    XAxes = e.Select(x => new Axe
        //    {
        //      Display = true,
        //      Ticks = new AxeTicks
        //      {
        //        //FontColor = ,
        //        Major = new AxeMajorTick(),
        //        Minor = new AxeMinorTick(),
        //        Padding = 2,
        //        Display = true
        //      }
        //    }).ToList()
        //  }
        //});
        for (var i = 0; i < e.Length; i++)
        {
          this.lineChart.AddDataSet(new LineChartDataset<Models.Point>
          {
            Label = "Rapport " + (i+1),
            Data = e[i].Select(i => new Models.Point { x=i.Item1, y= i.Item2 }).ToList(),
            BackgroundColor = GetColor(i),  
            BorderColor = GetColor(i),
            Fill = false,
            PointRadius = 3,
            BorderDash = new List<int> { }
          });
        }
        await this.lineChart.Update();
      }
      else
      {
        this.InitializeChart();
      }
    }

    private List<string> GetColor(int i)
    {
      switch (i)
      {
        case 0:
          return new List<string> { "#5B9BD5" };
        case 1:
          return new List<string> { "#ED7D31" };
        case 2:
          return new List<string> { "#A5A5A5" };
        case 3:
          return new List<string> { "#FFC000" };
        case 4:
          return new List<string> { "#4472C4" };
        case 5:
          return new List<string> { "#70AD47" };
        default:
          return new List<string> { "#5B9BD5" };
      }
    }

  }
}
