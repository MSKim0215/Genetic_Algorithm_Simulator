using System;
using System.Collections.Generic;
using System.Linq;

namespace MinSeob.Genetic
{
    public class GeneticManager<T>
    {
        private int generation;        // 세대
        private float bestFitness;     // 최고 적합도
        private T[] bestGenes;          // 최고 유전자
        private float mutateRatio;     // 돌연변이 발생율
        private float totalFitness;    // 적합도 총합

        private Random random;

        public List<DNA<T>> CurrentPopulation { private set; get; } = new();     // 현재 세대 인구수
        public List<DNA<T>> NextPopulation { private set; get; } = new();        // 다음 세대 인구수

        public GeneticManager(int populationSize, int dnaSize, Random random, Func<T> getRandomGene, Func<float, int> fitnessFunction, float mutateRatio = 0.01f)
        {
            generation = 1;
            CurrentPopulation = new();
            this.random = random;
            this.mutateRatio = mutateRatio;

            for(int i = 0; i < populationSize; i++)
            {
                CurrentPopulation.Add(new(dnaSize, random, getRandomGene, fitnessFunction, isInitGene: true));
            }
        }

        public void NextGenerate()
        {
            if (CurrentPopulation.Count <= 0) return;

            CalcutateFitness();

            if(NextPopulation.Count > 0)
            {
                NextPopulation.Clear();
            }

            for(int i = 0; i < CurrentPopulation.Count; i++)
            {
                DNA<T> parentA = ChooseParent();
                DNA<T> parentB = ChooseParent();

                DNA<T> child = parentA.CrossOver(parentB);
                child.Mutate(mutateRatio);

                NextPopulation.Add(child);
            }

            CurrentPopulation = NextPopulation;
            generation++;
        }

        public void CalcutateFitness()
        {
            totalFitness = 0f;
            DNA<T> bestDNA = CurrentPopulation.FirstOrDefault();

            for(int i = 0; i < CurrentPopulation.Count; i++)
            {
                totalFitness += CurrentPopulation[i].CalcuateFitness(i);

                if (bestDNA.Fitness < CurrentPopulation[i].Fitness)
                {
                    bestDNA = CurrentPopulation[i];
                }
            }

            bestFitness = bestDNA.Fitness;
            bestDNA.Genes.CopyTo(bestGenes, 0);
        }

        public DNA<T> ChooseParent()
        {
            double randomNumber = random.NextDouble() * totalFitness;

            for(int i = 0; i < CurrentPopulation.Count; i++)
            {
                if(randomNumber < CurrentPopulation[i].Fitness)
                {
                    return CurrentPopulation[i];
                }

                randomNumber -= CurrentPopulation[i].Fitness;
            }
            return null;
        }
    }
}