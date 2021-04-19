using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCApiLegaltec.Models
{
  public class VehiculoAux
  {
    public int id_vehiculo { get; set; }
    public int id_modelo { get; set; }
    public int id_marca { get; set; }
    public string glosa_marca { get; set; }
    public string glosa_modelo { get; set; }
    public string patente { get; set; }
  }
}
