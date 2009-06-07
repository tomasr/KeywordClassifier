using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
#pragma warning restore 649

      //returns an instance of the classifier
      public IClassifier GetClassifier(ITextBuffer buffer, IEnvironment context) {
         return buffer.Properties.GetOrCreateSingletonProperty<ControlFlowClassifier>(
            delegate { 
               return new ControlFlowClassifier(ClassificationRegistry); 
            });
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

      internal ControlFlowClassifier(IClassificationTypeRegistryService registry) {
         _classificationType = registry.GetClassificationType("ControlFlow");
      }

      public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span) {
         List<ClassificationSpan> classifications = new List<ClassificationSpan>();
         String text = span.GetText();

         foreach ( String kw in KEYWORDS ) {
            FindKeyword(text, kw, start => {
               Span loc = new Span(span.Start.Position+start, kw.Length);
               SnapshotSpan sspan = new SnapshotSpan(span.Snapshot, loc);
               classifications.Add(new ClassificationSpan(sspan, _classificationType));
            });
         }
         return classifications;
      }

      private void FindKeyword(String text, String keyword, Action<int> action) {
         int lastPos = 0;
         while ( true ) {
            bool found = false;
            int pos = text.IndexOf(keyword, lastPos);
            if ( pos < 0 ) {
               break;
            }
            if ( pos > 0 && IsDelimiter(text[pos - 1]) ) {
               if ( text.Length > pos + keyword.Length + 1 ) {
                  if ( IsDelimiter(text[pos+keyword.Length]) ) {
                     found = true;
                  }
               }
            }
            if ( found ) {
               action(pos);
            }
            lastPos = pos + 1;
         }
      }
      private bool IsDelimiter(char c) {
         return Char.IsWhiteSpace(c) || !Char.IsLetterOrDigit(c);
      }
   }
}
