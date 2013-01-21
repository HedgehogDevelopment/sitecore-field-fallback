<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AncestorSublayout.ascx.cs" Inherits="FieldFallback.Demo.Web.sitecore_modules.FieldFallbackDemo.AncestorSublayout" %>

<h3>Field Values</h3>
<table class="table table-striped table-bordered">
    <tr>
        <td><code>DemoEmptyField</code></td>
        <td><sc:Text runat="server" Field="DemoEmptyField" /></td>
    </tr>
    <tr>
        <td><code>DemoFieldStandardValues</code></td>
        <td><sc:Text runat="server" Field="DemoFieldStandardValues" /></td>
    </tr>
</table>