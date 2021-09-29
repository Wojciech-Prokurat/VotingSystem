using System;
using System.Collections.Generic;
using System.Linq;
using VotingSystem.Models;
// 3 Rules of TDD
//1. You are not allowed to write any production code unless it is to make a failing unit test pass.
//2. You are not allowed to write any more of a unit test than is sufficient to fail; and compilation failures are failures.
//3. You are not allowed to write any more production code than is sufficient to pass the one failing unit test.


namespace VotingSystem
{
    public class CounterManager
    {
        public Counter GetStatistics(Counter counter, int totalCount)
        {
            counter.Percent = RoundUp(counter.Count * 100.0 / totalCount);
            return counter;
        }
        public void ResolveExcess(List<Counter> counters)
        {
            var totalPercent = counters.Sum(x => x.Percent);
            if (totalPercent  == 100) return;

            var excess = 100 - totalPercent;

            var highestPercent = counters.Max(x => x.Percent);
            var highestCounters = counters.Where(x => x.Percent == highestPercent).ToList();
            if(highestCounters.Count == 1)
            {
                counters[0].Percent += excess;
            }
            else if(highestCounters.Count < counters.Count)
            {
                var lowestPercent = counters.Min(x => x.Percent);
                var lowestCounter = counters.First(x => x.Percent == lowestPercent);
                lowestCounter.Percent = RoundUp(lowestCounter.Percent + excess);
            }
        }

         private static double RoundUp(double num) => Math.Round(num,2);
    }
}
