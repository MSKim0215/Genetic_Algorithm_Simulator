using System;
using System.Collections.Generic;

namespace MinSeob.Genetic
{
    public class DNA<T>
    {
        private Random random;
        private Func<T> getRandomGene;
        private Func<float, int> fitnessFunction;

        public T[] Genes { private set; get; }             // 유전자
        public float Fitness { private set; get; }         // 적합도

        public DNA(int size, Random random, Func<T> getRandomGene, Func<float, int> fitnessFunction, bool isInitGene = true)
        {
            Genes = new T[size];

            this.random = random;
            this.getRandomGene = getRandomGene;
            this.fitnessFunction = fitnessFunction;

            if(isInitGene)
            {
                for(int i = 0; i < Genes.Length; i++)
                {
                    Genes[i] = getRandomGene();
                }
            }
        }

        public float CalcuateFitness(int index)
        {
            return Fitness = fitnessFunction(index);
        }

        public DNA<T> CrossOver(DNA<T> otherParent)
        {
            var child = new DNA<T>(Genes.Length, random, getRandomGene, fitnessFunction, isInitGene: false);

            for(int i = 0; i < Genes.Length; i++)
            {
                child.Genes[i] = random.NextDouble() < 0.5f ? Genes[i] : otherParent.Genes[i];
            }

            return child;
        }

        public void Mutate(float mutateRatio)
        {
            for(int i = 0; i < Genes.Length; i++)
            {
                if(random.NextDouble() < mutateRatio)
                {
                    // TODO: Mutate Do SomeThing
                }
            }
        }
    }
}