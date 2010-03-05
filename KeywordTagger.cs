using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.VisualStudio.Extensions.Text {

   static class Constants {
      public const String CLASSIF_NAME = "Keyword - Flow Control";
      public const String LINQ_CLASSIF_NAME = "Operator - LINQ";
      public const String VISIBILITY_CLASSIF_NAME = "Keyword - Visibility";
   }

   [Export(typeof(IViewTaggerProvider))]
   [ContentType(CSharp.ContentType)]
   [ContentType(Cpp.ContentType)]
   [TagType(typeof(ClassificationTag))]
   public class KeywordTaggerProvider : IViewTaggerProvider {
      [Import]
      internal IClassificationTypeRegistryService ClassificationRegistry = null;
      [Import]
      internal IBufferTagAggregatorFactoryService Aggregator = null;

      public ITagger<T> CreateTagger<T>(ITextView textView, ITextBuffer buffer) where T : ITag {
         return new KeywordTagger(
            ClassificationRegistry,
            Aggregator.CreateTagAggregator<ClassificationTag>(buffer)
         ) as ITagger<T>;
      }
   }

   class KeywordTagger : ITagger<ClassificationTag> {
      private ClassificationTag keywordClassification;
      private ClassificationTag linqClassification;
      private ClassificationTag visClassification;
      private ITagAggregator<ClassificationTag> aggregator;
      private static readonly IList<ClassificationSpan> EmptyList = 
         new List<ClassificationSpan>();

#pragma warning disable 67
      public event EventHandler<SnapshotSpanEventArgs> TagsChanged;
#pragma warning restore 67

      internal KeywordTagger(
            IClassificationTypeRegistryService registry, 
            ITagAggregator<ClassificationTag> aggregator) {
         keywordClassification = 
            new ClassificationTag(registry.GetClassificationType(Constants.CLASSIF_NAME));
         linqClassification = 
            new ClassificationTag(registry.GetClassificationType(Constants.LINQ_CLASSIF_NAME));
         visClassification = 
            new ClassificationTag(registry.GetClassificationType(Constants.VISIBILITY_CLASSIF_NAME));
         this.aggregator = aggregator;
      }

      public IEnumerable<ITagSpan<ClassificationTag>> GetTags(NormalizedSnapshotSpanCollection spans) {
         if ( spans.Count == 0 ) {
            yield break;
         }
         ITextSnapshot snapshot = spans[0].Snapshot;
         LanguageKeywords keywords =
            GetKeywordsByContentType(snapshot.TextBuffer.ContentType);
         if ( keywords == null ) {
            yield break;
         }

         // find spans that the language service has already classified as keywords ...
         var mappedSpans =
            from tagSpan in aggregator.GetTags(spans)
            let name = tagSpan.Tag.ClassificationType.Classification.ToLower()
            where name.Contains("keyword")
            select tagSpan.Span;
         var classifiedSpans =
            from mappedSpan in mappedSpans
            let cs = mappedSpan.GetSpans(snapshot)
            where cs.Count > 0
            select cs[0];

         // ... and from those, ones that match our keywords
         foreach ( var cs in classifiedSpans ) {
            String text = cs.GetText();
            if ( keywords.ControlFlow.Contains(text) ) {
               yield return new TagSpan<ClassificationTag>(cs, keywordClassification);
            } else if ( keywords.Visibility.Contains(text) ) {
               yield return new TagSpan<ClassificationTag>(cs, visClassification);
            } else if ( keywords.Linq.Contains(text) ) {
               yield return new TagSpan<ClassificationTag>(cs, linqClassification);
            }
         }
      }

      private LanguageKeywords GetKeywordsByContentType(IContentType contentType) {
         if ( contentType.IsOfType(CSharp.ContentType) ) {
            return new CSharp();
         } else if ( contentType.IsOfType(Cpp.ContentType) ) {
            return new Cpp();
         }
         // VS is calling us for the "CSharp Signature Help" content-type
         // which we didn't ask for. Argh!!!
         // throw new InvalidOperationException("Running into an unsupported editor");
         return null;
      }
   }
}
