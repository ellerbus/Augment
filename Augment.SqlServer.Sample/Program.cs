using System.Data.SqlClient;

namespace Augment.SqlServer.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=SAMPLE;Integrated Security=SSPI;";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                Installer installer = new Installer(typeof(Program).Assembly, con);

                installer.Install();
            }
        }
    }
}
