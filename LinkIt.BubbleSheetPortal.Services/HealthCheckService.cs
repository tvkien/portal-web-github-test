using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Models.Configugration;
using LinkIt.BubbleSheetPortal.Models.Old.UnGroup;
using S3Library;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class HealthCheckService
    {
        private readonly IConnectionString _connectionString;

        public HealthCheckService(IConnectionString connectionString)
        {
            _connectionString = connectionString;
        }

        public List<HealthCheckResult> Check(S3Settings s3Settings)
        {
            var result = new List<HealthCheckResult>
            {
                CheckSqlServer()
            };

            return result;
        }

        private  HealthCheckResult CheckSqlServer()
        {
            var result = new HealthCheckResult
            {
                Name = "Sql Server"
            };

            try
            {
                using (var connection = new SqlConnection(_connectionString.GetLinkItConnectionString()))
                {
                    connection.Open();
                    var command = new SqlCommand("SELECT 1", connection);
                    command.ExecuteScalar();
                    result.Status = HealthCheckStatus.Healthy;
                }
            }
            catch (Exception ex)
            {
                result.Status = HealthCheckStatus.UnHealthy;
                result.Message = ex.Message;
            }
            
            return result;
        }
    }
}
