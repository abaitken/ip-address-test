using System;
using System.Linq;
using System.IO;


namespace IPAddressGenerator
{
    class App
    {
        public void Run(string[] args)
        {
            var followSpec = args.Length == 0 || !args[0].Equals("NoSpec", StringComparison.CurrentCultureIgnoreCase);
            var generator = new ProblemGenerator(followSpec);

            var problems = from number in Enumerable.Range(1, 100)
                           let problem = generator.Generate()
                           select problem;

            var problemList = problems.ToList();

            var problemTextTableBuilder = new TextTableBuilder<Problem>();
            problemTextTableBuilder.AddColumn("Address", i => i.IP);

            File.WriteAllText("Problems.txt", problemTextTableBuilder.Format(problemList));

            var solutionTextTableBuilder = new TextTableBuilder<Problem>();
            solutionTextTableBuilder.AddColumn("Address", i => i.IP);
            solutionTextTableBuilder.AddColumn("Subnet Mask", i => i.SubnetMask);
            solutionTextTableBuilder.AddColumn("Network ID", i => i.NetworkID);
            solutionTextTableBuilder.AddColumn("Host ID", i => i.HostID);

            File.WriteAllText("Solutions.txt", solutionTextTableBuilder.Format(problemList));

            Console.WriteLine(@"Two files have been written to the current directory, Problems.txt and Solutions.txt
Problems.txt contains a list of IP addresses with subnet masks in the CDN notation.
The task is to workout what the Subnet Mask and the Network ID is from this information.

The Solutions to each problem are provided in the Solutions.txt file.

Press any key to exit...");
            Console.ReadKey();
        }
    }
}
