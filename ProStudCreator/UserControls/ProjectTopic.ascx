<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ProjectTopic.ascx.cs" Inherits="ProStudCreator.UserControls.ProjectTopic" %>
<style type="text/css">
    .projectTopicWrapper {
        --factor: 1.6;
        --height: 55px;
        --width: calc(var(--height) * var(--factor));
        height: calc(var(--height))!important;
        width: calc(var(--width))!important;
        border-style: none;
        border-width: calc(var(--height) / 25);
        border-radius: calc(var(--height) / 9);
        background-color: lightgreen;
    }
    .projectTopicWrapper:hover {
        border-style: solid;
    }
    .flexbox {
        display: flex;
        justify-content: center;
        align-items: center;
    }
    .content {
        flex: 0 0 auto;
        font-weight: bold;
        font-size: calc(var(--height) / 2.75);
        color: #000000;
    }
    .fullWidth {
        width: var(--width);
    }
    .button {
        position: absolute;
        width: var(--width);
        height: var(--height);
        opacity: 0;
    }
</style>
<asp:UpdatePanel runat="server" UpdateMode="Conditional" ID="TopicUpdatePanel">
    <Triggers>
        <asp:AsyncPostBackTrigger ControlID="ButToggle" EventName="click"/>
    </Triggers>
    <ContentTemplate>
        <asp:Panel runat="server" CssClass="projectTopicWrapper flexbox" ID="DivWrapper">
            <asp:Button runat="server" CssClass="button" ID="ButToggle" OnClick="ButToggle_OnClick" />
            <div class="content">
                <div runat="server" class="flexbox fullWidth" id="DivFirstTopic"></div>
                <div runat="server" class="flexbox fullWidth" id="DivSecondTopic"></div>
            </div>
        </asp:Panel>
   </ContentTemplate>
</asp:UpdatePanel>
