using FieldFallback.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Validators;

namespace FieldFallback.Validators
{
   public class FallbackValidator : StandardValidator
   {
      protected override ValidatorResult Evaluate()
      {
         Field field = GetField();

         if (field != null && FallbackValuesManager.Provider != null)
         {
            if (FallbackValuesManager.Provider.FieldContainsFallbackValue(field, ItemUri.Language))
            {
               Text = GetText("Field \"{0}\" contains a fallback value", new[] { GetFieldDisplayName() });
               return GetFailedResult(ValidatorResult.Warning);
            }
         }

         return ValidatorResult.Valid;
      }

      protected override ValidatorResult GetMaxValidatorResult()
      {
         return GetFailedResult(ValidatorResult.Warning);
      }

      public override string Name
      {
         get
         {
            return "Fallback";
         }
      }
   }
}
