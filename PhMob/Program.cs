using System.Globalization;

namespace PhMob
{
    public class Program
    {
   
        static void Main(string[] args)
        {
         
            Station ats = new("ATS Station");
            Console.WriteLine();

          
            UI.Menu(ats);
        }


    }
}