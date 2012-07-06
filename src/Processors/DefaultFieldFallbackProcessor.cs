using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Data.Fields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Diagnostics;

namespace FieldFallback.Processors
{
    public class DefaultFieldFallbackProcessor : FieldFallbackProcessor
    {
        public override bool IsEnabledForField(Field field)
        {
            TemplateFallbackFieldItem fallbackField = field;
            return (fallbackField != null && fallbackField.EnableDefaultValueFallback);
        }

        public override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            Assert.IsNotNull(args.Field, "Field is null");
            TemplateFallbackFieldItem fallbackField = args.Field;

            string resultText = fallbackField.DefaultFallbackValue;
            MasterVariablesReplacer masterVariablesReplacer = Sitecore.Configuration.Factory.GetMasterVariablesReplacer();
            if (masterVariablesReplacer != null)
            {
                resultText = masterVariablesReplacer.Replace(resultText, args.Field.Item);

                if (resultText != null)
                {
                    // we found our value, don't execute any other processors.
                    args.AbortPipeline();
                }
            }
            return resultText;

        }
    }
}
