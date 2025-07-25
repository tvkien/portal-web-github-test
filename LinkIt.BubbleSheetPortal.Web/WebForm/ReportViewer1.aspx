<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ReportViewer1.aspx.cs" Inherits="LinkIt.BubbleSheetPortal.Web.WebForm.ReportViewer1" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a"
    Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>



<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script src="../Scripts/jquery-1.7.1.min.js" type="text/javascript"></script>
<script type="text/javascript">
//    var i = 1;
//    $(function () {
//        // Bug-fix on Chrome and Safari etc (webkit)
//        if ($.browser.webkit || true) {
//            // Start timer to make sure overflow is set to visible
//            setInterval(function () {
//                i++;
//                if (i == 5) alert(i);
//                var div = $('#<%=linkitReportView.ClientID %>_fixedTable > tbody > tr:last > td:last > div');
//                div.css('overflow', 'visible');
//            }, 1000);
//        }
//    });
</script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server" AsyncPostBackTimeout="1800">
        </asp:ScriptManager>
        <rsweb:ReportViewer ID="linkitReportView" runat="server" Width="100%" Height="100%" KeepSessionAlive="True">
        </rsweb:ReportViewer>
        <label runat="server" id="errorMessage"></label>
    </div>
    </form>
</body>
</html>
