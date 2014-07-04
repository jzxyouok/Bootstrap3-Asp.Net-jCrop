<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default.aspx.cs" Inherits="_Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml" class="no-js">
<head runat="server">
    <title></title>
    <script src="Scripts/modernizr-2.7.2.js"></script>
    <link href="Content/bootstrap.min.css" rel="stylesheet" />
    <link href="Content/bootstrap-theme.min.css" rel="stylesheet" />
    <link href="Content/font-awesome.min.css" rel="stylesheet" />
    <link href="Content/jquery.Jcrop.min.css" rel="stylesheet" />

</head>
<body>
    <form id="form1" runat="server">
        <nav class="navbar navbar-inverse navbar-static-top" role="navigation">
            <div class="container">
                <!-- Brand and toggle get grouped for better mobile display -->
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target="#bs-example-navbar-collapse-1">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                    <a class="navbar-brand" href="Default.aspx">jCrop ASP.Net</a>
                </div>
                <!-- Collect the nav links, forms, and other content for toggling -->
                <div class="collapse navbar-collapse" id="bs-example-navbar-collapse-1">
                    <ul class="nav navbar-nav">
                        <li class="active"><a href="#">Link</a></li>
                        <li><a href="#">Link</a></li>
                        <li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">Dropdown <span class="caret">
                            </span></a>
                            <ul class="dropdown-menu" role="menu">
                                <li><a href="#">Action</a></li>
                                <li><a href="#">Another action</a></li>
                                <li><a href="#">Something else here</a></li>
                                <li class="divider"></li>
                                <li><a href="#">Separated link</a></li>
                                <li class="divider"></li>
                                <li><a href="#">One more separated link</a></li>
                            </ul>
                        </li>
                    </ul>
                    <ul class="nav navbar-nav navbar-right">
                        <li><a href="#">Link</a></li>
                        <li class="dropdown">
                            <a href="#" class="dropdown-toggle" data-toggle="dropdown">Dropdown <span class="caret">
                            </span></a>
                            <ul class="dropdown-menu" role="menu">
                                <li><a href="#">Action</a></li>
                                <li><a href="#">Another action</a></li>
                                <li><a href="#">Something else here</a></li>
                                <li class="divider"></li>
                                <li><a href="#">Separated link</a></li>
                            </ul>
                        </li>
                    </ul>
                </div>
                <!-- /.navbar-collapse -->
            </div>
            <!-- /.container-fluid -->
        </nav>
        <div class="container">
            <asp:Image ID="imgProfile" runat="server" CssClass="img-responsive" />
            <br />
            <asp:FileUpload ID="fupProfileImg" runat="server" />
            <br />
            <asp:Button ID="btnProfileImage" runat="server" Text="Upload" CssClass="btn btn-primary"
                OnClick="btnProfileImage_Click" />
            <br />
            <asp:Label ID="lblError" runat="server" Text="Label" Visible="false"></asp:Label>
        </div>
        <div class="modal fade" id="mdlImageCrop" tabindex="-1" role="dialog" aria-labelledby="mdlImageLabel"
            aria-hidden="true">
            <div class="modal-dialog">
                <div class="modal-content">
                    <div class="modal-header">
                        <h4 class="modal-title" id="mdlImageCropLabel">Crop Image</h4>
                    </div>
                    <div class="modal-body">
                        <div>
                            <%--<asp:Label ID="Label13" runat="server" Text="Please Upload the image of 1094x376 for Better Showing" Visible="false" />--%>
                            <asp:Image ID="imgImageCrop" runat="server" />
                            <br />
                            <asp:HiddenField ID="hdnW" runat="server" />
                            <asp:HiddenField ID="hdnH" runat="server" />
                            <asp:HiddenField ID="hdnX" runat="server" />
                            <asp:HiddenField ID="hdnY" runat="server" />
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="butImageCrop" runat="server" Text="Crop" CssClass="btn btn-primary"
                            OnClick="butImageCrop_Click" />
                        <asp:Button ID="butImageCropClose" runat="server" Text="Close" CssClass="btn btn-default" />
                    </div>
                </div>
                <!-- /.modal-content -->
            </div>
            <!-- /.modal-dialog -->
        </div>
        <script src="Scripts/jquery-1.9.0.min.js"></script>
        <script src="Scripts/bootstrap.min.js"></script>
        <script src="Scripts/jquery.color.js"></script>
        <script src="Scripts/jquery.Jcrop.min.js"></script>
        <script type="text/javascript">
            var ImagejcropValues = [
            <asp:Repeater ID="rptImageJcropValues" runat="server">
            <ItemTemplate>
                     {
                         "ImageminSizeWidth":'<%# Eval("ImageminSizeWidth") %>',
                       "ImageminSizeHeight":'<%# Eval("ImageminSizeHeight") %>',
                       "ImagemaxSizeWidth":'<%# Eval("ImagemaxSizeWidth") %>',
                       "ImagemaxSizeHeight":'<%# Eval("ImagemaxSizeHeight") %>',
                       "RatioIs":'<%# Eval("RatioIs") %>',
                       "RatioTo":'<%# Eval("RatioTo") %>'
                   }
    </ItemTemplate>
    <SeparatorTemplate>
        ,
    </SeparatorTemplate>
    </asp:Repeater>
          ];
        
          jQuery(document).ready(function () {
              jQuery('#imgImageCrop').Jcrop({
                  aspectRatio: ImagejcropValues[0].RatioIs / ImagejcropValues[0].RatioTo,
                  minSize: [ImagejcropValues[0].ImageminSizeWidth, ImagejcropValues[0].ImageminSizeHeight],
                  // maxSize: [jcropValues[0].maxSizeWidth, jcropValues[0].maxSizeHeight],
                  // boxWidth: 270, boxHeight: 70,
                  setSelect:[10,10,50,50],
                  onSelect: storeImageCoords
              });
          });
          function storeImageCoords(c) {
              jQuery('#hdnX').val(c.x);
              jQuery('#hdnY').val(c.y);
              jQuery('#hdnW').val(c.w);
              jQuery('#hdnH').val(c.h);
          };
        </script>
    </form>
</body>
</html>
