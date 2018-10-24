<%@ Page Title="IP5/IP6 Projekte" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Projectlist.aspx.cs" Inherits="ProStudCreator.Projectlist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script type="text/javascript">
        function confirmSaving(message) {
            return confirm(message);
        }
    </script>
    <div class="well usernSettings">
        <div class="col-sm-3">
            <asp:DropDownList runat="server" DataValueField="Id" DataTextField="Name" ID="dropSemester" AutoPostBack="true" CssClass="form-control"></asp:DropDownList>
        </div>
        <div class="col-sm-1"></div>
        <div class="radioButtonSettings marginProject">
            <asp:RadioButtonList ID="whichOwner" runat="server" RepeatDirection="Horizontal" AutoPostBack="true" CssClass="col-sm-5" TextAlign="Right">
                <asp:ListItem Value="AllProjects">Veröffentlichte Projekte</asp:ListItem>
                <asp:ListItem Value="OwnProjects" Selected="True">Nur eigene Projekte</asp:ListItem>
            </asp:RadioButtonList>
        </div>
        <div class="col-sm-3 input-group">
            <asp:TextBox placeholder="Student..." runat="server" class="form-control" ClientIDMode="Static" id="filterText" name="filterText"></asp:TextBox>
            <span class="input-group-btn">
            <asp:Button runat="server" OnClick="FilterButton_Click" class="btn" id="filterBtn" Text="Suchen"/>
            </span>
        </div>
        <br/>
        <hr/>
        <div class="well" style="background-color: #ffffff; margin-top: 10px;">
            <asp:GridView ID="AllProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ProjectRowClick" OnRowDataBound="AllProjects_RowDataBound" AllowSorting="True" OnSorting="AllProjects_Sorting">
                <%--<AlternatingRowStyle BackColor="White" />--%>
                <Columns>
                    <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" HtmlEncode="false" ItemStyle-Wrap="false"/>
                    <asp:BoundField DataField="Institute" HeaderText="Institut" SortExpression="Institute"/>
                    <asp:BoundField DataField="ProjectNr" HeaderText="#" SortExpression="ProjectNr"/>
                    <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="projectName" ItemStyle-Width="100%"/>
                    <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="P5"/>
                    <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="P6"/>
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Themen" ItemStyle-Width="20px"/>
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px"/>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a  class="btn btn-default btnHeight glyphicon glyphicon-info-sign" href="ProjectInfoPage?id=<%# Item.id %>"></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a title="Projekt bearbeiten" class="btn btn-default btnHeight glyphicon glyphicon-pencil" href="AddNewProject?id=<%# Item.id %>"></a>
                        </ItemTemplate>
                    </asp:TemplateField> 
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <a title="PDF anzeigen" class="btn btn-default btnHeight glyphicon glyph-pdf" href="PDF?dl=false&id=<%# Item.id %>"></a>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="deleteProjectButton" ToolTip="Projekt löschen" CommandName="deleteProject" OnClientClick="return confirm('Wollen Sie dieses Projekt wirklich löschen?');" CommandArgument="<%# Item.id %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                        </ItemTemplate>
                    </asp:TemplateField>
                    <asp:TemplateField ItemStyle-Wrap="false">
                        <ItemTemplate>
                            <asp:LinkButton runat="server" ID="submitBtn" title="Projekt einreichen" class="btn btn-default btnHeight greenButton"  OnClientClick="return confirmSaving('Dieses Projekt einreichen?');" CommandArgument="<%# Item.id %>" CommandName="submitProject">Einreichen</asp:LinkButton>
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
        </div>
        <div style="font-size: 70%">
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#CEECF5"/>
            <%--
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight"/>--%> = In Bearbeitung&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#A9F5A9"/>
            = Veröffentlicht&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#F5A9A9"/>
            = Abgelehnt&nbsp;&nbsp;&nbsp;&nbsp;
            <asp:Button runat="server" Enabled="false" CssClass="btn btn-default btnHeight" BackColor="#ffcc99"/>
            = Eingereicht&nbsp;&nbsp;&nbsp;&nbsp;
        </div>
        <div style="margin-top: 16px;">
            <asp:Button runat="server" ID="NewProject" CssClass="btn btn-default buttonFont" Text="Neues Projekt" OnClick="NewProject_Click"/>
            <asp:Button runat="server" ID="AllProjectsAsPDF" CssClass="btn btn-default buttonFont pdf" Text="Projekte als PDF" OnClick="AllProjectsAsPDF_Click"/>
            <asp:Button runat="server" ID="AllProjectsAsExcel" CssClass="btn btn-default buttonFont" Text="Projektliste in Excel" OnClick="AllProjectsAsExcel_Click"/>
        </div>
    </div>
</asp:Content>