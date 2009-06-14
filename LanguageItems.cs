using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Winterdom.VisualStudio.Extensions.Text {
   interface ILanguageKeywords {
      String[] ControlFlow { get; }
      String[] Linq { get; }
      String[] Visibility { get; }
   }

   class CSharp : ILanguageKeywords {
      public const String ContentType = "CSharp";
      static readonly String[] CS_KEYWORDS = {
         "if", "else", "while", "do", "for", "foreach", 
         "switch", "break", "continue", "return", "goto", "throw" 
      };
      static readonly String[] CS_LINQ_KEYWORDS = {
         "select", "let", "where", "join", "orderby", "group",
         "by", "on", "equals", "into", "from", "descending",
         "ascending"
      };
      static readonly String[] CS_VIS_KEYWORDS = {
         "public", "private", "protected", "internal"
      };
      public String[] ControlFlow {
         get { return CS_KEYWORDS; }
      }
      public String[] Linq {
         get { return CS_LINQ_KEYWORDS; }
      }
      public String[] Visibility {
         get { return CS_VIS_KEYWORDS; }
      }
   }
   class Cpp : ILanguageKeywords {
      public const String ContentType = "C/C++";
      static readonly String[] CPP_KEYWORDS = {
         "if", "else", "while", "do", "for", "each", "switch",
         "break", "continue", "return", "goto", "throw"
      };
      static readonly String[] CPP_VIS_KEYWORDS = {
         "public", "private", "protected", "internal"
      };
      public String[] ControlFlow {
         get { return CPP_KEYWORDS; }
      }
      public String[] Linq {
         get { return new String[0]; }
      }
      public String[] Visibility {
         get { return CPP_VIS_KEYWORDS; }
      }
   }
}
