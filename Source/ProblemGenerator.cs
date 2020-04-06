using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPAddressGenerator
{
    class ProblemGenerator
    {
        private readonly Random random;
        private readonly bool followSpec;

        public ProblemGenerator(bool followSpec)
        {
            this.followSpec = followSpec;
            random = new Random((int)DateTime.Now.Ticks);
        }

        public Problem Generate()
        {
            var ip = new IPAddress(
                GenerateIPClass(),
                GenerateRandomNumber(1, 254),
                GenerateRandomNumber(1, 254),
                GenerateRandomNumber(1, 254));

            var subnetBits = GenerateSubnetMask(ip.Part1);

            var subnetMask = ToSubnetMask(subnetBits);
            var networkId = BitwiseAND(ip, subnetMask);
            var hostId = GetHostId(ip, subnetMask);
            return new Problem
            {
                IP = string.Format("{0}/{1}", ip, subnetBits),
                SubnetMask = subnetMask.ToString(),
                NetworkID = networkId.ToString(),
                HostID = hostId.ToString()
            };
        }

        private int GenerateIPClass()
        {
            if (!followSpec)
                return GenerateRandomNumber(1, 255);
            var result = GenerateRandomNumber(1, 223);
            if (result == 127)
                return 128;
            return result;
        }

        private int GenerateSubnetMask(int ipPart1)
        {
            if (!followSpec)
                return GenerateRandomNumber(1, 32);

            if (ipPart1 >= 1 && ipPart1 <= 126)
                return GenerateRandomNumber(1, 8);
            if (ipPart1 >= 128 && ipPart1 <= 192)
                return GenerateRandomNumber(1, 16);
            if (ipPart1 >= 192 && ipPart1 <= 223)
                return GenerateRandomNumber(1, 24);
            throw new InvalidOperationException();
        }

        private IPAddress GetHostId(IPAddress ip, IPAddress subnetMask)
        {
            var subnetComplement = new IPAddress(
                                        ~subnetMask.Part1,
                                        ~subnetMask.Part2,
                                        ~subnetMask.Part3,
                                        ~subnetMask.Part4);
            return BitwiseAND(ip, subnetComplement);
        }

        private IPAddress BitwiseAND(IPAddress ip, IPAddress subnetMask)
        {
            return new IPAddress(
                ip.Part1 & subnetMask.Part1,
                ip.Part2 & subnetMask.Part2,
                ip.Part3 & subnetMask.Part3,
                ip.Part4 & subnetMask.Part4);
        }

        private IPAddress ToSubnetMask(int bits)
        {
            var remainingBits = bits;
            var part1 = GetSubnetPart(remainingBits, out remainingBits);
            var part2 = GetSubnetPart(remainingBits, out remainingBits);
            var part3 = GetSubnetPart(remainingBits, out remainingBits);
            var part4 = GetSubnetPart(remainingBits, out remainingBits);
            return new IPAddress(part1, part2, part3, part4);
        }

        private int GetSubnetPart(int totalBits, out int remainingBits)
        {
            if (totalBits <= 0)
            {
                remainingBits = 0;
                return 0;
            }

            if (totalBits >= 8)
            {
                remainingBits = totalBits - 8;
                return 255;
            }

            var bitNumbers = new[] { 128, 64, 32, 16, 8, 4, 2, 1 };

            var result = bitNumbers.Take(totalBits).Sum();
            remainingBits = 0;
            return result;
        }

        private int GenerateRandomNumber(int min, int max)
        {
            return random.Next(max - 1) + 1;
        }
    }

    class IPAddress
    {
        private readonly int part1;
        private readonly int part2;
        private readonly int part3;
        private readonly int part4;

        public IPAddress(int part1, int part2, int part3, int part4)
        {
            this.part1 = part1;
            this.part2 = part2;
            this.part3 = part3;
            this.part4 = part4;
        }

        public int Part1 { get { return part1; } }
        public int Part2 { get { return part2; } }
        public int Part3 { get { return part3; } }
        public int Part4 { get { return part4; } }

        public override string ToString()
        {
            return string.Format("{0}.{1}.{2}.{3}", part1, part2, part3, part4);
        }
    }
}
