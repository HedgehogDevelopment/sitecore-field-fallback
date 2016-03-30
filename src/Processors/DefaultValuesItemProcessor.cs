using System;
using FieldFallback.Pipelines.FieldFallbackPipeline;
using FieldFallback.Processors.Data.Fields;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Data.Templates;
using Sitecore.Events;
using Sitecore.SecurityModel;

namespace FieldFallback.Processors
{
    public class DefaultValuesItemProcessor : FieldFallbackProcessor
    {
        protected override bool IsEnabledForField(Field field)
        {
            //This processor is always enabled because it is handled
            // by create and delete events
            return true;
        }

        protected override string GetFallbackValue(FieldFallbackPipelineArgs args)
        {
            return null; 
        }
    }
}