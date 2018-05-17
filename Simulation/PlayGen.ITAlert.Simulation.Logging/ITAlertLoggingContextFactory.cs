using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.Entity;
using MySql.Data.MySqlClient;

namespace PlayGen.ITAlert.Simulation.Logging
{
    public class ITAlertLoggingContextFactory
    {
        public static ITAlertLoggingContext Create()
        {
            var connectionStrings = System.Configuration.ConfigurationManager.ConnectionStrings;
            var connectionString = connectionStrings["DatabaseEventLoggerContext"].ConnectionString;

            DbConfiguration.SetConfiguration(new MySqlEFConfiguration());

            var connection = new MySqlConnection(connectionString);
            var context = new ITAlertLoggingContext(connection, true);
            context.Database.CreateIfNotExists();

            return context;
        }
    }
}
