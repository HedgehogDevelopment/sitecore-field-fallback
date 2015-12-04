<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MediaSublayout.ascx.cs" Inherits="FieldFallback.Demo.Web.sitecore_modules.FieldFallbackDemo.MediaSublayout" %>

<h3>Field Values</h3>
<table class="table table-striped table-bordered">
    <tr>
        <td><code>Media Item</code></td>
        <td><sc:Image runat="server" Field="Image"></sc:Image> </td>
    </tr>
    <tr>
        <td><code>Title Field</code></td>
        <td><asp:Literal runat="server" ID="litImageTitle"></asp:Literal></td>
    </tr>

</table>