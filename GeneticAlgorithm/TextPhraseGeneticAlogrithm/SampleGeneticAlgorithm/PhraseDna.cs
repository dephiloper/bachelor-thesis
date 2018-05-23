using System;

namespace GeneticAlgorithm
{
    public class PhraseDna : IDna<char>
    {
        public char[] Genes { get; set; }
        public float Fitness { get; set; }

        private const string Chars = "1234567890ABCDEFGHIJKLMNOPQRSTUVWXYZÄÜÖ?!abcdefghijklmnopqrstuvwxyzäüöß,. ";
        private static readonly Random Random = new Random();

        public PhraseDna(int count)
        {
            Genes = new char[count];
            
            // prepopulation
            for (var i = 0; i < count; i++)
                Genes[i] = Chars[Random.Next(Chars.Length)];
        }
        
        public void CalculateFitness(char[] target)
        {
            var score = 0;

            for (var i = 0; i < Genes.Length; i++)
                score += Genes[i] == target[i] ? 1 : 0;

            Fitness = (float)score / Genes.Length;
            Fitness = (float) Math.Pow(Fitness, 4);
        }

        public IDna<char> Crossover(IDna<char> partner)
        {
            var midpoint = Random.Next(Genes.Length);
            var child = new PhraseDna(Genes.Length);
            
            for (var i = 0; i < Genes.Length; i++)
            {
                if (i < midpoint) child.Genes[i] = partner.Genes[i];
                else child.Genes[i] = Genes[i];
            }

            return child;
        }

        public void Mutate(float mutationRate)
        {
            for (var i = 0; i < Genes.Length; i++)
                if (Random.NextDouble() < mutationRate)
                    Genes[i] = Chars[Random.Next(Chars.Length)];
        }

        public override string ToString()
        {
            return new string(Genes);
        }
    }
}