using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static Xunit.Assert;
// 3 Rules of TDD
//1. You are not allowed to write any production code unless it is to make a failing unit test pass.
//2. You are not allowed to write any more of a unit test than is sufficient to fail; and compilation failures are failures.
//3. You are not allowed to write any more production code than is sufficient to pass the one failing unit test.


namespace VotingSystem.Tests
{
    public class CounterManagerTests
    {
        public const string CounterName = "Counter Name";
        public Counter _counter = new Counter {Name = CounterName, Count = 5 };

        [Fact]
        public void GetCounterStatistics_IncludesCounterName()
        {
            var statistics = new CounterManager().GetStatistics(_counter,5);
            Equal(CounterName, statistics.Name);
        }
        [Fact]
        public void GetCounterStatistics_IncludesCounterCount()
        {
            var statistics = new CounterManager().GetStatistics(_counter, 5);
            Equal(5, statistics.Count);
        }
        [Theory]
        [InlineData(5,10,50)]
        [InlineData(1,3,33.33)]
        [InlineData(2,3,66.67)]
        public void GetCounterStatistics_ShowsPercentageUpToTwoDecimalsBasedOnTotalCount(int count, int total, double expected)
        {
            _counter.Count = count;

            var statistics = new CounterManager().GetStatistics(_counter, total);

            Equal(expected, statistics.Percent);
        }


        [Fact]
        public void ResolveExcess_DoesntAddExcessWhenAllCountersAreEqual()
        {
            var counter1 = new Counter { Count = 1, Percent = 33.33 };
            var counter2 = new Counter { Count = 1, Percent = 33.33 };
            var counter3 = new Counter { Count = 1, Percent = 33.33 };
            var counters = new List<Counter> { counter1, counter2, counter3 };

            new CounterManager().ResolveExcess(counters);

            Equal(33.33, counter1.Percent);
            Equal(33.33, counter2.Percent);
            Equal(33.33, counter3.Percent);
        }
         
        [Fact]
        public void ResolveExcess_AddsExcessToHighestCounter()
        {
            var counter1 = new Counter { Count = 2, Percent = 66.66 };
            var counter2 = new Counter { Count = 1, Percent = 33.33 };
            var counters = new List<Counter> { counter1, counter2 };

            new CounterManager().ResolveExcess(counters);

            Equal(66.67, counter1.Percent);
            Equal(33.33, counter2.Percent);
        }

        [Theory]
        [InlineData(66.66,66.67, 33.33)]
        [InlineData(66.66,66.68, 33.32)]
        public void ResolveExcess_DoesntAddExcessIfTotalPercentIs100(double initial, double expected, double lowest)
        {
            var counter1 = new Counter { Count = 2, Percent = initial };
            var counter2 = new Counter { Count = 1, Percent = lowest };
            var counters = new List<Counter> { counter1, counter2 };

            new CounterManager().ResolveExcess(counters);

            Equal(expected, counter1.Percent);
            Equal(lowest, counter2.Percent);
        }

        [Theory]
        [InlineData(11.11, 11.12, 44.44)]
        [InlineData(11.10, 11.12, 44.44)]
        public void ResolveExcess_AddsExcessToLowestCounterWhenMoreThanOneHighestCounters(double initial, double expected, double highest)
        {
            var counter1 = new Counter { Count = 2, Percent = highest };
            var counter2 = new Counter { Count = 2, Percent = highest };
            var counter3 = new Counter { Count = 1, Percent = initial };
            var counters = new List<Counter> { counter1, counter2, counter3 };

            new CounterManager().ResolveExcess(counters);

            Equal(highest, counter1.Percent);
            Equal(highest, counter2.Percent);
            Equal(expected, counter3.Percent);
        }
        
        
    }

    public class Counter
    {
        public string Name { get; set; }
        public int Count { get; set; }
        public double Percent { get; set; }

       
    }

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
