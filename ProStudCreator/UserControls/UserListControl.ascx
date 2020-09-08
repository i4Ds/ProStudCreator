<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserListControl.ascx.cs" Inherits="ProStudCreator.UserListControl" %>
<asp:GridView ID="UserGrid" ItemType="ProStudCreator.UserRowElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="UserRowClick" OnRowDataBound="UserGrid_RowDataBound" AllowSorting="True" OnSorting="UserGrid_Sorting">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="name" ItemStyle-Width="100%"/>
        <asp:BoundField DataField="Mail" HeaderText="Mail" SortExpression="mail"/>
        <asp:BoundField DataField="DepartmentName" HeaderText="Institut" SortExpression="department"/>
        <asp:CheckBoxField HeaderText="Darf Haupt- betreuer sein" DataField="CanBeAdvisor1" SortExpression="canbeadvisor1"/>
        <asp:CheckBoxField HeaderText="Darf Excel Exportieren" DataField="CanExportExcel" SortExpression="canexportexcel"/>
        <asp:CheckBoxField HeaderText="Darf Projekte Veröffentlichen" DataField="CanPublishProject" SortExpression="canpublishproject"/>
        <asp:CheckBoxField HeaderText="Admin Seite" DataField="CanVisitAdminPage" SortExpression="canvisitadminpage"/>
        <asp:CheckBoxField HeaderText="Alle Projekte Sehen" DataField="CanSeeAllProjectsInProgress" SortExpression="canseeallprojectsinprogress"/>
        <asp:CheckBoxField HeaderText="Alle Projekte Bearbeiten" DataField="CanEditAllProjects" SortExpression="caneditallprojects"/>
        <asp:CheckBoxField HeaderText="Darf Projekte Einreichen" DataField="CanSubmitAllProjects" SortExpression="cansubmitallprojects"/>
        <asp:CheckBoxField HeaderText="Darf Projekte Reservieren" DataField="CanReserveProjects" SortExpression="canreserveprojects"/>
        <asp:CheckBoxField HeaderText="Aktiv" DataField="IsActive" SortExpression="isactive"/>
        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <a title="User bearbeiten" class="btn btn-default btnHeight glyphicon glyphicon-pencil" href="UserEditPage?id=<%# Item.Id %>"></a>
            </ItemTemplate>
        </asp:TemplateField>
    </Columns>
    <EditRowStyle BackColor="#2461BF"/>
    <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"/>
    <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White"/>
    <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center"/>
    <RowStyle BackColor="#EFF3FB"/>
    <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333"/>
    <SortedAscendingCellStyle BackColor="#F5F7FB"/>
    <SortedAscendingHeaderStyle BackColor="#6D95E1"/>
    <SortedDescendingCellStyle BackColor="#E9EBEF"/>
    <SortedDescendingHeaderStyle BackColor="#4870BE"/>
</asp:GridView>