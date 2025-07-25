using System;
using System.Linq;
using MySql.Data.MySqlClient;

namespace GenericLog
{
    public class PortalLogRepository : IPortalLogRepository
    {
        private AuroraDBContext _context;

        public PortalLogRepository(ILogConnectionString conn)
        {
            var connection = new MySqlConnection(conn.GetAuroraPortalLogConnectionString());
            _context = new AuroraDBContext(connection);
        }
        public void Insert(LogViewModel dto)
        {
            try
            {
                _context.AddPortalLog(dto);
            }
            catch (Exception ex)
            {
            }
        }

        public void InsertUserLogoutLog(UserLogOutModel dto)
        {
            try
            {
                _context.AddUserLogoutLog(dto);
            }
            catch (Exception ex)
            {
            }
        }
    }
}
