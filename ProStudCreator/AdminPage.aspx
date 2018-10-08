<%@ Page Title="Admin Bereich" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="AdminPage.aspx.cs" Inherits="ProStudCreator.AdminPage" %>

<asp:Content ID="AdminPageContent" ContentPlaceHolderID="MainContent" runat="server">
<script type="text/javascript">
    function confirmSaving(message) {
        return confirm(message);
    }
</script>
    <div class="well newProjectSettings" runat="server" visible="False">
        <%-- Admin-Todos--%>
        <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Diese Seite ist noch in der Entwicklung!" ForeColor="red"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <asp:GridView ID="GVTasks" ItemType="ProStudCreator.ProjectSingleTask" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" Width="100%">
                <Columns>
                    <asp:BoundField DataField="project" HeaderText="Projekt" SortExpression="Project" ItemStyle-Width="60%" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseExpert" HeaderText="Tasks" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseDate" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseRoom" ItemStyle-Width="20px" />
                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskPayExpert" ItemStyle-Width="20px" />
                </Columns>
                <EditRowStyle BackColor="#2461BF" />
                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                <RowStyle BackColor="#EFF3FB" />
                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                <SortedDescendingHeaderStyle BackColor="#4870BE" />
            </asp:GridView>
        </div>
    </div>
    <asp:PlaceHolder ID="AdminView" runat="server">
        <div class="well adminSettings" runat="server" id="DivAdminProjects">
            <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Admin Projekte" CssClass="col-sm-3"></asp:Label>
            <asp:UpdatePanel runat="server" ID="updateAdminProjects">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="radioSelectedProjects" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="btnAdminProjectsCollapse" EventName="Click" />
                </Triggers>
                <ContentTemplate>
                    <div class="radioButtonSettingsAdmin" runat="server" id="divRadioProjects">
                        <asp:RadioButtonList runat="server" ID="radioSelectedProjects" RepeatDirection="Horizontal" OnSelectedIndexChanged="RadioSelectedProjects_OnSelectedIndexChanged" AutoPostBack="True" CssClass="col-sm-5" TextAlign="Right">
                            <asp:ListItem Value="toPublish">Projekte zur Freigabe</asp:ListItem>
                            <asp:ListItem Value="inProgress">Projekte in Bearbeitung</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div style="text-align: right;">
                        <asp:Button runat="server" ID="btnAdminProjectsCollapse" CssClass="btn btn-default btnHeight" Text="◄" OnClick="BtnAdminProjectsCollapse_OnClick" />
                    </div>
                    <br />
                    <div id="DivAdminProjectsCollapsable" runat="server" visible="False">
                        <div class="well" style="background-color: #ffffff">
                            <asp:GridView ID="CheckProjects" ItemType="ProStudCreator.ProjectSingleElement" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="False" OnRowCommand="ProjectRowClick" OnRowDataBound="CheckProjects_RowDataBound" ViewStateMode="Disabled">
                                <%--<AlternatingRowStyle BackColor="White" />--%>
                                <Columns>
                                    <asp:BoundField DataField="advisorName" HeaderText="Betreuer" SortExpression="Advisor" ItemStyle-Width="200px" HtmlEncode="false" ItemStyle-Wrap="false" />
                                    <asp:BoundField DataField="Institute" HeaderText="Institut" SortExpression="Institute" />
                                    <asp:BoundField DataField="ProjectNr" HeaderText="#" SortExpression="ProjectNr"/>
                                    <asp:BoundField DataField="projectName" HeaderText="Projektname" SortExpression="projectName" ItemStyle-Width="100%" />
                                    <asp:CheckBoxField HeaderText="P5" DataField="p5" SortExpression="P5" />
                                    <asp:CheckBoxField HeaderText="P6" DataField="p6" SortExpression="P6" />
                                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType1" HeaderText="Themen" ItemStyle-Width="20px" />
                                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="projectType2" ItemStyle-Width="20px" />
                                    <asp:TemplateField ItemStyle-Wrap="false">
                                        <ItemTemplate>
                                            <a title="Projekt Informationen" class="btn btn-default btnHeight glyphicon glyphicon-info-sign" href="ProjectInfoPage?id=<%# Item.id %>"></a>
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
                                        <asp:LinkButton runat="server" ID="submitBtn" ToolTip="Projekt Einreichen" title="Projekt einreichen" class="btn btn-default btnHeight greenButton"  OnClientClick="return confirmSaving('Dieses Projekt einreichen?');" CommandName="submitProject"  CommandArgument="<%# Item.id %> ">Einreichen</asp:LinkButton>
                                    </ItemTemplate>
                                    </asp:TemplateField>
                                </Columns>
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                            </asp:GridView>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
    </asp:PlaceHolder>
    <div class="well newProjectSettings" runat="server" id="DivExcelExport">
        <asp:UpdatePanel runat="server" ID="UpdateExcelExport">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="btnExcelExportCollapse" EventName="Click" />
                <asp:PostBackTrigger ControlID="btnMarketingExport"/>
                <asp:PostBackTrigger ControlID="btnBillingExport"/>

            </Triggers>
            <ContentTemplate>
                <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Excel-Export" CssClass="col-sm-4"></asp:Label>
                <div style="text-align: right;">
                    <asp:Button runat="server" ID="btnExcelExportCollapse" CssClass="btn btn-default btnHeight" Text="◄" OnClick="BtnExcelExportCollapse_OnClick" />
                </div>
                <br />
                <div class="form-group" runat="server" id="DivExcelExportCollapsable" visible="False">

                    <div class="well contentDesign form-horizontal" style="background-color: #ffffff">

                        <asp:Label runat="server" Text="Semester:" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="col-sm-3">
                            <asp:DropDownList runat="server" DataValueField="Id" DataTextField="Name" ID="SelectedSemester" AutoPostBack="false" CssClass="form-control col-sm-3 alignbottom "></asp:DropDownList>
                        </div>
                        <br />
                        <%--<br />
                        <hr />
                        <asp:Label runat="server" Text="Projekt-Spezifikation:" CssClass="control-label col-sm-3"></asp:Label>
                        <asp:RadioButtonList ID="radioProjectStart" runat="server" RepeatDirection="Vertical" AutoPostBack="false" CssClass="col-sm-3, alignbottom" TextAlign="Right">
                            <asp:ListItem Value="StartingProjects">&nbsp;Startende Projekte</asp:ListItem>
                            <asp:ListItem Value="EndingProjects" Selected="True">&nbsp;Endende Projekte</asp:ListItem>
                        </asp:RadioButtonList>--%>
                    </div>
                    
                    <div style="text-align: right;">
                        <asp:Button runat="server" ID="btnBillingExport" OnClick="BtnBillingExport_Click" CssClass="btn btn-default" Text="Verrechnung" Width="111px" />
                        <asp:Button runat="server" ID="btnMarketingExport" OnClick="BtnMarketingExport_OnClick" CssClass="btn btn-default" Text="Export"></asp:Button>                        
                        </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="well newProjectSettings">
        <asp:UpdatePanel runat="server" ID="UpdateAddInfo">
            <Triggers></Triggers>
            <ContentTemplate>
                <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Zusätzliche Informationen" CssClass="col-sm-5"></asp:Label>
                <div style="text-align: right;">
                    <asp:Button runat="server" ID="btnAddInfoCollapse" CssClass="btn btn-default btnHeight" Text="▼" OnClick="BtnAddInfoCollapse_OnClick" />
                </div>
                <br />
                <div runat="server" id="DivAddInfoCollapsable" visible="true">
                    <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
                        <div class="form-group">
                            <asp:Label runat="server" Text="GitHub-Projekte:" CssClass="control-label col-sm-3"></asp:Label>
                            <asp:Label runat="server" CssClass="col-sm-3 alignbottom">
                            <a href="https://github.com/i4Ds/ProStudCreator" target="_blank">ProStud (Diese Seite)</a>
                            </asp:Label>
                            <br />
                            <br />
                            <div class="col-sm-3"></div>
                            <asp:Label runat="server" CssClass="col-sm-3 alignbottom">
                            <a href="https://github.com/i4Ds/ProDist" target="_blank">ProDist (Projekt-Zuteilung)</a>
                            </asp:Label>
                        </div>
                        <hr />
                        <div class="form-group">
                            <asp:Label runat="server" Text="Server-Infos:" CssClass="control-label col-sm-3"></asp:Label>
                            <asp:Label runat="server" CssClass="col-sm-3 alignbottom">server1085.cs.technik.fhnw.ch</asp:Label>
                        </div>
                        <hr />
                        <div class="form-group">
                            <asp:Label runat="server" Text="Ansprechpersonen:" CssClass="control-label col-sm-3"></asp:Label>
                            <asp:Label runat="server" CssClass="col-sm-3 alignbottom">
                                <a href="mailto:simon.felix@fhnw.ch" target="_blank">Simon Felix</a>,
                                <a href="mailto:jonas.suter@fhnw.ch" target="_blank">Jonas Suter</a>, 
                                <a href="mailto:stephen.randles@fhnw.ch" target="_blank">Stephen Randles</a>
                            </asp:Label>
                        </div>
                        <hr />
                        <div class="form-group">
                            <asp:Label runat="server" Text="Semesterdaten:" CssClass="control-label col-sm-3"></asp:Label>
                            <asp:Label runat="server" CssClass="col-sm-3 alignbottom">
                            <a href="https://www.fhnw.ch/de/studium/technik/termine" target="_blank">Termine & Stundenpläne</a>
                            </asp:Label>
                            <div class="col-sm-3"></div>
                            <asp:GridView ID="gvDates" CssClass="col-sm-3" runat="server" CellPadding="4" ForeColor="#333333" GridLines="None" AutoGenerateColumns="True" Width="100%">
                                <%--<Columns>
                                    <asp:BoundField DataField="project" HeaderText="Projekt" SortExpression="Project" ItemStyle-Width="60%" />
                                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseExpert" HeaderText="Tasks" ItemStyle-Width="20px" />
                                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseDate" ItemStyle-Width="20px" />
                                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskOrganiseRoom" ItemStyle-Width="20px" />
                                    <asp:ImageField ControlStyle-CssClass="img-rounded imageHeight" DataImageUrlField="taskPayExpert" ItemStyle-Width="20px" />
                                </Columns>--%>
                                <EditRowStyle BackColor="#2461BF" />
                                <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                <PagerStyle BackColor="#2461BF" ForeColor="White" HorizontalAlign="Center" />
                                <RowStyle BackColor="#EFF3FB" />
                                <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                <SortedAscendingCellStyle BackColor="#F5F7FB" />
                                <SortedAscendingHeaderStyle BackColor="#6D95E1" />
                                <SortedDescendingCellStyle BackColor="#E9EBEF" />
                                <SortedDescendingHeaderStyle BackColor="#4870BE" />
                            </asp:GridView>

                        </div>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
</asp:Content>
