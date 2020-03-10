using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Motorize.Components
{
  public class ChartValuesState
  {
    private List<Tuple<decimal, decimal>>[] _D;
    public event EventHandler<List<Tuple<decimal, decimal>>[]> OnDChanged;

    public void SetD (List<Tuple<decimal, decimal>>[] value)
    {
      this._D = value;
      OnDChanged?.Invoke(this, value);
    }
  }
}
