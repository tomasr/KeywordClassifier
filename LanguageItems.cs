using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;

namespace Winterdom.VisualStudio.Extensions.Text {
   abstract class LanguageKeywords {
      private Dictionary<string, string[]> keywords =
         new Dictionary<string, string[]>();

      public String[] ControlFlow {
         get { return Get("ControlFlow", ControlFlowDefaults); }
      }
      public String[] Linq {
         get { return Get("Linq", LinqDefaults); }
      }
      public String[] Visibility {
         get { return Get("Visibility", VisibilityDefaults); }
      }

      protected abstract String[] ControlFlowDefaults { get; }
      protected abstract String[] LinqDefaults { get; }
      protected abstract String[] VisibilityDefaults { get; }
      protected abstract String KeyName { get; }

      protected String[] Get(String name, String[] defaults) {
         if ( !keywords.ContainsKey(name) ) {
            String[] values = 
               ConfigHelp.GetValue(KeyName + "_" + name, "").AsList();
            if ( values == null || values.Length == 0 )
               values = defaults;
            keywords[name] = values;
         }
         return keywords[name];
      }
   }

   static class ConfigHelp {
      private static string REG_KEY = 
         "Software\\Winterdom\\VS Extensions\\KeywordClassifier";
      public static String GetValue(String name, String defValue) {
         using ( RegistryKey key = Registry.CurrentUser.CreateSubKey(REG_KEY) ) {
            String value = key.GetValue(name, defValue) as String;
            if ( String.IsNullOrEmpty(defValue) ) value = defValue;
            return value;
         }
      }
   }

   static class StringExtensions {
      public static String[] AsList(this String str) {
         return str.Split(new Char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
      }
   }

   class CSharp : LanguageKeywords {
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
      protected override String[] ControlFlowDefaults {
         get { return CS_KEYWORDS; }
      }
      protected override String[] LinqDefaults {
         get { return CS_LINQ_KEYWORDS; }
      }
      protected override String[] VisibilityDefaults {
         get { return CS_VIS_KEYWORDS; }
      }
      protected override string KeyName {
         get { return "CSharp"; }
      }
   }
   class Cpp : LanguageKeywords {
      public const String ContentType = "C/C++";
      static readonly String[] CPP_KEYWORDS = {
         "if", "else", "while", "do", "for", "each", "switch",
         "break", "continue", "return", "goto", "throw"
      };
      static readonly String[] CPP_VIS_KEYWORDS = {
         "public", "private", "protected", "internal"
      };
      protected override String[] ControlFlowDefaults {
         get { return CPP_KEYWORDS; }
      }
      protected override String[] LinqDefaults {
         get { return new String[0]; }
      }
      protected override String[] VisibilityDefaults {
         get { return CPP_VIS_KEYWORDS; }
      }
      protected override string KeyName {
         get { return "Cpp"; }
      }
   }
}
