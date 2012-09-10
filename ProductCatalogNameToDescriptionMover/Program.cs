using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProductCatalogNameToDescriptionMover.zuora;

namespace ProductCatalogNameToDescriptionMover
{
    class Program
    {

        static string USERNAME = "";
        static string PASSWORD = "";
        static string ENDPOINT = "https://apisandbox.zuora.com/apps/services/a/38.0";
        private zuora.ZuoraService binding;

        public Program()
        {
            binding = new zuora.ZuoraService();
            binding.Url = ENDPOINT;

        }

        static void Main(string[] args)
        {
            Program p = new Program();
            p.login();
            Console.WriteLine("getting product rate plans");
            List<ProductRatePlan> prpList = p.getProductRatePlans();
            Console.WriteLine("getting product rate plan charges");
            List<ProductRatePlanCharge> prpcList = p.getProductRatePlanCharges();
            foreach (ProductRatePlan prp in prpList)
            {
                prp.Description = prp.Name;
                Console.WriteLine("Updating the description for rate plan: " + prp.Name + " result: " + p.update(prp));   
            }
            foreach (ProductRatePlanCharge prpc in prpcList)
            {
                prpc.Description = prpc.Name;
                Console.WriteLine("Updating the description for charge: " + prpc.Name + " result: " + p.update(prpc));
            }
            Console.ReadLine();
        }
        //login
        private bool login()
        {

            try
            {
                //execute the login placing the results  
                //in a LoginResult object 
                zuora.LoginResult loginResult = binding.login(USERNAME, PASSWORD);

                //set the session id header for subsequent calls 
                binding.SessionHeaderValue = new zuora.SessionHeader();
                binding.SessionHeaderValue.session = loginResult.Session;

                //reset the endpoint url to that returned from login 
                // binding.Url = loginResult.ServerUrl;

                Console.WriteLine("Session: " + loginResult.Session);
                Console.WriteLine("ServerUrl: " + loginResult.ServerUrl);

                return true;
            }
            catch (Exception ex)
            {
                //Login failed, report message then return false 
                Console.WriteLine("Login failed with message: " + ex.Message);
                return false;
            }
        }
        private string create(zObject acc)
        {
            SaveResult[] result = binding.create(new zObject[] { acc });
            return result[0].Id;
        }
        private List<ProductRatePlan> getProductRatePlans()
        {
            List<ProductRatePlan> output = new List<ProductRatePlan>();
            QueryResult qResult = binding.query("SELECT id, name, description FROM ProductRatePlan");
            foreach(zObject qr in qResult.records)
            {
                output.Add((ProductRatePlan)qr);
            }
            return output;
        }
        private List<ProductRatePlanCharge> getProductRatePlanCharges()
        {
            List<ProductRatePlanCharge> output = new List<ProductRatePlanCharge>();
            QueryResult qResult = binding.query("SELECT id, name, description FROM ProductRatePlanCharge");
            foreach (zObject qr in qResult.records)
            {
                output.Add((ProductRatePlanCharge)qr);
            }
            return output;
        }

        private Account queryAccount(string accId)
        {
            QueryResult qResult = binding.query("SELECT id, name, accountnumber FROM account WHERE id = '" + accId + "'");
            Account rec = (Account)qResult.records[0];
            return rec;
        }

        private string update(zObject acc)
        {
            SaveResult[] result = binding.update(new zObject[] { acc });
            return result[0].Id;
        }

        private bool delete(String type, string id)
        {

            DeleteResult[] result = binding.delete(type, new string[] { id });
            return result[0].success;

        }
    }
}
