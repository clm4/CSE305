<%@ Page Title="Home Page" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebApplication1._Default" %>
<%@ Register assembly="DatePickerControl" namespace="DatePickerControl" tagprefix="cc1" %>


<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>
<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>
        Welcome to ABCTrade!
    </h2>
    <p>
        Here is the 
    </p>
    <p>
        You can also find <a href="http://go.microsoft.com/fwlink/?LinkID=152368&amp;clcid=0x409"
            title="MSDN ASP.NET Docs">documentation on ASP.NET at MSDN</a>.
    </p>
    <asp:TextBox runat="server" ID="TickerSymbol" Value="Ticker Symbol" onfocus="value=''"></asp:TextBox><br />
    <asp:Button runat="server" OnClick="FindStockData" Text="Find Prices"></asp:Button><br />
    
    Start Date &nbsp<cc1:DatePicker ID="StartPicker" runat="server" /><br />
    End Date &nbsp<cc1:DatePicker ID="EndPicker" runat="server" /><br />

    <%if (this.HasData && TableRows.Count > 0)
      { %>
      <table Id="StockTable" border="1" cellpadding="5">
      <tr>
      <%for (int i = 0; i < TableRows[0].Cells.Count; i++)
        { %>
        <td>
        <%=TableColumnNames[i]%>
        </td>
      <%} %>
      </tr>
      <%for (int i = 0; i < TableRows.Count; i++)
        {
            TableRow row = TableRows[i];%>
            <tr>
            <% for (int j = 0; j < row.Cells.Count; j++)
               {%>
            <td>
            <%TableCell cell = row.Cells[j]; %>
            <%=cell.Text%>
            </td>
            <% } %>
            </tr>            
        <% } %>
      
      </table>

    
    <% } %>
    <%--<asp:Table runat="server" ID="StockTable" BorderWidth="1"></asp:Table>--%>
</asp:Content>
