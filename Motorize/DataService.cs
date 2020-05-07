using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Motorize.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Motorize
{
  public class DataService
  {
    private IJSRuntime _jsRuntime;

    public ChartValuesViewModel CurrentViewModel { get; set; }
    public event EventHandler<List<KeyValuePair<string, ChartValuesViewModel>>> OnDataSaved;
    public DataService(IJSRuntime jSRuntime)
    {
      this._jsRuntime = jSRuntime;
    }

    public async Task SaveCurrentViewModel(string modelName)
    {
      if(CurrentViewModel != null)
      {
        await _jsRuntime.InvokeVoidAsync("dataStorage.add", KeyValuePair.Create(modelName, CurrentViewModel));
        OnDataSaved?.Invoke(this, await GetAll());
      }
    }

    public async Task<List<KeyValuePair<string, ChartValuesViewModel>>> GetAll()
    {
      if(CurrentViewModel != null)
      {
        return await _jsRuntime.InvokeAsync<List<KeyValuePair<string, ChartValuesViewModel>>>("dataStorage.getAll");
      }
      return new List<KeyValuePair<string, ChartValuesViewModel>>();
    }
  }
}
