using FieldFallback.Shell;

namespace FieldFallback.Pipelines.RenderContentEditor
{
    public class RenderStandardContentEditor
    {
        public void Process(global::Sitecore.Shell.Applications.ContentEditor.Pipelines.RenderContentEditor.RenderContentEditorArgs args)
        {
            global::Sitecore.Diagnostics.Assert.ArgumentNotNull(args, "args");

            if (args.EditorFormatter is global::Sitecore.Shell.Applications.ContentManager.Dialogs.ResetFields.ResetFieldsFormatter ||
                args.EditorFormatter is global::Sitecore.Shell.Applications.ContentEditor.TranslatorFormatter)
            {
                args.EditorFormatter.RenderSections(args.Parent, args.Sections, args.ReadOnly);
            }
            else
            {
                var fallbackFormatter = new FallbackEditorFormatter
                {
                    Arguments = args.EditorFormatter.Arguments,
                    IsFieldEditor = args.EditorFormatter.IsFieldEditor
                };

                fallbackFormatter.RenderSections(args.Parent, args.Sections, args.ReadOnly);
            }
        }
    }
}