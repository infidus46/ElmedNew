using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotPoint
{
    class Program
    {
        static void Main(string[] args)
        {

            var commandLine = new CommandLineArguments(args);

            var IdSchet = commandLine["IdSchet"];
            var raschet = commandLine["raschet"];


        }
    }
}
