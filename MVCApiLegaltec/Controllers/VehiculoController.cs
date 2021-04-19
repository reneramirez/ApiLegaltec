using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MVCApiLegaltec.Models;
using MySql.Data.MySqlClient;

namespace MVCApiLegaltec.Controllers
{
  [Route("api/[controller]")]
  [ApiController]
  public class VehiculoController : ControllerBase
  {
    private readonly IConfiguration _config;
    private string _connection;
    public VehiculoController(IConfiguration config)
    {
      _config = config;
      _connection = _config.GetValue<string>("ConnectionStrings:mysql");
    }


    [HttpGet]
    [Route("ListMarcas")]
    public IActionResult GetMarcas()
    {
      IEnumerable<Models.Marca> lst = null;
      using (var db = new MySqlConnection(_connection))
      {
        var sql = @"SELECT 
                      marca.id_marca
                      ,marca.glosa_marca 
                    FROM marca
                    ";
        lst = db.Query<Models.Marca>(sql);
      }
      return Ok(lst);
    }

    [HttpGet]
    [Route("ListModelos")]
    [Route("ListModelos/{id}")]
    public IActionResult getModelos(int? id)
    {
      IEnumerable<Models.Modelo> lst = null;
      using (var db = new MySqlConnection(_connection))
      {
        var where = "";
        if(id > 0){
          where = $"where modelo.id_marca = {id}";  
        }
        var sql = $@"SELECT 
                      modelo.id_modelo
                      ,modelo.glosa_modelo
                      ,marca.id_marca
                      ,marca.glosa_marca 
                    FROM modelo
                      left join marca on marca.id_marca = modelo.id_marca
                    {where}
                    ";
        lst = db.Query<Models.Modelo, Models.Marca, Models.Modelo>(sql, (modelo, marca) => {
          modelo.marca = marca;
          return modelo;
        }, splitOn: "id_marca");
      }
      return Ok(lst);
    }

    [HttpGet("GetVehiculo/{id}")]
    public IActionResult getVehiculo(string? id)
    {
      Models.VehiculoAux vehiculoAux = new Models.VehiculoAux();
      using (var db = new MySqlConnection(_connection))
      {
        var where = "";
        if (id != "")
        {
          where = $"where vehiculo.patente = '{id}'";
        }
        var sql = $@"SELECT 
                      vehiculo.id_vehiculo
                      ,vehiculo.id_modelo
                      ,vehiculo.patente
                      ,marca.id_marca
                      ,marca.glosa_marca
                      ,modelo.glosa_modelo
                    FROM vehiculo
                      left join modelo on modelo.id_modelo = vehiculo.id_modelo
                      left join marca on marca.id_marca = modelo.id_marca
                    {where}
                    ";
        vehiculoAux = db.QuerySingle<Models.VehiculoAux>(sql);
      }
      return Ok(vehiculoAux);
    }

    [HttpGet("GetAllVehiculo")]
    public IActionResult getAllVehiculo()
    {
      IEnumerable<Models.VehiculoAux> lst = null;
      using (var db = new MySqlConnection(_connection))
      {
        var sql = $@"SELECT 
                      vehiculo.id_vehiculo
                      ,vehiculo.id_modelo
                      ,vehiculo.patente
                      ,marca.id_marca
                      ,marca.glosa_marca
                      ,modelo.glosa_modelo
                    FROM vehiculo
                      left join modelo on modelo.id_modelo = vehiculo.id_modelo
                      left join marca on marca.id_marca = modelo.id_marca
                    ";
        lst = db.Query<Models.VehiculoAux>(sql);
      }
      return Ok(lst);
    }

    [HttpGet("GetDetalleVehiculo/{id}")]
    public IActionResult getDetalleVehiculo(int? id)
    {
      
      IEnumerable<Models.DetalleVehiculoAux> lst = null;
      using (var db = new MySqlConnection(_connection))
      {
        var where = "";
        if (id > 0)
        {
          where = $"where vehiculo.id_vehiculo = {id}";
        }
        var sql = $@"SELECT 
                      vehiculo_detalle.id_vehiculo
                      ,vehiculo_detalle.id_vehiculo_detalle
                      ,vehiculo_detalle.id_tipo_servicio
                      ,vehiculo_detalle.id_estado_pago
                      ,vehiculo_detalle.monto
                      ,estado_pago.glosa_estado_pago
                      ,tipo_servicio.glosa_tipo_servicio
                    FROM vehiculo_detalle
                      left join vehiculo on vehiculo.id_vehiculo = vehiculo_detalle.id_vehiculo
                      left join estado_pago on estado_pago.id_estado_pago = vehiculo_detalle.id_estado_pago
                      left join tipo_servicio on tipo_servicio.id_tipo_servicio = vehiculo_detalle.id_tipo_servicio
                    {where}
                    ";
        lst = db.Query<Models.DetalleVehiculoAux>(sql);
      }
      return Ok(lst);
    }

    [HttpPost]
    [Route("InsertVehiculo")]
    public IActionResult insertVehiculo(Models.Vehiculo vehiculo){
      int result = 0;
      using (var db = new MySqlConnection(_connection))
      {
        var sql  = "";
        if(vehiculo.id_vehiculo >0){
          
          sql = @" update vehiculo 
                    set id_modelo = @id_modelo
                    , patente = @patente
                    , fh_modificacion = NOW()
                    where id_vehiculo = @id_vehiculo
                  ";
          result = db.Execute(sql, vehiculo);
        }
        else{
          sql = @"insert into vehiculo (id_modelo, patente, fh_creacion)
                  value(@id_modelo, @patente, NOW());
                  select LAST_INSERT_ID();
                 ";
          result = db.Query<int>(sql, vehiculo).Single();
        }
      }
      if (vehiculo.id_vehiculo > 0)
      {
        result = vehiculo.id_vehiculo;
      }
      
      return Ok(new { id_vehiculo = result });
    }

    [HttpPost]
    [Route("InsertDetalle")]
    public IActionResult InsertDetalle(IEnumerable<Models.DetalleVehiculo> detalle){
      int result = 0;
      using (var db = new MySqlConnection(_connection))
      {
        foreach (var item in detalle)
        {
          var sql = "";
          if(item.id_vehiculo_detalle > 0){
            sql = @" update vehiculo_detalle 
                    set id_vehiculo = @id_vehiculo
                    ,id_tipo_servicio = @id_tipo_servicio
                    ,id_estado_pago = @id_estado_pago
                    ,monto = @monto
                    ,fh_modificacion = NOW()
                    where id_vehiculo_detalle = @id_vehiculo_detalle
                  ";
          }else{
            sql = @"insert into vehiculo_detalle (id_vehiculo, id_tipo_servicio, id_estado_pago, monto, fh_creacion)
                  value(@id_vehiculo, @id_tipo_servicio, @id_estado_pago, @monto, NOW())";
          }
          result = db.Execute(sql, item);
        }
      }
      return Ok(new { id_vehiculo = result });
    }


  }

}