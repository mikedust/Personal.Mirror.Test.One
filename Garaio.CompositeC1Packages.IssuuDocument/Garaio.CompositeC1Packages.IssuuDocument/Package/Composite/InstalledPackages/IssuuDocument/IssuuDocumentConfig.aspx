<?xml version="1.0" encoding="UTF-8" ?>

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="IssuuDocumentConfig.aspx.cs" Inherits="Garaio.CompositeC1Packages.IssuuDocument.Package.Composite.InstalledPackages.IssuuDocument.IssuuDocumentConfig" %>

<%@ Register TagPrefix="aspui" Namespace="Composite.Core.WebClient.UiControlLib" Assembly="Composite" %>
<%@ Register TagPrefix="control" TagName="httpheaders" Src="~/Composite/controls/HttpHeadersControl.ascx" %>
<%@ Register TagPrefix="control" TagName="styleloader" Src="~/Composite/controls/StyleLoaderControl.ascx" %>
<%@ Register TagPrefix="control" TagName="scriptloader" Src="~/Composite/controls/ScriptLoaderControl.ascx" %>
<%@ Register TagPrefix="composite" TagName="TextBox" Src="~/Composite/InstalledPackages/IssuuDocument/TextBox.ascx" %>

<html xmlns="http://www.w3.org/1999/xhtml" xmlns:ui="http://www.w3.org/1999/xhtml">
<control:httpheaders runat="server" />
<head runat="server">
	<title>Issuu Add-In Settings</title>
	<control:styleloader runat="server" />
	<control:scriptloader type="sub" runat="server" updateManagerDisabled="True" />
</head>
<body>

	<form id="Form1" runat="server">
		<ui:page label="Issuu Konfiguration Einstellungen" image="${icon:tools}">
			<ui:toolbar id="toolbar">
				<ui:toolbarbody>
					<ui:toolbargroup>
						<aspui:ToolbarButton autopostback="true" Text="Speichern" ImageUrl="${icon:save}" runat="server" OnClick="SaveSettings" />
					</ui:toolbargroup>
				</ui:toolbarbody>
			</ui:toolbar>
			<ui:scrollbox id="scrollbox" class="padded flexboxelement">
				<ui:fields>
					<ui:fieldgroup class="first" label="Issuu Konfiguration">
						<ui:field class="clearfloatelement">
							<ui:fielddesc>API-Schlüssel</ui:fielddesc>
							<ui:fielddata>
								<composite:TextBox runat="server" Type="String" ID="ApiKey" Required="True" />
							</ui:fielddata>
						</ui:field>
						<ui:field class="clearfloatelement">
							<ui:fielddesc>API-Geheimschlüssel</ui:fielddesc>
							<ui:fielddata>
								<composite:TextBox runat="server" Type="String" ID="ApiSecretKey" Required="True" />
							</ui:fielddata>
						</ui:field>
						<ui:field class="clearfloatelement">
							<ui:fielddesc>API-URL</ui:fielddesc>
							<ui:fielddata>
								<composite:TextBox runat="server" Type="String" ID="ApiUrl" Required="True" />
							</ui:fielddata>
						</ui:field>
						<ui:field class="clearfloatelement">
							<ui:fielddesc>Anzahl der Abfragen pro Sekunde (http://developers.issuu.com/api/limits.html)</ui:fielddesc>
							<ui:fielddata>
								<composite:TextBox runat="server" Type="Integer" ID="RequestsPerSec" Required="True" />
							</ui:fielddata>
						</ui:field>
					</ui:fieldgroup>
				</ui:fields>
			</ui:scrollbox>
		</ui:page>
	</form>
</body>
</html>
