using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.ServiceProcess;
 

namespace Sincronizador
{
    public partial class Service : ServiceBase{
        
        System.Threading.Timer timer = null; 
        
        public Service(){
            InitializeComponent();
           
        }

        protected override void OnStart(string[] args) { 
         
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            timer =new System.Threading.Timer(_=> OnTimer(),null,100,100);  
               
        }
        


        public void OnTimer ()
        {
            string paraLog = "";
            timer.Change(6000000 * 15, 6000000 * 15);
            
            try
            { 

                paraLog = paraLog + " - Antes de Sync de articulos";                
                SyncArticulos();                
                paraLog = paraLog + " - Despues de Sync de articulos"; 
            }
            catch (Exception ex)
            { 
                Global.grabarLog("ERROR EN EL TIMER GENERAL " + ex.Message + " - " + paraLog);
            }
            finally {
                timer.Change(60000 * 15 ,  60000 * 15 );
            }
        }
         
        private void SyncArticulos()
        {
            try
            {
                string cnnstr= "Server=DELNORTESERVER1\\AXSQLEXPRESS;Database=Del_Norte_Nueva_2023;User Id=Axoft;Password=Axoft;";



                List<Models.Articulo> lstArticulos = Procesos.Articulos.GetArticulosSincronizar(cnnstr);
               
                int batchSize = 10; // Tamaño del lote
                var splitArticulos = SplitList(lstArticulos, batchSize);

               
                foreach (var ListaCortada in splitArticulos)
                {
                    RestClient client = new RestClient($"https://yhnjym8bmg.execute-api.sa-east-1.amazonaws.com/Prod/Api/Articulos/multiple");
                   
                    RestRequest request = new RestRequest(Method.POST);
                    request.AddHeader("Content-Type", "application/json");
                    request.AddParameter("undefined", Newtonsoft.Json.JsonConvert.SerializeObject(ListaCortada), ParameterType.RequestBody);
                    IRestResponse response = client.Execute(request);

                    if (response.StatusCode != System.Net.HttpStatusCode.OK)
                    {
                        Global.grabarLog("ERROR Articulo "  + response.ErrorMessage);
                    }
                    else
                    {
                        foreach (var artIndividual in ListaCortada) {

                            
                            Global.InsertOrUpdateHash(cnnstr, "Articulos", artIndividual.ID_TANGO, artIndividual.HASH);
                        } 

                    } 
 
                }


 
            }
            catch (Exception ex)
            {

                Global.grabarLog("ERROR EN LA FUNCIONES DE ARTICULOS " + ex.Message);
            } 
        }

        public static IEnumerable<List<T>> SplitList<T>(List<T> source, int chunkSize)
        {
            for (int i = 0; i < source.Count; i += chunkSize)
            {
                yield return source.GetRange(i, Math.Min(chunkSize, source.Count - i));
            }
        }


          
        protected override void OnStop(){

        }
    }
}
