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

   [Export(typeof(IClassifierProvider))]
   [ContentType("CSharp")]
   internal class ControlFlowClassifierProvider : IClassifierProvider {

#pragma warning disable 649 //because of the import this is not an empty reference
      [Import]
      internal IClassificationTypeRegistryService ClassificationRegistry;
      [Import]
      private IClassifierAggregatorService Aggregator;
#pragma warning restore 649
      static bool ignoreRequest = false;

      //returns an instance of the classifier
      public IClassifier GetClassifier(ITextBuffer buffer, IEnvironment context) {
         if ( ignoreRequest ) return null;
         try {
            ignoreRequest = true;
            return buffer.Properties.GetOrCreateSingletonProperty<ControlFlowClassifier>(
               delegate {
                  return new ControlFlowClassifier(
                     ClassificationRegistry,
                     Aggregator.GetClassifier(buffer, context)
                  );
               });
         } finally {
            ignoreRequest = false;
         }
      }
   }

   class ControlFlowClassifier : IClassifier {
      static readonly String[] KEYWORDS = {
         "if", "else", "while", "do", 
         "for", "foreach"
      };

#pragma warning disable 67

      public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

      IClassificationType _classificationType;
      IClassifier _classifier;

      internal ControlFlowClassifier(IClassificationTypeRegistryService registry, IClassifier classifier) {
         _classificationType = registry.GetClassificationType("ControlFlow");
         _classifier = classifier;
      }

      public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
         if ( span.IsEmpty ) return new List<ClassificationSpan>();

         // find spans that the C# language service has already classified as keywords ...
         var classifiedSpans = from cs in _classifier.GetClassificationSpans(span)
                               let name = cs.ClassificationType.Classification.ToLower()
                               where name.Contains("keyword")
                               select cs.Span;

         // ... and from those, ones that match our keywords
         var controlFlowSpans = from kwSpan in classifiedSpans
                                where KEYWORDS.Contains(kwSpan.GetText())
                                select kwSpan;

         return controlFlowSpans.Select(
               cfs => new ClassificationSpan(cfs, _classificationType)
            ).ToList();
      }
   }
}
