<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DefaultSublayout.ascx.cs" Inherits="FieldFallback.Demo.Web.sitecore_modules.FieldFallbackDemo.DefaultSublayout" %>

<h3>Field Values</h3>
<table class="table table-striped table-bordered">
    <tr>
        <td><code>PureTextDefault</code></td>
        <td><sc:Text runat="server" Field="PureTextDefault" /></td>
    </tr>
    <tr>
        <td><code>TokenDefault</code></td>
        <td><sc:Text runat="server" Field="TokenDefault" /></td>
    </tr>
    <tr>
        <td><code>TokenInStandardValues</code></td>
        <td><sc:Text runat="server" Field="TokenInStandardValues" /></td>
    </tr>
    <tr>
        <td><code>OutOfTheBoxDefault</code></td>
        <td><sc:Text runat="server" Field="OutOfTheBoxDefault" /></td>
    </tr>
</table>