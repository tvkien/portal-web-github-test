using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace GenericLog
{
    public class AuroraDBContext
    {
        private MySqlConnection _conn;
        public AuroraDBContext(MySqlConnection conn)
        {
            _conn = conn;
        }

        public void AddPortalLog(LogViewModel dto)
        {
            try
            {
                string query = BuildInsertQuery(dto);
                //var dayConfig = ConfigurationManager.AppSettings["DayToDeletePortalLog"];
                //var day = !string.IsNullOrEmpty(dayConfig) ? int.Parse(dayConfig) : 10;
                //var date = DateTime.UtcNow.AddDays(day * -1);

                //string deleteQuery = "DELETE FROM PortalLog WHERE Date < '" + date.ToString("yyyy-MM-dd HH:mm:ss") + "'";
                if (!string.IsNullOrEmpty(query))
                {
                    MySqlCommand command = new MySqlCommand(query, _conn);
                    //MySqlCommand commandDelete = new MySqlCommand(deleteQuery, _conn);
                    _conn.Open();
                    command.ExecuteNonQuery();
                    //commandDelete.ExecuteNonQuery();
                    _conn.Close();
                }
            }
            catch (Exception ex)
            {
            }
        }

        private string BuildInsertQuery(LogViewModel dto)
        {
            dto.Date = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            var query = "INSERT INTO PortalLog(";
            var names = new List<string>();
            var values = new List<string>();
            string valueStr = string.Empty;

            foreach (var item in dto.GetType().GetProperties())
            {
                names.Add(item.Name);
                var value = dto.GetType().GetProperty(item.Name).GetValue(dto, null);
                valueStr = "'" + (value != null ? value.ToString() : string.Empty) + "'";
                values.Add(valueStr);
            }
            names.RemoveAt(0);
            values.RemoveAt(0);
            query += string.Join(",", names) + ") VALUES(" + string.Join(",", values) + ")";
            return query;
        }

        public void AddUserLogoutLog(UserLogOutModel dto)
        {
            try
            {
                dto.DateTime = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
                var query = "INSERT INTO PortalUserLogOutLog(";
                var names = new List<string>();
                var values = new List<string>();
                var valueStr = string.Empty;

                foreach (var item in dto.GetType().GetProperties())
                {
                    names.Add(item.Name);
                    var value = dto.GetType().GetProperty(item.Name).GetValue(dto, null);
                    valueStr = "'" + (value != null ? value.ToString() : string.Empty) + "'";
                    values.Add(valueStr);
                }
                names.RemoveAt(0);
                values.RemoveAt(0);
                query += string.Join(",", names) + ") VALUES(" + string.Join(",", values) + ")";

                if (!string.IsNullOrEmpty(query))
                {
                    MySqlCommand command = new MySqlCommand(query, _conn);
                    _conn.Open();
                    command.ExecuteNonQuery();
                    _conn.Close();
                }
            }
            catch (Exception)
            {

            }
        }
    }
}
