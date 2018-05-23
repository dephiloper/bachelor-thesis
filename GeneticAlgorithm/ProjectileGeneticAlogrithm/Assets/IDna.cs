public interface IDna<T>
{
    T[] Genes { get; set; }
    float Fitness { get; set; }
    int Punishment { get; set; }

    void CalculateFitness(T position, T target);
    IDna<T> Crossover(IDna<T> partner);
    void Mutate(float mutationRate);
    
}