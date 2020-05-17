using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ConsoleAppSqlDependency
{
    class Program
    {
        private static string connectionString = "Data Source=DIGLEY-PC;Initial Catalog=ENOVAR;User ID=DigleyDba;Password=123456";
        static void Main(string[] args)
        {
            StartListening();
            Console.ReadKey();
        }
        private static void StartListening()
        {
            try
            {
                using (var cn = new SqlConnection(connectionString))
                {
                    SqlDependency.Start(cn.ConnectionString);
                    cn.Open();
                    //SqlDependency.Stop(cn.ConnectionString);
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = @"
                        select
                         [id]
                         ,[data]
                         ,[loja]
                         ,[cliente]
                         ,[valor]
                        from [dbo].[vendas]
                        ";

                        cmd.Notification = null;

                        //  creates a new dependency for the SqlCommand
                        SqlDependency dep = new SqlDependency(cmd);
                        //  creates an event handler for the notification of data
                        //      changes in the database.
                        dep.OnChange += Dep_OnChange;

                        cmd.ExecuteReader();
                    }
                }
                Console.WriteLine("Listening...");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error..." + ex);
            }

        }

        private static void Dep_OnChange(object sender, SqlNotificationEventArgs e)
        {
            Console.WriteLine("Change caught! Triggering update to Webserver..." + e.Info);
            StartListening();
        }
    }
}
