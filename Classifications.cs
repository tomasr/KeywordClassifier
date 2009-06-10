using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.VisualStudio.Extensions.Text {

   static class FlowControlClassificationDefinition {
      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Constants.CLASSIF_NAME)]
      internal static ClassificationTypeDefinition FlowControlClassificationType = null;
   }
   static class LinqKeywordClassificationDefinition {
      [Export(typeof(ClassificationTypeDefinition))]
      [Name(Constants.LINQ_CLASSIF_NAME)]
      internal static ClassificationTypeDefinition LinqKeywordClassificationType = null;
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

   [Export(typeof(EditorFormatDefinition))]
   [ClassificationType(ClassificationTypeNames = Constants.LINQ_CLASSIF_NAME)]
   [Name(Constants.LINQ_CLASSIF_NAME)]
   [DisplayName("LINQ Keyword")]
   [UserVisible(true)]
   [Order(After = Priority.High)]
   sealed class LinqKeywordFormat : ClassificationFormatDefinition {
      public LinqKeywordFormat() {
         this.ForegroundColor = Colors.MediumSeaGreen;
      }
   }
}
