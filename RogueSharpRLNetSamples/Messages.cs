using System.Collections.Generic;
using System.Linq;
using RLNET;

namespace RogueSharpRLNetSamples
{
   public class Messages
   {
      private readonly Queue<string> _lines;

      public Messages()
      {
         _lines = new Queue<string>();
      }

      public void Add( string message )
      {
         _lines.Enqueue( message );
         if ( _lines.Count > 5 )
         {
            _lines.Dequeue();
         }
      }

      public void Draw( RLConsole console )
      {
         console.Clear();
         string[] lines = _lines.ToArray();
         for ( int i = 0; i < lines.Count(); i++ )
         {
            console.Print( 1, i, lines[i], RLColor.White );
         }
      }
   }
}