using Encog.Engine.Network.Activation;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Random = UnityEngine.Random;

public class Brain
{
    public double Fitness { get; set; }
    public double Score { get; set; }

    public BasicNetwork Network { get; private set; }

    public Brain()
    {
        Setup();
    }

    private Brain(double[] weights)
    {
        Setup();
        Network.Flat.Weights = weights;
    }

    private void Setup()
    {
        Network = new BasicNetwork();
        Network.AddLayer(new BasicLayer(null, true, 5));
        Network.AddLayer(new BasicLayer(new ActivationReLU(), true, 10));
        Network.AddLayer(new BasicLayer(new ActivationSigmoid(), false, 4));
        Network.Structure.FinalizeStructure();
        Network.Reset();
    }

    public Action Think(Percept percept)
    {
        var mlData = new BasicMLData(percept.ToDoubleArray());
        var result = Network.Compute(mlData);
        return new Action((result as BasicMLData)?.Data);
    }

    public void Mutate()
    {
        for (var i = 0; i < Network.Flat.Weights.Length; i++)
            if (Random.value < GameManager.Instance.MutationRate) {
                Network.Flat.Weights[i] = Network.Flat.Weights[i] + (Random.value - 0.5f);
            }
    }

    public Brain Crossover(Brain partner)
    {
        var weights = Network.Flat.Weights;
        var partnerWeights = partner.Network.Flat.Weights;
        
        var midpoint = Random.value * partnerWeights.Length;
        
        var childWeights = new double[partnerWeights.Length];
            
        for (var i = 0; i < partnerWeights.Length; i++)
        {
            if (i < midpoint) childWeights[i] = partnerWeights[i];
            else childWeights[i] = weights[i];
        }

        return new Brain(childWeights);
        
    }
}