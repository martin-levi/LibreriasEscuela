using System;
using System.Data;
using System.Data.SqlClient;

namespace Sincronizador
{
    public static class Global {
         
         

        //TABLA VARCHAR(20)
        //ID_TANGO INT
        //HASH VARBINARY(16)

        public static bool InsertOrUpdateHash(string cnnstr, string tabla, Int32 id, Int32 hash)
        {
            

            SqlConnection cnn = new SqlConnection();
            SqlCommand cmd = new SqlCommand();
            string query = "";
            cnn.ConnectionString = cnnstr;


            cnn.Open(); 
            cmd.Connection = cnn;
            try
            {
                if (ComprobarHASH(cnnstr, tabla, id) == false)
                {

                    query = $"INSERT INTO Z_SINCRONIZADOR (TABLA,ID_TANGO,HASH) VALUES ('{tabla}',{id},{hash})";
                }
                else
                {
                    query = $"UPDATE Z_SINCRONIZADOR SET HASH={hash} WHERE ID_TANGO={id} AND TABLA='{tabla}'";

                }
               
                cmd.CommandText = query;
                cmd.ExecuteNonQuery();


                cnn.Close();
                cnn.Dispose();
                cmd.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                grabarLog($"Al insertar/updatear HASH - Tabla {tabla} - {ex.Message}");
                return false;
            }
           
            

        }



        public static void grabarLog(string texto)
        {
            System.IO.StreamWriter log = new System.IO.StreamWriter(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\log\\" + DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + DateTime.Now.Day.ToString() + ".txt", true);
            log.WriteLine(texto);
            log.Close();
        }

       

 
        public static bool ComprobarHASH(string cnnstr, string tabla, Int32 id)
        {
            bool existe = false;
            DataTable dt = new DataTable();
            string cn = cnnstr;
            string query = $"SELECT 1  FROM Z_SINCRONIZADOR WHERE ID_TANGO={id} AND TABLA='{tabla}'";
            SqlDataAdapter da = new SqlDataAdapter(query, cn);
            da.Fill(dt);
            if (dt.Rows.Count > 0) {
                existe = true;
            }

            return existe; 
        }
         
        
        
    }
}
