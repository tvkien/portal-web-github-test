using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Data.Extensions
{
    public static class BulkExtensions
    {
        public static IEnumerable<T> BulkCopy<T>(
            this string connectionString,
            List<TempTableCreation> tempTableCreations,
            string finalizeProcedure,
            object inputParams = null,
            int timeout = 30,
            CommandType finalizeType = CommandType.StoredProcedure) where T : new()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(string.Empty, connection))
                {
                    foreach (var tempTable in tempTableCreations)
                    {
                        cmd.CommandText = tempTable.TempTableNameCreateScript;
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandTimeout = timeout;
                        cmd.ExecuteNonQuery();

                        using (var bulkCopy = new SqlBulkCopy(connection))
                        {
                            bulkCopy.DestinationTableName = tempTable.TempTableName;

                            if (tempTable.TempTableData?.Count() > 0)
                            {
                                using (DataTable dataTable = MakeDataTableFromListObject(tempTable.TempTableData))
                                {
                                    bulkCopy.WriteToServer(dataTable);
                                }
                            }
                        }
                    }

                    cmd.CommandText = finalizeProcedure;
                    cmd.CommandType = finalizeType;
                    cmd.CommandTimeout = timeout;

                    if (inputParams != null)
                    {
                        AssignParameter(cmd, inputParams);
                    }

                    using (var adap = new SqlDataAdapter(cmd))
                    {
                        DataSet finalizeResult = new DataSet();
                        adap.Fill(finalizeResult);
                        return finalizeResult?.Tables.Count > 0
                            ? ConvertTableToArrayOfObject<T>(finalizeResult.Tables[0])
                            : (new T[0]);
                    }
                }
            }
        }

        public static IEnumerable<T> Query<T>(
            this string connectionString,
            string finalizeProcedure,
            object inputParams = null,
            int timeout = 30,
            CommandType finalizeType = CommandType.StoredProcedure) where T : new()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(string.Empty, connection))
                {
                    cmd.CommandText = finalizeProcedure;
                    cmd.CommandType = finalizeType;
                    cmd.CommandTimeout = timeout;

                    if (inputParams != null)
                    {
                        AssignParameter(cmd, inputParams);
                    }

                    using (var adap = new SqlDataAdapter(cmd))
                    {
                        DataSet finalizeResult = new DataSet();
                        adap.Fill(finalizeResult);
                        return finalizeResult?.Tables.Count > 0
                            ? ConvertTableToArrayOfObject<T>(finalizeResult.Tables[0])
                            : (new T[0]);
                    }
                }
            }
        }

        private static IEnumerable<T> ConvertTableToArrayOfObject<T>(DataTable dataTable) where T : new()
        {
            if (dataTable.Rows.Count == 0)
            {
                return new T[0];
            }
            var mapping = GetDataTableToObjectMapping(dataTable, typeof(T));
            return dataTable.Rows.Cast<DataRow>().Select(c => BindDataRowToObject<T>(c, mapping)).ToArray();
        }

        private static List<PropertyMapToColumnNameDto> GetDataTableToObjectMapping(DataTable dataTable, Type type)
        {
            return (from prop in type.GetProperties()
                    join column in dataTable.Columns.Cast<DataColumn>()
                    on prop.Name.ToLower() equals column.ColumnName.ToLower()
                    select new PropertyMapToColumnNameDto()
                    {
                        PropertyInfo = prop,
                        ColumnName = column.ColumnName
                    })
                 .ToList();
        }

        private static T BindDataRowToObject<T>(DataRow dataRow, List<PropertyMapToColumnNameDto> mapping) where T : new()
        {
            var theObject = new T();
            var matchProps = mapping.Select(c => new
            {
                c.PropertyInfo,
                value = dataRow[c.ColumnName]
            }).ToList();
            foreach (var matchProp in matchProps)
            {
                if (matchProp.value != DBNull.Value && matchProp.value != null)
                    matchProp.PropertyInfo.SetValue(theObject, Convert.ChangeType(matchProp.value, matchProp.PropertyInfo.PropertyType));
            }
            return theObject;
        }

        private static void AssignParameter(SqlCommand cmd, object inputParams)
        {
            inputParams.GetType().GetProperties().Select(c => new { paramName = $"@{c.Name}", val = c.GetValue(inputParams) })
             .ToList().ForEach(param =>
             {
                 cmd.Parameters.Add(new SqlParameter(param.paramName, param.val));
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

    public class PropertyMapToColumnNameDto
    {
        public PropertyInfo PropertyInfo { get; internal set; }
        public string ColumnName { get; internal set; }
    }

    public class TempTableCreation
    {
        public string TempTableName { get; set; }
        public string TempTableNameCreateScript { get; set; }
        public IEnumerable<object> TempTableData { get; set; }
    }
}
