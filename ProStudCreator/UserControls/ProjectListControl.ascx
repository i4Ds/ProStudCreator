<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectListControl.ascx.cs" Inherits="ProStudCreator.ProjectListControl" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectTopicImage" Src="~/UserControls/ProjectTopicImage.ascx" %>

<asp:GridView ID="ProjectGrid" ItemType="ProStudCreator.ProjectRowElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ProjectRowClick" OnRowDataBound="ProjectGrid_RowDataBound" AllowSorting="True" OnSorting="ProjectGrid_Sorting">
    <%--<AlternatingRowStyle BackColor="White" />--%>
    <Columns>
        <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" HtmlEncode="false" ItemStyle-Wrap="false"/>
        <asp:BoundField DataField="projectNr" HeaderText="Projektnummer" SortExpression="ProjectNr"/>
        <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="projectName" ItemStyle-Width="100%"/>
        <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="P5"/>
        <asp:CheckBoxField HeaderText="Lang" DataField="lng" SortExpression="Long"/>
        <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="P6"/>
        <asp:TemplateField HeaderText="Themen">
            <ItemTemplate>
                <UserControl:ProjectTopicImage runat="server" ID="ProjectTopic1"></UserControl:ProjectTopicImage>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField>
            <ItemTemplate>
                <UserControl:ProjectTopicImage runat="server" ID="ProjectTopic2"></UserControl:ProjectTopicImage>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:BoundField DataField="modDateString" HeaderText="Zuletzt geändert" SortExpression="modDate" />
        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <a title="PDF anzeigen" class="btn btn-default btnHeight glyphicon glyph-pdf" target="_blank" href="PDF?dl=false&id=<%# Item.id %>"></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <a  class="btn btn-default btnHeight glyphicon glyphicon-info-sign" href="ProjectInfoPage?id=<%# Item.id %>"></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <a title="Projekt bearbeiten" class="btn btn-default btnHeight glyphicon glyphicon-pencil" href="ProjectEditPage?id=<%# Item.id %>"></a>
            </ItemTemplate>
        </asp:TemplateField>
        <asp:TemplateField ItemStyle-Wrap="false">
            <ItemTemplate>
                <asp:LinkButton runat="server" ID="deleteProjectButton" ToolTip="Projekt löschen" CommandName="deleteProject" OnClientClick="return confirm('Wollen Sie dieses Projekt wirklich löschen?');" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
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