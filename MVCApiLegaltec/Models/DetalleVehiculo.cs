using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCApiLegaltec.Models
{
  public class DetalleVehiculo
  {

    public int id_vehiculo_detalle { get; set; }
    public int id_vehiculo { get; set; }
    public int id_tipo_servicio { get; set; }
    public int id_estado_pago { get; set; }
    public int monto { get; set; }
  }
}
