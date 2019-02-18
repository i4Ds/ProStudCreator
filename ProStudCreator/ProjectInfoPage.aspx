<%@ Page Title="Projekt Information" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ProjectInfoPage.aspx.cs" Inherits="ProStudCreator.ProjectInfoPage" EnableEventValidation="false" %>

<%@ Register Assembly="AjaxControlToolkit" Namespace="AjaxControlToolkit" TagPrefix="ajax" %>
<asp:Content ID="ProjectInofpageContent" ContentPlaceHolderID="MainContent" runat="server">
    <div class="well newProjectSettings">
        <asp:Label runat="server" ID="SiteTitle" Font-Size="24px" Height="50px" Text="Projektinformationen"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" Text="Projektname:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-6">
                    <asp:TextBox runat="server" ID="ProjectTitle" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                </div>
                <div class="col-sm-3">
                    <asp:Image CssClass="img-rounded" ID="Topic1" Height="60px" runat="server"/>
                    <asp:Image CssClass="img-rounded" ID="Topic2" Height="60px" runat="server"/>
                </div>
            </div>
            <div class="form-group" style="text-align: left">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ChangeTitleDate" CssClass="col-sm-6"></asp:Label>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Studierende:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Student1Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Student2Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Betreuung:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Advisor1Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <asp:Label runat="server" ID="Advisor2Name" CssClass="col-sm-3 alignbottom"></asp:Label>
                <div runat="server" id="DivExpert">
                    <br />
                    <br />
                    <asp:Label runat="server" Text="Experte:" CssClass="control-label col-sm-3"></asp:Label>
                    <asp:Label runat="server" ID="ExpertName" CssClass="col-sm-9 alignbottom"></asp:Label>
                </div>
                <br />
                <br />
                <div runat="server" id="DivExpertAdmin">
                    <asp:Label runat="server" Text="Experte:" CssClass="control-label col-sm-3"></asp:Label>
                    <div class="col-sm-6">
                        <asp:DropDownList runat="server" ID="drpExpert" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="DrpExpert_SelectedIndexChanged"></asp:DropDownList>
                    </div>
                    <asp:Label runat="server" ID="ExpertMail" CssClass="col-sm-3 alignbottom"></asp:Label>
                </div>
            </div>
            <hr />
            <div class="form-group">
                <asp:Label runat="server" Text="Art des Projektes:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="lblProjectType" CssClass="col-sm-3 alignbottom"></asp:Label>
                <br />
                <br />
                <asp:Label runat="server" Text="Abgabe:" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="ProjectDelivery" CssClass="col-sm-3 alignbottom"></asp:Label>
                <div runat="server" id="divPresentation">
                    <br />
                    <br />
                    <asp:Label runat="server" Text="Schlusspräsentation:" CssClass="control-label col-sm-3" ID="lblProjectEndPresentation"></asp:Label>
                    <asp:Label runat="server" ID="ProjectEndPresentation" CssClass="col-sm-9 alignbottom"></asp:Label>
                </div>
                <div runat="server" id="divBachelor">
                    <br />
                    <br />
                    <asp:Label runat="server" Text="Ausstellung Bachelorthesis:" ID="lblAussstellungBachelorthese" CssClass="control-label col-sm-3"></asp:Label>
                    <asp:Label runat="server" ID="ProjectExhibition" CssClass="col-sm-3 alignbottom"></asp:Label>
                </div>
                <br />
            </div>
            <hr />

            <div class="form-group">
                <asp:Label runat="server" Text="Durchführungssprache:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpLogLanguage" AutoPostBack="true" CssClass="form-control" OnSelectedIndexChanged="drpLogLanguage_SelectedIndexChanged">
                        <asp:ListItem Text="(Bitte Auswählen)" Value="0" />
                        <asp:ListItem Text="Englisch" Value="1" />
                        <asp:ListItem Text="Deutsch" Value="2" />
                    </asp:DropDownList>
                </div>
            </div>

            <div class="form-group">
                <asp:Label runat="server" Text="Websummary:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:CheckBox runat="server" ID="cbxWebSummaryChecked" Text=" Websummary kontrolliert" CssClass="form-control" AutoPostBack="true" OnCheckedChanged="cbxWebSummaryChecked_CheckedChanged" />
                </div>
                <asp:Label runat="server" ID="lblWebsummaryLink" CssClass="col-sm-3 alignbottom">Link</asp:Label>
            </div>
            <br />

            <div class="form-group">
                <asp:Label runat="server" Text="Verrechnungsstatus:" CssClass="control-label col-sm-3"></asp:Label>
                <div class="col-sm-3">
                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpBillingstatus" AutoPostBack="true" OnSelectedIndexChanged="DrpBillingstatusChanged" CssClass="form-control"></asp:DropDownList>
                </div>
            </div>
            <div class="form-group" style="text-align: left" runat="server">
                <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                <asp:Label runat="server" ID="Label2" CssClass="col-sm-6" Text="Hinweis: Für verrechenbare Projekte werden unmittelbar Rechnungen ausgelöst. Bitte NUR definitiven Status eintragen!"></asp:Label>
            </div>
            <hr />

            <asp:UpdatePanel UpdateMode="Conditional" runat="server" ID="updateGradeFields">
                <Triggers>
                    <asp:AsyncPostBackTrigger ControlID="drpLogLanguage" EventName="SelectedIndexChanged" />
                    <asp:AsyncPostBackTrigger ControlID="cbxWebSummaryChecked" EventName="CheckedChanged" />
                    <asp:AsyncPostBackTrigger ControlID="drpBillingstatus" EventName="SelectedIndexChanged" />
                </Triggers>
                <ContentTemplate>
                    <div class="form-group" style="text-align: left" runat="server" visible="false" id="divGradeHint">
                        <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                        <asp:Label runat="server" ID="Label3" CssClass="col-sm-6" Text="Bitte füllen Sie die Felder Websummary, Durchführungssprache und Verrechnungsstatus aus, um die Noten einzutragen."></asp:Label>
                    </div>
                    <div runat="server" visible="false" id="divGradeStudent1">
                        <div class="form-group">
                            <asp:Label runat="server" Text="Note:" CssClass="control-label col-sm-3" ID="lblGradeStudent1"></asp:Label>
                            <div class="col-sm-3">
                                <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="nbrGradeStudent1" CssClass="form-control"></asp:TextBox>
                            </div>
                        </div>
                    </div>
                    <div class="form-group" id="divGradeStudent2" runat="server" visible="false">
                        <asp:Label runat="server" Text="Note:" ID="lblGradeStudent2" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="col-sm-3">
                            <asp:TextBox runat="server" TextMode="Number" min="1" max="6" step="0.1" ID="nbrGradeStudent2" CssClass="form-control"></asp:TextBox>
                        </div>
                    </div>
                    <div class="form-group" style="text-align: left" runat="server" visible="false" id="divGradeWarning">
                        <asp:Label runat="server" CssClass="control-label col-sm-3"></asp:Label>
                        <asp:Label runat="server" ID="Label1" CssClass="col-sm-6" Text="Hinweis: Noten werden unmittelbar an die Ausbildungsadministration weitergeleitet. Bitte NUR definitive Noten eintragen!"></asp:Label>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>

            <asp:PlaceHolder runat="server" ID="BillAddressPlaceholder">
                <hr />
                <h3>Kundeninformationen</h3>
                <asp:UpdatePanel runat="server" ID="updateClientCompany" UpdateMode="Conditional">
                    <Triggers>
                        <asp:AsyncPostBackTrigger ControlID="radioClientType" EventName="SelectedIndexChanged" />
                    </Triggers>
                    <ContentTemplate>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Kundentyp:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-7 radioButtonSettings">
                                <asp:RadioButtonList runat="server" RepeatDirection="Horizontal" TextAlign="Right" BorderStyle="None" ID="radioClientType" AutoPostBack="true" OnSelectedIndexChanged="RadioClientType_SelectedIndexChanged">
                                    <asp:ListItem Text=" FHNW intern" Value="Intern" />
                                    <asp:ListItem Text=" Unternehmen" Value="Company" />
                                    <asp:ListItem Text=" Privatperson" Value="PrivatePerson" />
                                </asp:RadioButtonList>
                            </div>
                        </div>
                        <div style="clear: both"></div>
                        <div class="form-group" runat="server" id="divClientForm" visible="false">
                            <div class="form-group" style="text-align: left" runat="server" id="divClientCompany">
                                <asp:Label runat="server" Text="Unternehmen*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientCompany" CssClass="form-control maxWidth" MaxLength="255"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Anrede*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-3">
                                    <asp:DropDownList runat="server" DataValueField="Id" DataTextField="DisplayName" ID="drpClientTitle" AutoPostBack="false" CssClass="form-control">
                                        <asp:ListItem Text="Herr" Value="1" />
                                        <asp:ListItem Text="Frau" Value="2" />
                                    </asp:DropDownList>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Vor- und Nachname*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientName" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="E-Mail Adresse*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientEmail" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left" runat="server" id="divClientDepartment">
                                <asp:Label runat="server" Text="Abteilung:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientDepartment" CssClass="form-control maxWidth" Placeholder="Falls vorhanden" MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Strasse und Nummer*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientStreet" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="PLZ*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientPLZ" CssClass="form-control maxWidth" MaxLength="10"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Ort*:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientCity" CssClass="form-control maxWidth" MaxLength="100"></asp:TextBox>
                                </div>
                            </div>
                            <div class="form-group" style="text-align: left">
                                <asp:Label runat="server" Text="Referenz des Kunden:" CssClass="control-label col-sm-3"></asp:Label>
                                <div class="col-sm-6">
                                    <asp:TextBox runat="server" ID="txtClientReference" CssClass="form-control maxWidth" Placeholder="Falls vorhanden." ToolTip="z.B. Bestellnummer des Auftraggebers." MaxLength="50"></asp:TextBox>
                                </div>
                            </div>
                        </div>
                        <div class="form-group" style="text-align: left">
                            <asp:Label runat="server" Text="Geheimhaltung:" CssClass="control-label col-sm-3"></asp:Label>
                            <div class="col-sm-6">
                                <asp:CheckBox runat="server" ID="chkNDA" CssClass="form-control" ToolTip="NDA-Projekte werden nicht in öffentlichen Broschüren aufgeführt" Text=" Projekt/Auftraggeber unter NDA" />
                            </div>
                        </div>
                    </ContentTemplate>
                </asp:UpdatePanel>
                <h6>Mit * markierte Felder sind Pflichtfelder.</h6>
            </asp:PlaceHolder>
        </div>
        <div style="clear: both"></div>
        <div style="text-align: right;">
            <asp:Button runat="server" ID="BtnSaveChanges" OnClick="BtnSaveChanges_OnClick" CssClass="btn btn-default" Text="Speichern & Schliessen"></asp:Button>
            <asp:Button runat="server" ID="BtnSaveBetween" OnClick="BtnSaveBetween_OnClick" CssClass="btn btn-default" Text="Zwischenspeichern" />
            <asp:Button runat="server" ID="BtnCancel" OnClick="BtnCancel_OnClick" CssClass="btn btn-default" Text="Abbrechen"></asp:Button>
        </div>
    </div>
    <div class="well newProjectSettings">
        <asp:Label runat="server" ID="lblProjectAttachements" Font-Size="24px" Height="50px" Text="Projekt Artefakte"></asp:Label>
        <div class="well contentDesign form-horizontal" style="background-color: #ffffff">
            <asp:UpdatePanel runat="server" ID="updateProjectAttachements" UpdateMode="Conditional">
                <ContentTemplate>
                    <asp:Label runat="server" Text="Projekt Artefakte (Dokumentation, Präsentation, Code):" CssClass="control-label col-sm-3"></asp:Label>
                    <div class="form-group col-sm-9">
                        <asp:GridView runat="server" Width="100%" ID="gridProjectAttachs" EmptyDataText="Noch keine Dokumente hochgeladen." ItemType="ProStudCreator.ProjectSingleAttachment" EnableModelValidation="False" ValidateRequestMode="Disabled" OnSelectedIndexChanged="GridProjectAttachs_OnSelectedIndexChanged" CellPadding="4" EnableViewState="False" GridLines="None" AutoGenerateColumns="False" ForeColor="#333333" AllowSorting="False" OnRowCommand="GridProjectAttachs_OnRowCommand" OnRowDataBound="GridProjectAttachs_OnRowDataBound" DataKeyNames="Guid">
                            <Columns>
                                <asp:ImageField ItemStyle-Width="20px" DataImageUrlField="FileType" ControlStyle-Height="30px" />
                                <asp:BoundField DataField="Name" HeaderText="Dokumentname" ItemStyle-Wrap="False" />
                                <asp:BoundField DataField="Size" HeaderText="Dateigrösse" />
                                <asp:TemplateField ItemStyle-Wrap="False">
                                    <ItemTemplate>
                                        <asp:LinkButton runat="server" ID="deleteProjectAttachButton" ToolTip="Datei löschen" CommandName="deleteProjectAttach" OnClientClick="return confirm('Wollen Sie diese Datei wirklich löschen?');" CommandArgument="<%# Item.Guid %>" CssClass="btn btn-default btnHeight glyphicon glyphicon-remove"></asp:LinkButton>
                                    </ItemTemplate>
                                </asp:TemplateField>
                            </Columns>
                            <HeaderStyle BackColor="#a5c7ff" Font-Bold="True" ForeColor="Black" />
                            <RowStyle BackColor="#FFFFFF" ForeColor="0000000"></RowStyle>
                        </asp:GridView>
                    </div>
                    <div style="clear: both"></div>
                    <div runat="server" id="divFileUpload">
                        <hr />
                        <asp:Label runat="server" Text="Upload Projekt Artefakte:" CssClass="control-label col-sm-3"></asp:Label>
                        <div class="form-group">
                            <ajax:AjaxFileUpload runat="server" MaxFileSize="-1" OnUploadComplete="OnUploadComplete" ClearFileListAfterUpload="True" AutoStartUpload="True" ID="fileUpProjectAttach" AllowedFileTypes="7z,aac,avi,bz2,csv,doc,docx,gif,gz,htm,html,jpeg,jpg,md,mp3,mp4,ods,odt,ogg,pdf,png,ppt,pptx,svg,tar,tgz,txt,xls,xlsx,xml,zip" OnClientUploadCompleteAll="doPostBack" MaximumNumberOfFiles="-1"  />
                            <small class="col-sm-offset-4">Dokumente mit gleichem Namen werden überschriben.</small>
                        </div>
                    </div>
                </ContentTemplate>
            </asp:UpdatePanel>
        </div>
        <asp:UpdatePanel ID="updateDownloadButton" runat="server" UpdateMode="Conditional">
            <Triggers>
                <%--<asp:AsyncPostBackTrigger runat="server" ControlID="gridProjectAttachs" EventName="DataBound"/>--%>
                <asp:PostBackTrigger ControlID="BtnDownloadAllFiles"/>
            </Triggers>
            <ContentTemplate>
                <div runat="server" ID="divDownloadBtn" style="text-align: right;">
                <button runat="server" ID="btnDownloadAllFiles" class="btn btn-default" OnServerClick="DownloadFiles_OnClick"><img src="Content/zip.png" style="height: 30px;" alt="download"/>  Download ZIP </button>
            </div>
          </ContentTemplate>
        </asp:UpdatePanel>
    </div>
    <script type="text/javascript">
        function doPostBack() {
            var updatePanel1 = '<%=updateProjectAttachements.ClientID%>';
            __doPostBack(updatePanel1, '');
        }
    </script>
</asp:Content>

