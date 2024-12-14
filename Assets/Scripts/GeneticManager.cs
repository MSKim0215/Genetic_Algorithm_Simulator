using System;
using System.Collections.Generic;
using System.Linq;

namespace MinSeob.Genetic
{
    public class GeneticManager<T>
    {
        public List<DNA<T>> CurrentPopulation { get; private set; }
        public int Generation { get; private set; }
        public float BestFitness { get; private set; }
        public T[] BestGenes { get; private set; }

        public int Elitism;
        public float MutationRate;

        private List<DNA<T>> NextPopulation;
        private Random random;
        private float totalFitness;
        private int dnaSize;
        private Func<T> getRandomGene;
        private Func<int, float> fitnessFunction;

        public GeneticManager(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<int, float> fitnessFunction,
            int elitism, float mutationRate = 0.01f)
        {
            Generation = 1;
            Elitism = elitism;
            MutationRate = mutationRate;
            CurrentPopulation = new(populationSize);
            NextPopulation = new(populationSize);
            this.random = random;
            this.dnaSize = dnaSize;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;

            BestGenes = new T[dnaSize];

            for (int i = 0; i < populationSize; i++)
            {
                CurrentPopulation.Add(new(dnaSize, random, getRandomGene, fitnessFunction));
            }
        }

        public void NewGeneration(int numNewDNA = 0, bool crossoverNewDNA = false)
        {
            int finalCount = CurrentPopulation.Count + numNewDNA;
            if (finalCount <= 0) return;

            if (CurrentPopulation.Count > 0)
            {
                CalculateFitness();
                CurrentPopulation.Sort(CompareDNA);
            }
            NextPopulation.Clear();

            for (int i = 0; i < CurrentPopulation.Count; i++)
            {
                if (i < Elitism && i < CurrentPopulation.Count)
                {
                    NextPopulation.Add(CurrentPopulation[i]);
                }
                else if (i < CurrentPopulation.Count || crossoverNewDNA)
                {
                    DNA<T> parent1 = ChooseParent();
                    DNA<T> parent2 = ChooseParent();

                    DNA<T> child = parent1.Crossover(parent2);

                    child.Mutate(MutationRate);

                    NextPopulation.Add(child);
                }
                else
                {
                    NextPopulation.Add(new DNA<T>(dnaSize, random, getRandomGene, fitnessFunction, shouldInitGenes: true));
                }
            }

            var tmpList = CurrentPopulation;
            CurrentPopulation = NextPopulation;
            NextPopulation = tmpList;

            Generation++;
        }

        private int CompareDNA(DNA<T> a, DNA<T> b)
        {
            if (a.Fitness > b.Fitness)
            {
                return -1;
            }
            else if (a.Fitness < b.Fitness)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        private void CalculateFitness()
        {
            totalFitness = 0;
            DNA<T> best = CurrentPopulation[0];

            for (int i = 0; i < CurrentPopulation.Count; i++)
            {
                totalFitness += CurrentPopulation[i].CalculateFitness(i);

                if (CurrentPopulation[i].Fitness > best.Fitness)
                {
                    best = CurrentPopulation[i];
                }
            }

            BestFitness = best.Fitness;
            best.Genes.CopyTo(BestGenes, 0);
        }

        private DNA<T> ChooseParent()
        {
            double randomNumber = random.NextDouble() * totalFitness;

            for (int i = 0; i < CurrentPopulation.Count; i++)
            {
                if (randomNumber < CurrentPopulation[i].Fitness)
                {
                    return CurrentPopulation[i];
                }

                randomNumber -= CurrentPopulation[i].Fitness;
            }

            return null;
        }
    }
}