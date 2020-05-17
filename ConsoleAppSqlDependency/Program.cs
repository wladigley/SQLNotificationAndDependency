using System;
using System.Data;
using System.Data.SqlClient;

namespace ConsoleAppSqlDependency
{
    class Program
    {
        private static string connectionString = "Data Source=DIGLEY-PC;Initial Catalog=ENOVAR;User ID=RenataDba;Password=123456";
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
                    //O metodo start deve ser acionado antes da conexão ser aberta
                    SqlDependency.Start(cn.ConnectionString);
                    cn.Open();
                    // Declar o metodo stop quando for necessa´rio interromper a escuta
                    // SqlDependency.Stop(cn.ConnectionString);
                    using (var cmd = cn.CreateCommand())
                    {
                        cmd.CommandType = CommandType.Text;
                        // esste script dever ser padrão 
                        // este script será utilizado para ser vir de escuta, de forma quando houver alteração 
                        // Acionará seu handle
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

                        //  cria-se uma dependecia para o SQLCommand
                        SqlDependency dep = new SqlDependency(cmd);
                        //  Cria a ação para quando houver alteração no banco de dados
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
