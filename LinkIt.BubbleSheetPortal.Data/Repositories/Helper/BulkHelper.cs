using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.Helper
{
    public class BulkHelper : IBulkHelper
    {
        private readonly IConnectionString connectionStringProvider;

        public BulkHelper(IConnectionString connectionStringProvider)
        {
            this.connectionStringProvider = connectionStringProvider;
        }
        public DataSet BulkCopy(string tempTableCreateScript, string tempTableName, IEnumerable<object> objectsToBeBulked, string finalizeProcedure, params object[] inputParams)
        {
            using (SqlConnection connection =
                      new SqlConnection(connectionStringProvider.GetLinkItConnectionString()))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = tempTableCreateScript;
                    cmd.CommandType = CommandType.Text;
                    cmd.ExecuteNonQuery();

                    using (SqlBulkCopy bulkCopy = new SqlBulkCopy(connection))
                    {
                        //bulkCopy.DestinationTableName = "#unitRawData";
                        bulkCopy.DestinationTableName = tempTableName;

                        try
                        {
                            if (objectsToBeBulked?.Count() > 0)
                                using (DataTable dataTable = MakeDataTableFromListObject(objectsToBeBulked))
                                {
                                    // Write from the source to the destination.
                                    bulkCopy.WriteToServer(dataTable);
                                }
                            cmd.CommandText = finalizeProcedure;
                            cmd.CommandType = CommandType.StoredProcedure;
                            AssignParameter(cmd, inputParams);
                            using (SqlDataAdapter adap = new SqlDataAdapter(cmd))
                            {
                                DataSet finalizeResult = new DataSet();
                                adap.Fill(finalizeResult);
                                return finalizeResult;
                            }

                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
            }
        }

        public void BulkCopy(IEnumerable<object> objectsToBeBulked, string tablename)
        {
            if (objectsToBeBulked?.Count() <= 0)
            {
                return;
            }

            using (SqlConnection con = new SqlConnection(connectionStringProvider.GetLinkItConnectionString()))
            {
                con.Open();
                using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con))
                {
                    sqlBulkCopy.DestinationTableName = tablename;

                    using (DataTable dataTable = MakeDataTableFromListObject(objectsToBeBulked))
                    {
                        sqlBulkCopy.WriteToServer(dataTable);
                    }
                }

                con.Close();
            }
        }

        private void AssignParameter(SqlCommand cmd, object[] inputParams)
        {
            if (inputParams == null)
            {
                return;
            }
            if (inputParams.Length % 2 != 0)
            {
                throw new Exception("Please correct amounts of params");
            }
            inputParams?.Select((val, index) => new
            {
                val,
                index
            }).Where(c => c.index % 2 == 0)
            .ToList().ForEach(paramName =>
            {
                cmd.Parameters.Add(new SqlParameter(paramName.val.ToString(), inputParams[paramName.index + 1]));
            });
        }

        private static DataTable MakeDataTableFromListObject(IEnumerable<object> objectsToBeBulked)
        {
            DataTable newDataTable = new DataTable();
            if (objectsToBeBulked?.Count() > 0)
            {
                foreach (var property in objectsToBeBulked.ElementAt(0).GetType().GetProperties())
                {
                    var propertyType = property.PropertyType;
                    if (Nullable.GetUnderlyingType(property.PropertyType) is Type underlineType)
                    {
                        propertyType = underlineType;
                    }
                    newDataTable.Columns.Add(property.Name, propertyType);
                }
            }
            var propInfors = (from prop in objectsToBeBulked.ElementAt(0).GetType().GetProperties()
                              join b in newDataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName)
                              on prop.Name.ToLower() equals b.ToLower()
                              select prop).ToArray();

            foreach (var entity in objectsToBeBulked)
            {
                DataRow newEntry = newDataTable.NewRow();
                foreach (var prop in propInfors)
                {
                    newEntry[prop.Name] = prop.GetValue(entity) ?? DBNull.Value;
                }
                newDataTable.Rows.Add(newEntry);
            }
            return newDataTable;
        }
    }
}
