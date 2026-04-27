using System;
using MySql.Data.MySqlClient;
using SIMANIK.Repositories;

namespace SIMANIK.Helpers
{
    public static class QueueHelper
    {
        public static int GenerateNextQueueNumber(DateTime date, VisitRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            return repository.GetNextQueueNumber(date.Date);
        }

        public static int GenerateNextQueueNumber(DateTime date, VisitRepository repository, MySqlConnection connection, MySqlTransaction transaction)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            return repository.GetNextQueueNumber(date.Date, connection, transaction);
        }
    }
}
