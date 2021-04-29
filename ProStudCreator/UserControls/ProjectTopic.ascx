<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTopic.ascx.cs" Inherits="ProStudCreator.UserControls.ProjectTopic" %>

<asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="TopicUpdatePanel" ChildrenAsTriggers="true">
    <ContentTemplate>
        <asp:Panel runat="server" CssClass="projectTopicWrapper projectTopicWrapperHover projectTopicWrapperMargin projectTopicFlexbox" ID="DivWrapper">
            <asp:Button runat="server" CssClass="projectTopicButton" ID="ButToggle" OnClick="ButToggle_OnClick" />
            <div class="projectTopicContent">
                <div runat="server" class="projectTopicFlexbox projectTopicFullWidth unselectable" id="DivFirstTopic"></div>
                <div runat="server" class="projectTopicFlexbox projectTopicFullWidth unselectable" id="DivSecondTopic"></div>
            </div>
        </asp:Panel>
    </ContentTemplate>
</asp:UpdatePanel>
