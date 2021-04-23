<%@ Page Title="Admin Bereich" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="True" CodeBehind="AdminPage.aspx.cs" Inherits="ProStudCreator.AdminPage" %>
<%@ Register TagPrefix="UserControl" TagName="ProjectList" Src="~/UserControls/ProjectListControl.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="UserList" Src="~/UserControls/UserListControl.ascx" %>
<%@ Register TagPrefix="UserControl" TagName="ExpertList" Src="~/UserControls/ExpertListControl.ascx" %>

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
                        <asp:RadioButtonList runat="server" ID="radioSelectedProjects" RepeatDirection="Horizontal" OnSelectedIndexChanged="RadioSelectedProjects_OnSelectedIndexChanged" AutoPostBack="True" CssClass="col-sm-6" TextAlign="Right">
                            <asp:ListItem Value="toPublish">Projekte zur Freigabe</asp:ListItem>
                            <asp:ListItem Value="inProgress">Projekte in Bearbeitung</asp:ListItem>
                            <asp:ListItem Value="allProjects">Alle Projekte</asp:ListItem>
                        </asp:RadioButtonList>
                    </div>
                    <div style="text-align: right;">
                        <asp:Button runat="server" ID="btnAdminProjectsCollapse" CssClass="btn btn-default btnHeight" Text="◄" OnClick="BtnAdminProjectsCollapse_OnClick" />
                    </div>
                    <br />
                    <div id="DivAdminProjectsCollapsable" runat="server" visible="False">
                        <div class="well" style="background-color: #ffffff">
                            <UserControl:ProjectList ID="ProjectGrid" runat="server" ShowModificationDate="true" />
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
                <asp:PostBackTrigger ControlID="btnGradeExport"/>
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
                        <asp:Button runat="server" ID="btnGradeExport" OnClick="BtnGradeExport_Click" CssClass="btn btn-default" Text="Noten" />
                        <asp:Button runat="server" ID="btnBillingExport" OnClick="BtnBillingExport_Click" CssClass="btn btn-default" Text="Verrechnung" />
                        <asp:Button runat="server" ID="btnMarketingExport" OnClick="BtnMarketingExport_OnClick" CssClass="btn btn-default" Text="Export"></asp:Button>                        
                        </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="well newProjectSettings" runat="server" id="DivAdminUsers">
        <asp:UpdatePanel runat="server" ID="UpdateAdminUsers">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="BtnAdminUsersCollapse" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Users" CssClass="col-sm-4"></asp:Label>
                <div style="text-align: right;">
                    <asp:Button runat="server" ID="BtnAdminUsersCollapse" CssClass="btn btn-default btnHeight" Text="◄" OnClick="BtnAdminUsersCollapse_OnClick" />
                </div>
                <br />
                <div id="DivAdminUsersCollapsable" runat="server" visible="False">
                    <div class="well" style="background-color: #ffffff">
                        <UserControl:UserList ID="UserList" runat="server" />
                    </div>
                    <div>
                        <asp:Button runat="server" ID="NewUser" CssClass="btn btn-default buttonFont" Text="Neuer User" OnClick="NewUser_Click"/>
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <div class="well newProjectSettings" runat="server" id="DivAdminExperts">
        <asp:UpdatePanel runat="server" ID="UpdateAdminExperts">
            <Triggers>
                <asp:AsyncPostBackTrigger ControlID="BtnAdminExpertsCollapse" EventName="Click" />
            </Triggers>
            <ContentTemplate>
                <asp:Label runat="server" Font-Size="24px" Height="50px" Text="Experts" CssClass="col-sm-4"></asp:Label>
                <div style="text-align: right;">
                    <asp:Button runat="server" ID="BtnAdminExpertsCollapse" CssClass="btn btn-default btnHeight" Text="◄" OnClick="BtnAdminExpertsCollapse_OnClick" />
                </div>
                <br />
                <div id="DivAdminExpertsCollapsable" runat="server" visible="False">
                    <div class="well" style="background-color: #ffffff">
                        <UserControl:ExpertList ID="ExpertList" runat="server" />
                    </div>
                    <div>
                        <asp:Button runat="server" ID="NewExpert" CssClass="btn btn-default buttonFont" Text="Neuer Expert" OnClick="NewExpert_Click"/>
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
                                <a href="mailto:simon.beck@fhnw.ch" target="_blank">Simon Beck</a>,
                                <a href="mailto:dominik.gruntz@fhnw.ch" target="_blank">Dominik Gruntz</a>
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
