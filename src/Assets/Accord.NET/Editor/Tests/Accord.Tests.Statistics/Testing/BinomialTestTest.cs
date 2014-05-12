// Accord Unit Tests
// The Accord.NET Framework
// http://accord-framework.net
//
// Copyright © César Souza, 2009-2014
// cesarsouza at gmail.com
//
//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU Lesser General Public
//    License as published by the Free Software Foundation; either
//    version 2.1 of the License, or (at your option) any later version.
//
//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
//

namespace Accord.Tests.Statistics
{
    using Accord.Statistics.Testing;
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class BinomialTestTest
    {


        private TestContext testContextInstance;


        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }




        [Test]
        public void BinomialTestConstructorTest1()
        {

            bool[] trials = { true, true, true, true, true, true, true, true, true, false };

            BinomialTest target = new BinomialTest(trials,
                hypothesizedProbability: 0.5, alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.010742, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [Test]
        public void BinomialTestConstructorTest2()
        {
            int successes = 5;
            int trials = 6;
            double probability = 1 / 4.0;

            BinomialTest target = new BinomialTest(successes, trials,
                hypothesizedProbability: probability,
                alternate: OneSampleHypothesis.ValueIsGreaterThanHypothesis);

            Assert.AreEqual(OneSampleHypothesis.ValueIsGreaterThanHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.OneUpper, target.Tail);

            Assert.AreEqual(0.004638, target.PValue, 1e-5);
            Assert.IsTrue(target.Significant);
        }

        [Test]
        public void BinomialTestConstructorTest3()
        {
            // GNU R (0.096248626708984375)
            // Wolfram Alpha reports 0.06357
			
			/*
			// Tested against Wolfram Alpha
			BinomialTest target = new BinomialTest(5, 18);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

            Assert.AreEqual(0.063564300537109319, target.PValue, 1e-4);
            Assert.IsFalse(target.Significant);
            */
			
			// Tested against GNU R
			BinomialTest target = new BinomialTest(5, 18);

			Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
			Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

			Assert.AreEqual(0.096248626708984375, target.PValue, 1e-4);
			Assert.IsFalse(target.Significant);
		}
		
		[Test]
		public void BinomialTestConstructorTest4()
		{

				bool[] trials = { false, false, false, false, false, false, false, false, false, true };

				BinomialTest target = new BinomialTest(trials,
						hypothesizedProbability: 0.5, alternate: OneSampleHypothesis.ValueIsSmallerThanHypothesis);

				Assert.AreEqual(OneSampleHypothesis.ValueIsSmallerThanHypothesis, target.Hypothesis);
				Assert.AreEqual(DistributionTail.OneLower, target.Tail);

				Assert.AreEqual(0.010742, target.PValue, 1e-5);
				Assert.IsTrue(target.Significant);
		}

        [Test]
        public void BinomialTestConstructorTest8()
        {
            // Example from Jeffrey S. Simonoff, Analyzing Categorical Data, pg 64
            // Preview available: http://books.google.com.br/books?id=G8w_rifweAoC

            BinomialTest target = new BinomialTest(8, 10, hypothesizedProbability: 0.4);

            Assert.AreEqual(OneSampleHypothesis.ValueIsDifferentFromHypothesis, target.Hypothesis);
            Assert.AreEqual(DistributionTail.TwoTail, target.Tail);

            Assert.AreEqual(0.018236313600000005, target.PValue, 1e-4);
            Assert.IsTrue(target.Significant);


            target = new BinomialTest(7, 10, hypothesizedProbability: 0.4);
            Assert.AreEqual(0.1010144256, target.PValue, 1e-4);
            Assert.IsFalse(target.Significant);


            target = new BinomialTest(9, 10, hypothesizedProbability: 0.4);
            Assert.AreEqual(0.0015728640000000009, target.PValue, 1e-4);
            Assert.IsTrue(target.Significant);
        }


		[TestCase(0, 0.000000000000)]
		[TestCase(1, 0.000000000000)]
		[TestCase(2, 0.0000001539975)]
		[TestCase(3, 0.00592949743)]
		[TestCase(4, 0.514242625443)]
		[TestCase(5, 0.25905439494)]
		[TestCase(6, 0.0053806543164)]
		[TestCase(7, 0.00001078919)]
		[TestCase(8, 0.000000003115)]
		[TestCase(9, 0.00000000000)]
		[TestCase(10, 0.00000000000)]
		public void BinomialTestConstructorTest6(int index, double expectedValue)
		{
			double p = index / 100.0 * 5;
			BinomialTest target = new BinomialTest(51, 235, p);

			Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
			Assert.AreEqual(expectedValue, target.PValue, 1e-5);
		}

		[TestCase(0, 0.00000000000)]
		[TestCase(1, 0.02819385651)]
		[TestCase(2, 0.382725376073)]
		[TestCase(3, 1.00000000000)]
		[TestCase(4, 0.34347252004)]
		//[TestCase(5, 0.063564300537)] // Wolfram Alpha reports 0.
		// http://www.wolframalpha.com/input/?i=test+for+binomial+parameter+p0%3D0.5%2C+samples%3D18%2C+successes%3D5
		[TestCase(5, 0.096248626708)] // GNU R reports 0.096248626708
		[TestCase(6, 0.00707077678)]
		[TestCase(7, 0.00026908252)]
		[TestCase(8, 0.000002519659)]
		[TestCase(9, 0.00000000052)]
		[TestCase(10, 0.00000000000)]
		public void BinomialTestConstructorTest7(int index, double expectedValue)
        {
            double p = index / 10.0;
	        BinomialTest target = new BinomialTest(5, 18, p);

	        Assert.AreEqual(DistributionTail.TwoTail, target.Tail);
			Assert.AreEqual(expectedValue, target.PValue, 5e-4);
        }

    }
}
