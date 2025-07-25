using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Data.SqlClient;
using LinkIt.BubbleSheetPortal.Common;
using System;
using System.Reflection;
using Amazon.CodeDeploy.Model;

namespace LinkIt.BubbleSheetPortal.Data.Extensions
{
    public static class DbContextExtensions
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> dataList) where T : class
        {
            DataTable convertedTable = new DataTable();
            PropertyInfo[] propertyInfo = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo prop in propertyInfo)
            {
                convertedTable.Columns.Add(prop.Name);
            }
            foreach (T item in dataList)
            {
                var row = convertedTable.NewRow();
                for (int i = 0; i < propertyInfo.Length; i++)
                {
                    row[i] = propertyInfo[i].GetValue(item, null);
                }
                convertedTable.Rows.Add(row);
            }
            return convertedTable;
        }

        public static List<T> Query<T>(this DataContext context, SqlParameterRequest request, out IEnumerable<(string ParameterName, object Value)> outputs) where T : new()
        {

            var result = new List<T>();
            using (SqlConnection connection = new SqlConnection(context.Connection.ConnectionString))
            {
                connection.Open();

                var cmd = new SqlCommand(request.StoredName, connection);
                cmd.CommandType = CommandType.StoredProcedure;


                SqlParameter sqlParameter = new SqlParameter();

                foreach (var parameter in request.Parameters)
                {

                    sqlParameter = cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    sqlParameter.SqlDbType = parameter.SqlDbType;
                    sqlParameter.Direction = parameter.Direction;
                    
                    if (!string.IsNullOrEmpty(parameter.TypeName))
                    {
                        sqlParameter.TypeName = parameter.TypeName;
                    }
                }
                using (SqlDataReader sdr = cmd.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        T t = new T();
                        for (int inc = 0; inc < sdr.FieldCount; inc++)
                        {
                            Type type = t.GetType();
                            PropertyInfo prop = type.GetProperty(sdr.GetName(inc));
                            if (prop != null && sdr.GetValue(inc) != DBNull.Value)
                            {
                                prop.SetValue(t, sdr.GetValue(inc), null);
                            } 
                        }

                        result.Add(t);
                    }
                }

                var outputList = new List<(string ParameterName, object Value)>();
                foreach (SqlParameter parameter in cmd.Parameters)
                {
                    if (parameter.Direction == ParameterDirection.Output || parameter.Direction == ParameterDirection.InputOutput)
                    {
                        outputList.Add((parameter.ParameterName, parameter.Value));
                    }
                }
                outputs = outputList;

                connection.Close();
            }

            return result;
        }

        public static DataSet QueryMutipleTable<T>(this DataContext context, SqlParameterRequest request) where T : new()
        {
            DataSet ds = new DataSet();
            using (SqlConnection connection = new SqlConnection(context.Connection.ConnectionString))
            {
                connection.Open();
                var cmd = new SqlCommand(request.StoredName, connection);
                cmd.CommandType = CommandType.StoredProcedure;
                SqlParameter sqlParameter = new SqlParameter();
                foreach (var parameter in request.Parameters)
                {
                    sqlParameter = cmd.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
                    sqlParameter.SqlDbType = parameter.SqlDbType;
                    if (!string.IsNullOrEmpty(parameter.TypeName))
                    {
                        sqlParameter.TypeName = parameter.TypeName;
                    }
                }
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                da.Fill(ds);                
                connection.Close();
            }

            return ds;
        }
    }
}
