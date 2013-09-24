using System.Collections.Generic;
using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Data.Items;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace FieldFallback.Processors
{
    public class LateralFieldFallbackProcessor : FieldFallbackProcessor
    {
        /// <summary>
        /// 
        /// <remarks>
        /// In certain scenarios a field may be configured with multiple fallback 
        /// processors. If enabled, then when the source fields are checked for a value
        /// they will be checked using their fallback values. 
        /// This could be problematic/inefficient with certain configurations.
        /// </remarks>
        /// </summary>
        public bool EnableNestedFallback { get; set; }

        protected override bool IsEnabledForField(Field field)
        {
            if (field.ID == FallbackItem.FallbackFields)
            {
                return false;
            }

            FallbackItem item = field.Item;
            return item.DoesFieldHaveLateralFallback(field);
        }

        protected override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");
            FallbackItem item = args.Field.Item;

            // Get the fields that this field should fallback to
            FieldFallback.Processors.Data.Items.FallbackItem.Setting setting = item.GetFallbackFields(args.Field);
            if (setting != null)
            {
                foreach (Field f in setting.SourceFields)
                {
                    // get the value of the field
                    // Standard Values are an acceptable value
                    string val = f.GetValueSafe(true, false, EnableNestedFallback);
                    if (val != null)
                    {
                        if (setting.TruncateText)
                        {
                            return TruncateText(val, setting.ClipAt.Value, setting.UseEllipsis);
                        }
                        else
                        {
                            return val;
                        }
                    }
                }
            }
            return null;
        }

        private static string TruncateText(string s, int clipAt, bool useEllipsis)
        {
            string text = s;
            text = text.Trim();
            text = Sitecore.StringUtil.RemoveTags(text);
            text = Sitecore.StringUtil.Clip(text, clipAt, useEllipsis);
            return text;
        }
    }
}
