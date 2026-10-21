
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Sincronizador.Procesos
{
    public class Articulos
    {
        public  static List<Models.Articulo> GetArticulosSincronizar(string cnnstr)
        {
            DataTable dt = new DataTable();  
            List<Models.Articulo> lista = new List<Models.Articulo>();
            
            try
            {
                string query = "";

                query = @"select sta11.COD_ARTICU as Codigo,Concat(descripcio,' ',desc_adic) as Descripcion ,FLD1.DESCRIP  as Categoria, Precio,
                            STA11.ID_STA11   AS ID_TANGO,
                            checksum(sta11.cod_articu,Concat(descripcio,' ',desc_adic),  FLD1.DESCRIP, sta11ITC.IDFOLDER,PRECIO   ) as HASH 
                            from sta11
                            INNER JOIN sta11ITC ON STA11ITC.CODE=sta11.COD_ARTICU
                            INNER JOIN sta11FLD  FLD2 ON FLD2.IDFOLDER=sta11itc.IDFOLDER 
                             INNER JOIN sta11FLD FLD1 ON FLD2.IDPARENT =FLD1.IDFOLDER 
                             INNER JOIn gva17 ON gva17.COD_ARTICU=sta11.COD_ARTICU and nro_De_lis=3
                             LEFT JOIN Z_SINCRONIZADOR ON Z_SINCRONIZADOR.id_TANGO=sta11.ID_sta11 and Tabla='Articulos'
                             WHERE Perfil IN ('A','V')    
                             AND (Z_SINCRONIZADOR.HASH <>  checksum(sta11.cod_articu,Concat(descripcio,' ',desc_adic),  FLD1.DESCRIP, sta11ITC.IDFOLDER,PRECIO   )  or 
                            Z_SINCRONIZADOR.ID_TANGO IS NULL ) ";
                SqlDataAdapter da = new SqlDataAdapter(query, cnnstr);

                da.Fill(dt);

                if (dt.Rows.Count > 0)
                { 
                    for  ( int x=0; x<= dt.Rows.Count - 1;x++)
                    {
                        Models.Articulo art = new Models.Articulo();
                        try
                        {                            
                            art.Codigo = dt.Rows[x]["Codigo"].ToString();
                            art.Descripcion = dt.Rows[x]["Descripcion"].ToString();
                            art.Categoria = dt.Rows[x]["Categoria"].ToString();
                            art.Precio = Convert.ToDecimal(dt.Rows[x]["Precio"].ToString()); 
                            art.HASH = Convert.ToInt32(dt.Rows[x]["HASH"]);
                            art.ID_TANGO = Convert.ToInt32(dt.Rows[x]["ID_TANGO"]); 
                           
                            lista.Add(art);
                        }
                        catch (Exception ex)
                        {
                            Global.grabarLog($"GetArticulosSincronizar - Error en el articulo - {art.Codigo} -  {ex.Message}");
                        } 
                    } 
                } 

            }
            catch (Exception ex)
            {
                Global.grabarLog("GetArticulosSincronizar" + ex.Message);
            }
            return lista;
        }

 


    }
}
