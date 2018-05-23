using System;
using System.Data.SqlClient;
using System.IO;

namespace Augment.SqlServer.Sample
{
    class Program
    {
        private static string ConnectionString = @"Server=(LocalDB)\MSSQLLocalDB; Integrated Security=SSPI; AttachDbFilename=|DataDirectory|\SAMPLE.mdf; Database=SAMPLE;";

        static void Main(string[] args)
        {
            string path = Directory.GetParent(AppContext.BaseDirectory).FullName;

            ConnectionString = ConnectionString.Replace("|DataDirectory|", path);

            DropDatabase();
            CreateDatabase();
            Install();
        }

        private static void DropDatabase()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);

            using (SqlConnection con = new SqlConnection($"Server={builder.DataSource}"))
            {
                con.Open();

                string sql =
                    $"if exists(select 1 from master.sys.databases where name = '{builder.InitialCatalog}')" + Environment.NewLine +
                    $"begin" + Environment.NewLine +
                    $"  drop database {builder.InitialCatalog}" + Environment.NewLine +
                    $"end";

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void CreateDatabase()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(ConnectionString);

            using (SqlConnection con = new SqlConnection($"Server={builder.DataSource}"))
            {
                con.Open();

                string mdf = builder.AttachDBFilename;
                string ldf = builder.AttachDBFilename.Replace(".mdf", ".ldf");

                string sql =
                    $"if not exists(select 1 from master.sys.databases where name = '{builder.InitialCatalog}')" + Environment.NewLine +
                    $"begin" + Environment.NewLine +
                    $"  create database {builder.InitialCatalog}" + Environment.NewLine +
                    $"    on primary (name={builder.InitialCatalog}_data, filename='{mdf}')" + Environment.NewLine +
                    $"    log on (name={builder.InitialCatalog}_log, filename='{ldf}')" + Environment.NewLine +
                    $"end";

                using (SqlCommand cmd = con.CreateCommand())
                {
                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private static void Install()
        {
            using (SqlConnection con = new SqlConnection(ConnectionString))
            {
                con.Open();

                Installer installer = new Installer(typeof(Program).Assembly, con);

                installer.Install();
            }
        }
    }
}
