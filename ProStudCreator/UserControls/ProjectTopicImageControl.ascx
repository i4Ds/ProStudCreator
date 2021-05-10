<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTopicImageControl.ascx.cs" Inherits="ProStudCreator.UserControls.ProjectTopicImageControl" %>

<asp:Panel runat="server" CssClass="projectTopicWrapper projectTopicFlexbox" ID="DivWrapper">
    <div class="projectTopicContent">
        <div runat="server" class="projectTopicFlexbox projectTopicFullWidth unselectable" id="DivFirstTopic"></div>
        <div runat="server" class="projectTopicFlexbox projectTopicFullWidth unselectable" id="DivSecondTopic"></div>
    </div>
</asp:Panel>
