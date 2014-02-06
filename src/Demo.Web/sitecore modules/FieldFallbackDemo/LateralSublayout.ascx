<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="LateralSublayout.ascx.cs" Inherits="FieldFallback.Demo.Web.sitecore_modules.FieldFallbackDemo.LateralSublayout" %>

<h3>Field Values</h3>
<table class="table table-striped table-bordered">
    <tr>
        <td><code>SourceField1</code></td>
        <td><sc:Text runat="server" Field="SourceField1" /></td>
    </tr>
    <tr>
        <td><code>SourceField2</code></td>
        <td><sc:Text runat="server" Field="SourceField2" /></td>
    </tr>
    <tr>
        <td><code>SourceField3</code></td>
        <td><sc:Text runat="server" Field="SourceField3" /></td>
    </tr>
    <tr>
        <td><code>TargetField</code></td>
        <td><sc:Text runat="server" Field="TargetField" /></td>
    </tr>
</table>