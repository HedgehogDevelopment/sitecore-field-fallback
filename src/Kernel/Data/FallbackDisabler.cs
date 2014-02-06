using Sitecore.Common;

namespace FieldFallback.Data
{
    public enum FallbackStates
    {
        /// <summary>
        /// Fallback is not forced to disabled
        /// </summary>
        Default,

        /// <summary>
        /// Fallback is disabled
        /// </summary>
        Disabled
    }

    /// <summary>
    /// Provides the ability to disable field fallback
    /// </summary>
    public class FallbackDisabler : Switcher<FallbackStates>
    {
        public FallbackDisabler()
            : base(FallbackStates.Disabled)
        {
        }

        public FallbackDisabler(FallbackStates state)
            : base (state)
        {
        }

    }
}
