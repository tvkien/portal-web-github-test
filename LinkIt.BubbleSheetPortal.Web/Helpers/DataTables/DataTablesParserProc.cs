using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web;


namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables
{
    public class DataTableParserProc<T> where T : class, new()
    {
        private const string IndividualSortKeyPrefix = "iSortCol_";
        private const string IndividualSortDirectionKeyPrefix = "sSortDir_";
        private const string IndividualSearcColPrefix = "mDataProp_";
        private const string IndividualSearchKeyPrefix = "bSearchable_";
        private const string IndividualSearchTextPrefix = "sSearch_";
        private const string DisplayStart = "iDisplayStart";
        private const string DisplayLength = "iDisplayLength";
        private const string ECHO = "sEcho";
        private const string AscendingSort = "asc";
        private readonly Type type;
        private readonly PropertyInfo[] properties;
        private readonly PropertyInfo[] searchableProperties;
        private readonly PropertyInfo[] sortableProperties;
        private DataTableRequest _request = null;
        public DataTableParserProc()
        {
            type = typeof(T);
            properties = type.GetProperties();
            searchableProperties = GetSearchableProperties(properties);
            sortableProperties = GetSortableProperties(properties);
        }
        private DataTableRequest Request
        {
            get
            {
             if(_request==null)
             {
                 _request = GetRequestFromHttpContext();
             }
                return _request;
            }
        }
        public int StartIndex
        {
            get { return Request.Skip; }
        }
        public int PageSize
        {
            get { return Request.Take; }
        }
        public List<string> SearchInBox
        {
            get
            {
                List<string> search = new List<string>();
                if (!string.IsNullOrEmpty(Request.AllSearch))
                {
                    search = GetSearchTerms(Request.AllSearch).ToList();
                }
                return search;
            }
        }
        public string SearchInBoxXML
        {
            get
            {
                if (SearchInBox.Count == 0)
                {
                    return string.Empty;
                }
                else
                {


                    string xml = "<Search>";

                    foreach (var word in SearchInBox)
                    {
                        xml += string.Format("<word text=\"{0}\">", HttpUtility.UrlEncode(word.ToLower()));
                    }
                    xml += "</Search>";
                    return xml;
                }
            }
        }
        public string SortableColumns
        {
            get
            {
                string sortColumns = string.Empty;
                foreach (var col in Request.Columns)//Request.Columns contain both searchable and sortable columns,sortable columns has SortDirection
                {
                    if (sortableProperties.Contains(properties[col.ColumnIndex]))
                    {
                        if (!string.IsNullOrEmpty(col.SortDirection))
                        {
                            if (sortColumns.Length == 0)
                            {
                                sortColumns += properties[col.ColumnIndex].Name + " " + col.SortDirection;
                            }
                            else
                            {
                                sortColumns += ", " + properties[col.ColumnIndex].Name + " " + col.SortDirection;
                            }
                        }
                    }
                }
                return sortColumns; 
            }
        }
        public string SearchableColumns
        {
            get
            {
                string searchColumns = string.Empty;

                foreach (var searchColumn in Request.Columns)//sortable column always be searchable 
                {
                    searchColumns += string.Format("[{0}]", properties[searchColumn.ColumnIndex].Name);
                }
                return searchColumns;
            }
        }

        public FormatedList Parse(IQueryable<T> queriable,int totalRecords,bool forceSearchInString = false)
        {            
            var list = new FormatedList();
            list.Import(properties.Select(x => x.Name).ToArray());
            list.sEcho = this.Request.TableEcho;

            var setData = queriable.Select(x => new PreFilterTotal<T> { Val = x });
            var results = setData.ToList();

            if (!results.Any())
            {
                list.aaData = new List<T>().Select(SelectProperties);
                return list;
            }

            list.aaData = results.Select(x => x.Val).Select(SelectProperties).ToList();
            list.iTotalDisplayRecords = totalRecords;
            list.iTotalRecords = totalRecords;

            return list;
        }        

        private Expression<Func<PreFilterTotal<T>, bool>> IndividualPropertySearch(DataTableRequest request, bool forceSearchInString)
        {
            var paramExpression = Expression.Parameter(typeof(PreFilterTotal<T>), "val");
            Expression compoundExpression = Expression.Constant(true);

            foreach (var dataTableColumn in request.Columns)
            {
                var query = dataTableColumn.SearchTerm;

                if (string.IsNullOrEmpty(query))
                    continue;

                var searchColumn = dataTableColumn.ColumnIndex;
                if (searchColumn < 0 || searchColumn >= properties.Length)
                    continue;

                var propertyToSearch = properties[searchColumn];
                if (!searchableProperties.Contains(propertyToSearch))
                    continue;

                var expression = ParseFactory.GetParser(query, propertyToSearch, paramExpression, forceSearchInString).GetSearchExpression();
                compoundExpression = Expression.And(compoundExpression, expression);
            }
            var result = Expression.Lambda<Func<PreFilterTotal<T>, bool>>(compoundExpression, paramExpression);
            return result;
        }

        private Expression<Func<PreFilterTotal<T>, bool>> ApplyGenericSearch(DataTableRequest request, bool forceSearchInString)
        {
            string search = request.AllSearch;


            if (string.IsNullOrWhiteSpace(search) || properties.Length == 0)
                return x => true;

            var paramExpression = Expression.Parameter(typeof(PreFilterTotal<T>), "val");
            var searchValues = GetSearchTerms(search).ToList();

            Expression compoundExpression = null;
            foreach (var searchValue in searchValues)
            {
                if (string.IsNullOrEmpty(searchValue)) continue;

                Expression singleValue = Expression.Constant(false);
                var tempPropertyQueries = GetSearchQueries(searchValue, paramExpression, forceSearchInString).ToList();
                List<Expression> propertyQueries = new List<Expression>();

                var searchableColumns = request.Columns.OrderBy(c => c.ColumnIndex).Select(c => c.ColumnIndex).ToList();
                for (int i = 0; i < tempPropertyQueries.Count(); i++)
                {
                    if (searchableColumns.Contains(i))
                    {
                        propertyQueries.Add(tempPropertyQueries[i]);
                    }
                }

                foreach (var propertyQuery in propertyQueries)
                {
                    singleValue = Expression.Or(singleValue, propertyQuery);
                }

                //compoundExpression = Expression.And(compoundExpression, singleValue);
                if (compoundExpression == null)
                {
                    compoundExpression = singleValue;
                }
                else
                {
                    compoundExpression = Expression.Or(compoundExpression, singleValue);
                }

            }
            // Hot fix Exception Production
            //Rubric/GetListRubricsNew?sEcho=47&iColumns=12&sColumns=SubjectName%2CGradeName%2CTestBankName%2CAuthor%2CTestName%2CFileName%2CDistrictId%2CGradeId%2CSubjectId%2CRubricId%2CTestId%2CRubricKey&iDisplayStart=0&iDisplayLength=10&mDataProp_0=0&mDataProp_1=1&mDataProp_2=2&mDataProp_3=3&mDataProp_4=4&mDataProp_5=5&mDataProp_6=6&mDataProp_7=7&mDataProp_8=8&mDataProp_9=9&mDataProp_10=10&mDataProp_11=11&sSearch=%22&bRegex=false&sSearch_0=&bRegex_0=false&bSearchable_0=true&sSearch_1=&bRegex_1=false&bSearchable_1=true&sSearch_2=&bRegex_2=false&bSearchable_2=true&sSearch_3=&bRegex_3=false&bSearchable_3=true&sSearch_4=&bRegex_4=false&bSearchable_4=true&sSearch_5=&bRegex_5=false&bSearchable_5=false&sSearch_6=&bRegex_6=false&bSearchable_6=false&sSearch_7=&bRegex_7=false&bSearchable_7=false&sSearch_8=&bRegex_8=false&bSearchable_8=false&sSearch_9=&bRegex_9=false&bSearchable_9=false&sSearch_10=&bRegex_10=false&bSearchable_10=false&sSearch_11=&bRegex_11=false&bSearchable_11=false&iSortCol_0=4&sSortDir_0=desc&iSortingCols=1&bSortable_0=true&bSortable_1=true&bSortable_2=true&bSortable_3=true&bSortable_4=true&bSortable_5=false&bSortable_6=false&bSortable_7=false&bSortable_8=false&bSortable_9=false&bSortable_10=false&bSortable_11=false&SubjectName=Electives&GradeId=33&DistrictId=undefined&TestBankName=french+1&TestName=&Author=&_=1421093130124
            if (compoundExpression == null)
                compoundExpression = Expression.Constant(true);

            var result = Expression.Lambda<Func<PreFilterTotal<T>, bool>>(compoundExpression, paramExpression);
            return result;
        }

        private IEnumerable<Expression> GetSearchQueries(string searchExpression, ParameterExpression paramExpression, bool forceSearchInString)
        {
            return properties
                .Where(x => searchableProperties.Contains(x))
                .Select(property => ParseFactory.GetParser(searchExpression, property, paramExpression, forceSearchInString).GetSearchExpression());
        }

        private IOrderedQueryable<T> ApplySort(IQueryable<T> queriable, DataTableRequest request)
        {
            IOrderedQueryable<T> orderedQueryable = null;
            foreach (var dataTableColumn in request.Columns)
            {
                var sortcolumn = dataTableColumn.ColumnIndex;
                if (sortcolumn < 0 || sortcolumn >= properties.Length)
                    continue;

                var property = properties[sortcolumn];
                if (!sortableProperties.Contains(property))
                    continue;

                var sortdir = dataTableColumn.SortDirection;
                if (string.IsNullOrEmpty(sortdir))
                    continue;

                orderedQueryable = SortOnColumn(queriable, orderedQueryable, sortcolumn, sortdir);
            }

            return orderedQueryable ?? SortOnColumn(queriable, null, 0, AscendingSort);
        }

        private IOrderedQueryable<T> SortOnColumn(IQueryable<T> queriable, IOrderedQueryable<T> orderedQueryable, int sortcolumn, string sortDirection)
        {
            var method = typeof(DataTableParserProc<T>).GetMethod("ApplyTypeSort", BindingFlags.NonPublic | BindingFlags.Instance);
            var generic = method.MakeGenericMethod(properties[sortcolumn].PropertyType);
            return (IOrderedQueryable<T>)generic.Invoke(this,
                                                        BindingFlags.NonPublic | BindingFlags.Instance,
                                                        null,
                                                        new object[] { queriable, orderedQueryable, sortcolumn, sortDirection },
                                                        null);
        }

        private DataTableRequest GetRequestFromHttpContext()
        {
            var httpRequest = new HttpRequestWrapper(HttpContext.Current.Request);
            var sortColumns = new List<DataTableColumn>();
            foreach (var key in httpRequest.Params.AllKeys.Where(x => x.StartsWith(IndividualSortKeyPrefix)))
            {
                var sortcolumn = int.Parse(httpRequest[key]);

                if (sortcolumn < 0 || sortcolumn >= properties.Length)
                    break;
                var sortdir = httpRequest[IndividualSortDirectionKeyPrefix + key.Replace(IndividualSortKeyPrefix, string.Empty)];
                sortColumns.Add(new DataTableColumn
                                    {
                                        ColumnIndex = sortcolumn,
                                        SortDirection = sortdir
                                    });
            }

            foreach (var key in httpRequest.Params.AllKeys.Where(x => x.StartsWith(IndividualSearchKeyPrefix)))
            {
                var searchable = bool.Parse(httpRequest[key]);
                
                if (!searchable)
                    continue;
                var searchCol = int.Parse(httpRequest[IndividualSearcColPrefix + key.Replace(IndividualSearchKeyPrefix, string.Empty)]);
                var searchText = httpRequest[IndividualSearchTextPrefix + key.Replace(IndividualSearchKeyPrefix, string.Empty)];
                var sortColumn = sortColumns.FirstOrDefault(x => x.ColumnIndex == searchCol);
                if(sortColumn != null)
                {
                    sortColumn.SearchTerm = searchText;
                }
                else
                {
                    sortColumns.Add(new DataTableColumn
                    {
                        ColumnIndex = searchCol,
                        SearchTerm = searchText
                    });
                }
            }

            string search = httpRequest["sSearch"];
            search = HttpUtility.UrlDecode(search);
            int echo = int.Parse(httpRequest[ECHO] ?? "1");
            int skip, take;
            int.TryParse(httpRequest[DisplayStart], out skip);
            int.TryParse(httpRequest[DisplayLength], out take);

            return new DataTableRequest
            {
                AllSearch = search,
                Skip = skip,
                Take = take,
                TableEcho = echo,
                Columns = sortColumns
            };
        }


        // ReSharper disable UnusedMember.Local - called via reflection
        private IOrderedQueryable<T> ApplyTypeSort<TSearch>(IQueryable<T> queriable, IOrderedQueryable<T> orderedQueryable, int sortcolumn, string sortdir)
        // ReSharper restore UnusedMember.Local
        {
            var paramExpr = Expression.Parameter(typeof(T), "val");
            var property = Expression.Property(paramExpr, properties[sortcolumn]);
            var propertyExpr = Expression.Lambda<Func<T, TSearch>>(property, paramExpr);

            if (string.IsNullOrEmpty(sortdir) || sortdir.Equals(AscendingSort, StringComparison.OrdinalIgnoreCase))
                orderedQueryable = orderedQueryable == null
                                       ? queriable.OrderBy(propertyExpr)
                                       : orderedQueryable.ThenBy(propertyExpr);
            else
                orderedQueryable = orderedQueryable == null
                                       ? queriable.OrderByDescending(propertyExpr)
                                       : orderedQueryable.ThenByDescending(propertyExpr);

            return orderedQueryable;
        }

        private PropertyInfo[] GetSortableProperties(IEnumerable<PropertyInfo> propertyInfos)
        {
            return propertyInfos.ToArray();
            //return propertyInfos.Where(x => x.GetCustomAttributes(typeof(SortableAttribute), false).Any()).ToArray();
        }

        private PropertyInfo[] GetSearchableProperties(IEnumerable<PropertyInfo> propertyInfos)
        {
            return propertyInfos.ToArray();
            //return propertyInfos.Where(x => x. x.GetCustomAttributes(typeof(SearchableAttribute), false).Any()).ToArray();
        }

        private Func<T, IEnumerable<object>> SelectProperties
        {
            get
            {
                return value => properties.Select(prop =>
                                                      {
                                                          var obj = prop.GetValue(value, new object[0]);

                                                          if (obj is DateTime)
                                                          {
                                                              var dateTime = (DateTime)obj;
                                                              var epoch = new DateTime(1970, 1, 1);
                                                              var span = new TimeSpan(dateTime.Ticks - epoch.Ticks);
                                                              return Convert.ToString(span.TotalMilliseconds);
                                                          }
                                                          if (obj is bool)
                                                          {
                                                              return Convert.ToString(obj);
                                                          }
                                                          if (obj is TimeZoneInfo)
                                                          {
                                                              return ((TimeZoneInfo)obj).StandardName;
                                                          }
                                                          return obj;
                                                      }).ToList();
            }
        }

        private IEnumerable<string> GetSearchTerms(string value)
        {
            var quotedList = new List<string>();

            var regex = new Regex("[^ ]*(\"([^\"]+)\")[^ ]*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
            for (var match = regex.Match(value); match.Success; match = match.NextMatch())
            {
                quotedList.Add(match.Groups[0].Value);
            }

            foreach (var quoted in quotedList)
            {
                var index = value.IndexOf(quoted, StringComparison.OrdinalIgnoreCase);
                value = value.Substring(0, index) +
                        value.Substring(index + quoted.Length, value.Length - (index + quoted.Length));
            }

            regex = new Regex("[^ ]*[^ ]+[^ ]*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
            for (var match = regex.Match(value); match.Success; match = match.NextMatch())
            {
                quotedList.Add(match.Groups[0].Value);
            }

            return quotedList.Select(x => x.Replace("\"", ""));
        }

    }
}
