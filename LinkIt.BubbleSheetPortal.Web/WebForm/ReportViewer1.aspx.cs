using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinkIt.BubbleSheetPortal.Web.Security;
using Microsoft.Reporting.WebForms;

namespace LinkIt.BubbleSheetPortal.Web.WebForm
{
    public partial class ReportViewer1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            // Get report request
            string reportRequest = Request.Params["Report"];
            try
            {
                // Get current user
                var user = (UserPrincipal) User;

                
                if (!IsPostBack && !string.IsNullOrEmpty(reportRequest))
                {
                    // Set paramater for report server
                    linkitReportView.ProcessingMode = ProcessingMode.Remote;
                    linkitReportView.ServerReport.ReportServerUrl = new Uri(ConfigurationManager.AppSettings["ReportServer"]);
                    linkitReportView.ServerReport.ReportPath = reportRequest;

                    string serverDomain = ConfigurationManager.AppSettings["ReportServerDomain"];
                    string serverUserName = ConfigurationManager.AppSettings["ReportServerUserName"];
                    string serverPassword = ConfigurationManager.AppSettings["ReportServerPassword"];

                    linkitReportView.ServerReport.ReportServerCredentials = new ReportServerCredentials(serverUserName, serverPassword, serverDomain);

                    try
                    {
                        // Put district id for report
                        if (user != null && user.IsDistrictAdminOrPublisher && !user.IsPublisher)
                        {
                            ReportParameter parameter = new ReportParameter("DistrictID", user.DistrictId.ToString(),
                                                                            false);
                            linkitReportView.ServerReport.SetParameters(parameter);
                        }
                    }
                    catch // fail when report doesn't have parameter DistrictID
                    {
                    }

                    linkitReportView.ServerReport.Refresh();
                }
            }
            catch
            {
                errorMessage.InnerText = "Sorry. This report is not available.";
            }
        }

        #region Sub Class

        /// <summary>
        /// Credential for report server via NetworkCredential 
        /// </summary>
        class ReportServerCredentials : IReportServerCredentials
        {
            #region Constructors
            /// <summary>
            /// Initializes a new instance of the <see cref="ReportServerCredentials"/> class.
            /// </summary>
            /// <param name="userName">Name of Server user.</param>
            /// <param name="password">Ppassword of Server user.</param>
            /// <param name="domain">The domain of Server.</param>
            public ReportServerCredentials(string userName, string password, string domain)
            {
                _userName = userName;
                _password = password;
                _domain = domain;
            }
            #endregion


            #region Properties

            private string _userName;
            private string _password;
            private string _domain;

            public WindowsIdentity ImpersonationUser
            {
                get
                {
                    // Use default identity.
                    return null;
                }
            }
            
            public ICredentials NetworkCredentials
            {
                get
                {
                    // Use default identity.
                    return new NetworkCredential(_userName, _password, _domain);
                }
            }

            #endregion


            #region Public Methods
            public bool GetFormsCredentials(out Cookie authCookie, out string user, out string password, out string authority)
            {
                // Do not use forms credentials to authenticate.
                authCookie = null;
                user = password = authority = null;
                return false;
            }
            #endregion
        }
        #endregion
    }
}