using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Media;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.VisualStudio.Extensions.Text {

   static class Constants {
      public const String CLASSIF_NAME = "CSharpFlowControl";
      public const String CONTENT_TYPE_CS = "CSharp";
      public const String CONTENT_TYPE_CPP = "C/C++";

      public static readonly String[] CS_KEYWORDS = {
         "if", "else", "while", "do", "for", "foreach", 
         "switch", "break", "continue", "return", "goto", "throw" 
      };
      public static readonly String[] CPP_KEYWORDS = {
         "if", "else", "while", "do", "for", "each", "switch",
         "break", "continue", "return", "goto", "throw"
      };
   }

   static class FlowControlClassificationDefinition {
      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Constants.CLASSIF_NAME)]
      internal static ClassificationTypeDefinition ControlFlowClassificationType = null;
   }

   [Export(typeof(EditorFormatDefinition))]
   [ClassificationType(ClassificationTypeNames = Constants.CLASSIF_NAME)]
   [Name(Constants.CLASSIF_NAME)]
   [DisplayName("Flow Control Keyword")]
   [UserVisible(true)]
   [Order(After = Priority.High)]
   sealed class FlowControlFormat : ClassificationFormatDefinition {
      public FlowControlFormat() {
         this.ForegroundColor = Colors.MediumTurquoise;
         this.IsItalic = true;
      }
   }

   [Export(typeof(IClassifierProvider))]
   [ContentType(Constants.CONTENT_TYPE_CS)]
   [ContentType(Constants.CONTENT_TYPE_CPP)]
   class FlowControlClassifierProvider : IClassifierProvider {
      [Import]
      internal IClassificationTypeRegistryService ClassificationRegistry = null;
      [Import]
      internal IClassifierAggregatorService Aggregator = null;
      private static bool ignoreRequest = false;

      public IClassifier GetClassifier(ITextBuffer buffer, IEnvironment context) {
         // ignoreRequest ensures that our own classifier doesn't get added when we 
         // go through the Aggregator Service below.
         if ( ignoreRequest ) return null;
         try {
            ignoreRequest = true;
            return buffer.Properties.GetOrCreateSingletonProperty<FlowControlClassifier>(
               delegate {
                  return new FlowControlClassifier(
                     ClassificationRegistry,
                     Aggregator.GetClassifier(buffer, context)
                  );
               });
         } finally {
            ignoreRequest = false;
         }
      }
   }

   class FlowControlClassifier : IClassifier {
      private IClassificationType _classificationType;
      private IClassifier _classifier;

#pragma warning disable 67
      public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

      internal FlowControlClassifier(
            IClassificationTypeRegistryService registry, 
            IClassifier classifier) {
               _classificationType = registry.GetClassificationType(Constants.CLASSIF_NAME);
         _classifier = classifier;
      }

      public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
         if ( span.IsEmpty ) return new List<ClassificationSpan>();

         // find spans that the language service has already classified as keywords ...
         var classifiedSpans = 
            from cs in _classifier.GetClassificationSpans(span)
            let name = cs.ClassificationType.Classification.ToLower()
            where name.Contains("keyword")
            select cs.Span;

         String[] keywords = 
            GetKeywordsByContentType(span.Snapshot.TextBuffer.ContentType);

         // ... and from those, ones that match our keywords
         var controlFlowSpans = from kwSpan in classifiedSpans
                                where keywords.Contains(kwSpan.GetText())
                                select kwSpan;

         return controlFlowSpans.Select(
               cfs => new ClassificationSpan(cfs, _classificationType)
            ).ToList();
      }

      private string[] GetKeywordsByContentType(IContentType contentType) {
         switch ( contentType.TypeName ) {
         case Constants.CONTENT_TYPE_CS:
            return Constants.CS_KEYWORDS;
         case Constants.CONTENT_TYPE_CPP:
            return Constants.CPP_KEYWORDS;
         default:
            throw new InvalidOperationException("Running into an unsupported editor");
         }
      }
   }
}
