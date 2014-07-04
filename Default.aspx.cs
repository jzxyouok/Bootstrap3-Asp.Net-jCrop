using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using SD = System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            SetImageJavascriptValues(200, 200, 0, 0, 1m, 1m);
            imgProfile.ImageUrl = "Content/Images/not_available.jpg";
        }
    }
    protected void btnProfileImage_Click(object sender, EventArgs e)
    {
        if (fupProfileImg.HasFile)
        {
            String[] allowedExtensions = { ".png", ".jpeg", ".jpg", ".gif" };
            if (allowedExtensions.Contains(Path.GetExtension(fupProfileImg.FileName).ToLower()))
            {
                // this code will create a dynamic folder named "Images" in solution explorer...
                Directory.CreateDirectory(Server.MapPath("TempImages"));
                string UploadedImg = "TempImages/" + "Photo" + '-' + DateTime.Now.Ticks.ToString() + Path.GetExtension(fupProfileImg.PostedFile.FileName);
                Session["ImgName"] = UploadedImg;
                Stream strmImage = fupProfileImg.PostedFile.InputStream;
                fupProfileImg.SaveAs(Server.MapPath(UploadedImg));
                //Call resize start
                string targetPath = Server.MapPath(UploadedImg);
                var targetFile = targetPath;
                GenerateThumbnails(strmImage, targetFile);
                //Call resize end
                imgImageCrop.ImageUrl = Session["ImgName"].ToString();
                string scriptstring = "<script>$(function () {$('#mdlImageCrop').modal({show:true,keyboard:false,backdrop:'static'});});</script>";
                ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "", scriptstring, false);
            }
            else
            {
                lblError.Text = "Cannot accept files of this type.";
                lblError.Visible = true;
            }
        }
        else
        {
            lblError.Text = "Please select a file to upload.";
            lblError.Visible = true;
        }
    }
    protected void butImageCrop_Click(object sender, EventArgs e)
    {
        String path = HttpContext.Current.Request.PhysicalApplicationPath + Session["ImgName"].ToString(); 
        // get the path of the image to crop
        string ImageName = Session["ImgName"].ToString();
        string abc = Math.Round(decimal.Parse(hdnW.Value)).ToString();
        int w = Convert.ToInt32(Math.Round(decimal.Parse(hdnW.Value)));      // these are the 4 co-ordinates of the cropped image that we are storing
        int h = Convert.ToInt32(Math.Round(decimal.Parse(hdnH.Value)));
        int x = Convert.ToInt32(Math.Round(decimal.Parse(hdnX.Value)));
        int y = Convert.ToInt32(Math.Round(decimal.Parse(hdnY.Value)));
        //  Response.Write("w = " + w + " h= " + h + " x= " + x + " y=" + y);
        //return;  //this is set by me to write the co-ordinates on the screen...
        byte[] CropImage = Crop(path, w, h, x, y);
        using (MemoryStream ms = new MemoryStream(CropImage, 0, CropImage.Length))
        {
            ms.Write(CropImage, 0, CropImage.Length);
            using (SD.Image CroppedImage = SD.Image.FromStream(ms, true))
            {
                using (var image = System.Drawing.Image.FromStream(ms))
                {
                    var newWidth = image.Width;
                    var newHeight = image.Height;
                    
                    var thumbnailImg = new Bitmap(newWidth, newHeight);
                    var thumbGraph = Graphics.FromImage(thumbnailImg);
                    thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
                    thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
                    thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    var imageRectangle = new Rectangle(0, 0, newWidth, newHeight);
                    thumbGraph.DrawImage(image, imageRectangle);
                    string SaveTo = HttpContext.Current.Request.PhysicalApplicationPath + Session["ImgName"].ToString().Replace("TempImages", "UploadedImages");
                    thumbnailImg.Save(SaveTo, image.RawFormat);
                   
                        imgProfile.ImageUrl = Session["ImgName"].ToString().Replace("TempImages", "UploadedImages");
                    
                    if (File.Exists(path))
                    {
                        Session.Remove("ImgName");
                        File.Delete(path);
                    }
                }
            }
        }
    }
    static byte[] Crop(string Img, int Width, int Height, int X, int Y)
    {
        try
        {
            using (SD.Image OriginalImage = SD.Image.FromFile(Img))
            {
                using (SD.Bitmap bmp = new SD.Bitmap(Width, Height))
                {
                    bmp.SetResolution(OriginalImage.HorizontalResolution, OriginalImage.VerticalResolution);
                    using (SD.Graphics Graphic = SD.Graphics.FromImage(bmp))
                    {
                        Graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        Graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        Graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        Graphic.DrawImage(OriginalImage, new SD.Rectangle(0, 0, Width, Height), X, Y, Width, Height, SD.GraphicsUnit.Pixel);
                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, OriginalImage.RawFormat);
                        return ms.GetBuffer();
                    }
                }
            }
        }
        catch (Exception Ex)
        {
            throw (Ex);
        }
    }
   
    private void SetImageJavascriptValues(int minWidth, int minHeight, int maxWidth, int maxHeight, decimal ratioIs, decimal ratioTo)
    {
        DataTable dt = new DataTable("ImageJcropValueTable");
        dt.Columns.Add(new DataColumn("ImageminSizeWidth", typeof(int)));
        dt.Columns.Add(new DataColumn("ImageminSizeHeight", typeof(int)));
        dt.Columns.Add(new DataColumn("ImagemaxSizeWidth", typeof(int)));
        dt.Columns.Add(new DataColumn("ImagemaxSizeHeight", typeof(int)));
        dt.Columns.Add(new DataColumn("RatioIs", typeof(int)));
        dt.Columns.Add(new DataColumn("RatioTo", typeof(int)));
        DataRow dr = dt.NewRow();
        dr["ImageminSizeWidth"] = minWidth;
        dr["ImageminSizeHeight"] = minHeight;
        dr["ImagemaxSizeWidth"] = maxWidth;
        dr["ImagemaxSizeHeight"] = maxHeight;
        dr["RatioIs"] = ratioIs;
        dr["RatioTo"] = ratioTo;
        dt.Rows.Add(dr);
        rptImageJcropValues.DataSource = dt;
        rptImageJcropValues.DataBind();
    }
    private void GenerateThumbnails(Stream sourcePath, string targetPath)
    {
        using (var image = SD.Image.FromStream(sourcePath))
        {
            //Default Width and Height for Jcrop Image
            var newWidth = image.Width;//(int)(image.Width * scaleFactor);//double scaleFactor,
            float newHeight = image.Height;//(int)(image.Height * scaleFactor);

            if(image.Width>560)
            {
                newWidth = 560;
                float aspectRatio = (float)image.Width / (float)image.Height;
                newHeight = newWidth / aspectRatio;
            }

            var thumbnailImg = new Bitmap(newWidth, (int)Math.Ceiling(newHeight));
            var thumbGraph = Graphics.FromImage(thumbnailImg);
            thumbGraph.CompositingQuality = CompositingQuality.HighQuality;
            thumbGraph.SmoothingMode = SmoothingMode.HighQuality;
            thumbGraph.InterpolationMode = InterpolationMode.HighQualityBicubic;
            var imageRectangle = new Rectangle(0, 0, newWidth, (int)Math.Ceiling(newHeight));
            thumbGraph.DrawImage(image, imageRectangle);
            thumbnailImg.Save(targetPath, image.RawFormat);
        }
    }
    protected void butImageCropClose_Click(object sender, EventArgs e)
    {
        Response.Redirect(Request.Url.AbsoluteUri);
    }
}