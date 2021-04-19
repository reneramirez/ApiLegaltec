using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCApiLegaltec.Models
{
  public class Modelo
  {
    public int id_modelo { get; set; }
    public string glosa_modelo { get; set; }
    public Marca marca { get; set; }
  }
}
