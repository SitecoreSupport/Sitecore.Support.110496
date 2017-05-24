using Sitecore.Configuration;
using Sitecore.Controls;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Shell.Applications.ContentEditor;
using Sitecore.Web;
using Sitecore.Web.UI.HtmlControls;
using Sitecore.Web.UI.Sheer;
using Sitecore.Web.UI.XamlSharp.Xaml;
using System;
using System.Web.UI.WebControls;

namespace Sitecore.Support.Shell.Applications.Media.ImageProperties
{
    public class ImagePropertiesPage : DialogPage
    {
        protected TextBox Alt;

        protected Sitecore.Web.UI.HtmlControls.Checkbox Aspect;

        protected TextBox HeightEdit;

        protected TextBox HSpace;

        protected Sitecore.Web.UI.HtmlControls.Literal OriginalSize;

        protected TextBox OriginalText;

        protected Border SizeWarning;

        protected TextBox VSpace;

        protected TextBox WidthEdit;

        public int ImageHeight
        {
            get
            {
                return (int)this.ViewState["ImageHeight"];
            }
            set
            {
                this.ViewState["ImageHeight"] = value;
            }
        }

        public int ImageWidth
        {
            get
            {
                return (int)this.ViewState["ImageWidth"];
            }
            set
            {
                this.ViewState["ImageWidth"] = value;
            }
        }

        private XmlValue XmlValue
        {
            get
            {
                return new XmlValue(StringUtil.GetString(this.ViewState["XmlValue"]), "image");
            }
            set
            {
                Assert.ArgumentNotNull(value, "value");
                this.ViewState["XmlValue"] = value.ToString();
            }
        }

        protected void ChangeHeight()
        {
            if (this.ImageHeight != 0)
            {
                int num = MainUtil.GetInt(this.HeightEdit.Text, 0);
                if (num > 0)
                {
                    if (num > 8192)
                    {
                        num = 8192;
                        this.HeightEdit.Text = "8192";
                        SheerResponse.SetAttribute(this.HeightEdit.ClientID, "value", this.HeightEdit.Text);
                    }
                    if (this.Aspect.Checked)
                    {
                        this.WidthEdit.Text = ((int)((float)num / (float)this.ImageHeight * (float)this.ImageWidth)).ToString();
                        SheerResponse.SetAttribute(this.WidthEdit.ClientID, "value", this.WidthEdit.Text);
                    }
                }
                SheerResponse.SetReturnValue(true);
            }
        }

        protected void ChangeWidth()
        {
            if (this.ImageWidth != 0)
            {
                int num = MainUtil.GetInt(this.WidthEdit.Text, 0);
                if (num > 0)
                {
                    if (num > 8192)
                    {
                        num = 8192;
                        this.WidthEdit.Text = "8192";
                        SheerResponse.SetAttribute(this.WidthEdit.ClientID, "value", this.WidthEdit.Text);
                    }
                    if (this.Aspect.Checked)
                    {
                        this.HeightEdit.Text = ((int)((float)num / (float)this.ImageWidth * (float)this.ImageHeight)).ToString();
                        SheerResponse.SetAttribute(this.HeightEdit.ClientID, "value", this.HeightEdit.Text);
                    }
                }
                SheerResponse.SetReturnValue(true);
            }
        }

        protected override void OK_Click()
        {
            XmlValue xmlValue = this.XmlValue;
            Assert.IsNotNull(xmlValue, "XmlValue");
            xmlValue.SetAttribute("alt", this.Alt.Text);
            xmlValue.SetAttribute("height", this.HeightEdit.Text);
            xmlValue.SetAttribute("width", this.WidthEdit.Text);
            xmlValue.SetAttribute("hspace", this.HSpace.Text);
            xmlValue.SetAttribute("vspace", this.VSpace.Text);
            SheerResponse.SetDialogValue(xmlValue.ToString());
            base.OK_Click();
        }

        protected override void OnLoad(EventArgs e)
        {
            Assert.ArgumentNotNull(e, "e");
            base.OnLoad(e);
            if (!XamlControl.AjaxScriptManager.IsEvent)
            {
                this.ImageWidth = 0;
                this.ImageHeight = 0;
                ItemUri itemUri = ItemUri.ParseQueryString();
                if (itemUri != null)
                {
                    Item item = Database.GetItem(itemUri);
                    if (item != null)
                    {
                        string text = item["Dimensions"];
                        if (!string.IsNullOrEmpty(text))
                        {
                            int num = text.IndexOf('x');
                            if (num >= 0)
                            {
                                this.ImageWidth = MainUtil.GetInt(StringUtil.Left(text, num).Trim(), 0);
                                this.ImageHeight = MainUtil.GetInt(StringUtil.Mid(text, num + 1).Trim(), 0);
                            }
                        }
                        if (this.ImageWidth <= 0 || this.ImageHeight <= 0)
                        {
                            this.Aspect.Checked = false;
                            this.Aspect.Disabled = true;
                        }
                        else
                        {
                            this.Aspect.Checked = true;
                        }
                        if (this.ImageWidth > 0)
                        {
                            this.OriginalSize.Text = Translate.Text("Original Dimensions: {0} x {1}", new object[]
                            {
                                this.ImageWidth,
                                this.ImageHeight
                            });
                        }
                        if (MainUtil.GetLong(item["Size"], 0L) >= Settings.Media.MaxSizeInMemory)
                        {
                            this.HeightEdit.Enabled = false;
                            this.WidthEdit.Enabled = false;
                            this.Aspect.Disabled = true;
                        }
                        else
                        {
                            this.SizeWarning.Visible = false;
                        }
                        this.OriginalText.Text = StringUtil.GetString(new string[]
                        {
                            item["Alt"],
                            Translate.Text("[none]")
                        });
                        UrlHandle urlHandle = UrlHandle.Get();
                        XmlValue xmlValue = new XmlValue(urlHandle["xmlvalue"], "image");
                        this.XmlValue = xmlValue;
                        this.Alt.Text = xmlValue.GetAttribute("alt");
                        this.HeightEdit.Text = xmlValue.GetAttribute("height");
                        this.WidthEdit.Text = xmlValue.GetAttribute("width");
                        this.HSpace.Text = xmlValue.GetAttribute("hspace");
                        this.VSpace.Text = xmlValue.GetAttribute("vspace");
                        if (MainUtil.GetBool(urlHandle["disableheight"], false))
                        {
                            this.HeightEdit.Enabled = false;
                            this.Aspect.Checked = false;
                            this.Aspect.Disabled = true;
                        }
                        if (MainUtil.GetBool(urlHandle["disablewidth"], false))
                        {
                            this.WidthEdit.Enabled = false;
                            this.Aspect.Checked = false;
                            this.Aspect.Disabled = true;
                        }
                    }
                }
            }
        }
    }
}