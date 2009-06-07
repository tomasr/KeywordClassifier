using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.VisualStudio.Extensions.Text {
   #region Format definition

   [Export(typeof(EditorFormatDefinition))]
   [ClassificationType(ClassificationTypeNames = "ControlFlow")]
   [Name("Control Flow")]
   [DisplayName("C# Control Flow Keywords")]
   [UserVisible(true)]
   [Order(After = Priority.High)]
   internal sealed class ControlFlowFormat : ClassificationFormatDefinition {

      public ControlFlowFormat() {
         this.ForegroundColor = Colors.DeepSkyBlue;
         this.IsItalic = true;
      }
   }
   #endregion //Format definition
}
