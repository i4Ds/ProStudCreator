<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ExpertListControl.ascx.cs" Inherits="ProStudCreator.ExpertListControl" %>
<asp:GridView ID="ExpertGrid" ItemType="ProStudCreator.ExpertRowElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ExpertRowClick" OnRowDataBound="ExpertGrid_RowDataBound" AllowSorting="True" OnSorting="ExpertGrid_Sorting">
    <Columns>
        <asp:BoundField DataField="Name" HeaderText="Name" SortExpression="name"/>
        <asp:BoundField DataField="Mail" HeaderText="Mail" SortExpression="mail"/>
        <asp:BoundField DataField="Unternehmen" HeaderText="Unternehmen" SortExpression="unternehmen"/>
        <asp:BoundField DataField="Knowhow" HeaderText="Knowhow" SortExpression="knowhow"/>

        <asp:CheckBoxField HeaderText="Automatische Auszahlung" DataField="AutomaticPayout" SortExpression="automaticpayout"/>
        <asp:CheckBoxField HeaderText="Aktiv" DataField="IsActive" SortExpression="isactive"/>
        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <a title="Expert bearbeiten" class="btn btn-default btnHeight glyphicon glyphicon-pencil" href="ExpertEditPage?id=<%# Item.Id %>"></a>
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