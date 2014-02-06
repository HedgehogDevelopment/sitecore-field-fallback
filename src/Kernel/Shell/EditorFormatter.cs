using System.Text;
using System.Web;
using System.Web.UI;
using FieldFallback.Data;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Globalization;
using Sitecore.Resources;
using Sitecore.Shell;
using Sitecore.Shell.Applications.ContentManager;

namespace FieldFallback.Shell
{
    public class FallbackEditorFormatter : Sitecore.Shell.Applications.ContentEditor.EditorFormatter
    {
        public override void RenderField(Control parent, Editor.Field field, bool readOnly)
        {
            Assert.ArgumentNotNull(parent, "parent");
            Assert.ArgumentNotNull(field, "field");
            Field itemField = field.ItemField;
            Item fieldType = GetFieldType(itemField);
            if (fieldType != null)
            {
                if (!itemField.CanWrite)
                {
                    readOnly = true;
                }
                RenderMarkerBegin(parent, field.ControlID);
                RenderMenuButtons(parent, field, fieldType, readOnly);
                RenderLabel(parent, field, fieldType, readOnly);
                RenderField(parent, field, fieldType, readOnly);
                RenderMarkerEnd(parent);
            }
        }

        public new void RenderLabel(Control parent, Editor.Field field, Item fieldType, bool readOnly)
        {
            Assert.ArgumentNotNull(parent, "parent");
            Assert.ArgumentNotNull(field, "field");
            Assert.ArgumentNotNull(fieldType, "fieldType");

            if (Arguments.Item != null)
            {
                Field itemField = Arguments.Item.Fields[field.ItemField.ID];

                Language language = Arguments.Item.Language;

                Assert.IsNotNull(language, "language");
                if (itemField.Language != language)
                {
                    Item item = ItemManager.GetItem(field.ItemField.Item.ID, language, Version.Latest, field.ItemField.Item.Database);
                    if (item != null)
                    {
                        itemField = item.Fields[itemField.ID];
                    }
                }
                string name = itemField.Name;
                if (!string.IsNullOrEmpty(itemField.DisplayName))
                {
                    name = itemField.DisplayName;
                }
                name = Translate.Text(name);
                string toolTip = itemField.ToolTip;
                if (!string.IsNullOrEmpty(toolTip))
                {
                    toolTip = Translate.Text(toolTip);
                    if (toolTip.EndsWith("."))
                    {
                        toolTip = Sitecore.StringUtil.Left(toolTip, toolTip.Length - 1);
                    }
                    name = name + " - " + toolTip;
                }
                name = HttpUtility.HtmlEncode(name);
                bool flag = false;
                StringBuilder builder = new StringBuilder(200);
                if (this.Arguments.IsAdministrator && (itemField.Unversioned || itemField.Shared))
                {
                    builder.Append("<span class=\"scEditorFieldLabelAdministrator\"> [");
                    if (itemField.Unversioned)
                    {
                        builder.Append(Translate.Text("unversioned"));
                        flag = true;
                    }
                    if (itemField.Shared)
                    {
                        if (flag)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(Translate.Text("shared"));
                        flag = true;
                    }
                }



                if (itemField.InheritsValueFromOtherItem)
                {
                    if (flag)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        builder.Append("<span class=\"scEditorFieldLabelAdministrator\"> [");
                    }
                    builder.Append(Translate.Text("original value"));
                    flag = true;
                }
                if (FallbackValuesManager.Provider.FieldContainsFallbackValue(itemField, Arguments.Item.Language))
                {
                    if (flag)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        builder.Append("<span class=\"scEditorFieldLabelAdministrator\"> [");
                    }
                    builder.Append(Translate.Text("fallback value"));
                }
                else if (itemField.ContainsStandardValue)
                {
                    if (flag)
                    {
                        builder.Append(", ");
                    }
                    else
                    {
                        builder.Append("<span class=\"scEditorFieldLabelAdministrator\"> [");
                    }
                    builder.Append(Translate.Text("standard value"));
                }

                if (builder.Length > 0)
                {
                    builder.Append("]</span>");
                }
                name = name + builder.ToString() + ":";
                if (readOnly)
                {
                    name = "<span class=\"scEditorFieldLabelDisabled\">" + name + "</span>";
                }
                string helpLink = itemField.HelpLink;
                if (helpLink.Length > 0)
                {
                    name = "<a class=\"scEditorFieldLabelLink\" href=\"" + helpLink + "\" target=\"__help\">" + name + "</a>";
                }
                string str4 = string.Empty;
                if (itemField.Description.Length > 0)
                {
                    str4 = " title=\"" + itemField.Description + "\"";
                }
                string str5 = "scEditorFieldLabel";
                if ((UserOptions.View.UseSmartTags && !readOnly) && !UserOptions.ContentEditor.ShowRawValues)
                {
                    Item item2 = fieldType.Children["Menu"];
                    if (item2 != null)
                    {
                        ChildList children = item2.Children;
                        int count = children.Count;
                        if (count > 0)
                        {
                            string text = children[0]["Display Name"];
                            name = (string.Concat(new object[] { "<span id=\"SmartTag_", field.ControlID, "\" onmouseover='javascript:scContent.smartTag(this, event, \"", field.ControlID, "\", ", Sitecore.StringUtil.EscapeJavascriptString(text), ",\"", count, "\")'>" }) + Images.GetImage("Images/SmartTag.png", 11, 11, "middle", "0px 4px 0px 0px")) + "</span>" + name;
                        }
                    }
                }
                name = "<div class=\"" + str5 + "\"" + str4 + ">" + name + "</div>";
                this.AddLiteralControl(parent, name);
            }
            else
            {
                base.RenderLabel(parent, field, fieldType, readOnly);
            }
        }
    }
}
