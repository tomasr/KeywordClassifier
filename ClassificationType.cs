using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Winterdom.VisualStudio.Extensions.Text {
   internal static class ControlFlowClassificationDefinition {
      #region Type definition

#pragma warning disable 649 //As an export, this will not have a value

      [Export(typeof(ClassificationTypeDefinition))]
      [Name("ControlFlow")]
      internal static ClassificationTypeDefinition ControlFlowClassificationType;
#pragma warning restore 649

      #endregion
   }
}
