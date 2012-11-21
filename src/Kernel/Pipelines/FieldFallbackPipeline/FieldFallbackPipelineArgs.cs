using Sitecore.Data.Fields;
using Sitecore.Globalization;
using Sitecore.Pipelines;

namespace FieldFallback.Pipelines.FieldFallbackPipeline
{
    public class FieldFallbackPipelineArgs : PipelineArgs
    {
        public enum Methods { Query, Execute }

        private string _fallbackValue;

        public Methods Method { get; private set; }

        public Field Field { get; private set; }

        public Language Language { get; set; }

        public string FallbackValue
        {
            get
            {
                return _fallbackValue;
            }
            set
            {
                _fallbackValue = value;
                HasFallbackValue = value != null;
            }
        }

        public bool HasFallbackValue { get; set; }

        public FieldFallbackPipelineArgs(Methods method, Field field)
        {
            Method = method;
            Field = field;
        }
    }
}
