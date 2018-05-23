namespace GeneticAlgorithm
{
    public interface IDna<T>
    {
        T[] Genes { get; set; }
        float Fitness { get; set; }

        void CalculateFitness(T[] target);
        IDna<T> Crossover(IDna<T> partner);
        void Mutate(float mutationRate);
    }
}