using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.ApplicationModel.Environments;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.VisualStudio.Extensions.Text {

   static class Constants {
      public const String CLASSIF_NAME = "FlowControl";
      public const String LINQ_CLASSIF_NAME = "LinqOperator";
   }

   [Export(typeof(IClassifierProvider))]
   [ContentType(CSharp.ContentType)]
   [ContentType(Cpp.ContentType)]
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
      private IClassificationType keywordClassification;
      private IClassificationType linqClassification;
      private IClassifier classifier;

#pragma warning disable 67
      public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

      internal FlowControlClassifier(
            IClassificationTypeRegistryService registry, 
            IClassifier classifier) {
         keywordClassification = registry.GetClassificationType(Constants.CLASSIF_NAME);
         linqClassification = registry.GetClassificationType(Constants.LINQ_CLASSIF_NAME);
         this.classifier = classifier;
      }

      public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
         List<ClassificationSpan> list = new List<ClassificationSpan>();
         if ( span.IsEmpty ) return list;

         // find spans that the language service has already classified as keywords ...
         var classifiedSpans = 
            from cs in classifier.GetClassificationSpans(span)
            let name = cs.ClassificationType.Classification.ToLower()
            where name.Contains("keyword")
            select cs.Span;

         ILanguageKeywords keywords = 
            GetKeywordsByContentType(span.Snapshot.TextBuffer.ContentType);

         // ... and from those, ones that match our keywords
         var controlFlowSpans = from kwSpan in classifiedSpans
                                where keywords.ControlFlow.Contains(kwSpan.GetText())
                                select kwSpan;

         list.AddRange(controlFlowSpans.Select(
               cfs => new ClassificationSpan(cfs, keywordClassification)
            ));
         var linqSpans = from kwSpan in classifiedSpans
                         where keywords.Linq.Contains(kwSpan.GetText())
                         select kwSpan;

         list.AddRange(linqSpans.Select(
               cfs => new ClassificationSpan(cfs, linqClassification)
            ));
         return list;
      }

      private ILanguageKeywords GetKeywordsByContentType(IContentType contentType) {
         if ( contentType.TypeName == CSharp.ContentType ) {
            return new CSharp();
         } else if ( contentType.TypeName == Cpp.ContentType ) {
            return new Cpp();
         }
         throw new InvalidOperationException("Running into an unsupported editor");
      }
   }
}
