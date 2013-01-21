<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DefaultLayout.aspx.cs" Inherits="FieldFallback.Demo.Web.sitecore_modules.FieldFallbackDemo.DefaultLayout" %>

<!DOCTYPE html>
<html lang="en">

<head runat="server">
    <title></title>
    <link href="//netdna.bootstrapcdn.com/twitter-bootstrap/2.2.2/css/bootstrap-combined.min.css" rel="stylesheet">
</head>
<body>
    <form id="form1" runat="server">
    
        <div class="container-fluid">
          <div class="row-fluid">
            <div class="span2">
              <sc:placeholder key="left" runat="server" />
            </div>
            <div class="span10">
              <sc:placeholder key="content" runat="server" />
            </div>
          </div>
        </div>
        
    
    </form>
</body>
</html>
