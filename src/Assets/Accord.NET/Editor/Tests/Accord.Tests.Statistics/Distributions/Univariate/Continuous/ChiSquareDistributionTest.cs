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
    using Accord.Statistics.Distributions.Univariate;
    using NUnit.Framework;
    //using Accord.Controls;
    using Accord.Statistics.Testing;

    [TestFixture]
    public class ChiSquareDistributionTest
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
        public void ConstructorTest()
        {
            var chisq = new ChiSquareDistribution(degreesOfFreedom: 7);

            double mean = chisq.Mean;     // 7
            double median = chisq.Median; // 6.345811195595612
            double var = chisq.Variance;  // 14

            double cdf = chisq.DistributionFunction(x: 6.27); // 0.49139966433823956
            double pdf = chisq.ProbabilityDensityFunction(x: 6.27); // 0.11388708001184455
            double lpdf = chisq.LogProbabilityDensityFunction(x: 6.27); // -2.1725478476948092

            double ccdf = chisq.ComplementaryDistributionFunction(x: 6.27); // 0.50860033566176044
            double icdf = chisq.InverseDistributionFunction(p: cdf); // 6.2700000000852318

            double hf = chisq.HazardFunction(x: 6.27); // 0.22392254197721179
            double chf = chisq.CumulativeHazardFunction(x: 6.27); // 0.67609276602233315

            string str = chisq.ToString(); // "χ²(x; df = 7)

			Assert.AreEqual(7, mean, 1e-10);
            Assert.AreEqual(6.345811195595612, median, 1e-6);
			Assert.AreEqual(14, var, 1e-10);
			Assert.AreEqual(0.67609276602233315, chf, 1e-10);
			Assert.AreEqual(0.49139966433823956, cdf, 1e-10);
			Assert.AreEqual(0.11388708001184455, pdf, 1e-10);
			Assert.AreEqual(-2.1725478476948092, lpdf, 1e-10);
			Assert.AreEqual(0.22392254197721179, hf, 1e-10);
			Assert.AreEqual(0.50860033566176044, ccdf, 1e-10);
            Assert.AreEqual(6.2700000000852318, icdf, 1e-6);
            Assert.AreEqual("χ²(x; df = 7)", str);
        }

        [Test]
        public void MedianTest()
        {
            var target = new ChiSquareDistribution(degreesOfFreedom: 4);

            Assert.AreEqual(target.Median, target.InverseDistributionFunction(0.5));
        }

        [Test]
        public void ProbabilityDensityFunctionTest()
        {
            int degreesOfFreedom;
            double actual, expected, x;
            ChiSquareDistribution target;

            degreesOfFreedom = 1;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 1;
            actual = target.ProbabilityDensityFunction(x);
            expected = 0.2420;
            Assert.AreEqual(expected, actual, 1e-4);

            degreesOfFreedom = 2;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.ProbabilityDensityFunction(x);
            expected = 0.1839;
            Assert.AreEqual(expected, actual, 1e-4);

            degreesOfFreedom = 10;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.ProbabilityDensityFunction(x);
            expected = 0.0077;
            Assert.AreEqual(expected, actual, 1e-4);
        }

        [Test]
        public void LogProbabilityDensityFunctionTest()
        {
            int degreesOfFreedom;
            double actual, expected, x;
            ChiSquareDistribution target;

            degreesOfFreedom = 1;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 1;
            actual = target.LogProbabilityDensityFunction(x);
            expected = System.Math.Log(target.ProbabilityDensityFunction(x));
            Assert.AreEqual(expected, actual, 1e-10);

            degreesOfFreedom = 2;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.LogProbabilityDensityFunction(x);
            expected = System.Math.Log(target.ProbabilityDensityFunction(x));
            Assert.AreEqual(expected, actual, 1e-10);

            degreesOfFreedom = 10;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 2;
            actual = target.LogProbabilityDensityFunction(x);
            expected = System.Math.Log(target.ProbabilityDensityFunction(x));
            Assert.AreEqual(expected, actual, 1e-10);
        }

        [Test]
        public void DistributionFunctionTest()
        {
            int degreesOfFreedom;
            double actual, expected, x;
            ChiSquareDistribution target;

            degreesOfFreedom = 1;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 5;
            actual = target.DistributionFunction(x);
            expected = 0.9747;
            Assert.AreEqual(expected, actual, 1e-4);


            degreesOfFreedom = 5;
            target = new ChiSquareDistribution(degreesOfFreedom);
            x = 5;
            actual = target.DistributionFunction(x);
            expected = 0.5841;
            Assert.AreEqual(expected, actual, 1e-4);
        }

     


    }
}
