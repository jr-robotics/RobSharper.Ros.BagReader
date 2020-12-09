// ReSharper disable once CheckNamespace

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace RobSharper.Ros.BagReader.Tests
{
    public class Bagfiles
    {
        public static IEnumerable<string> AllTestBags = new List<string>()
        {
            //"bags/2019-01-19-16-25-02.bag",
            "bags/2019-01-19-16-36-59.bag",
            //"bags/2019-01-22-14-19-20.bag"
        };

        public class All : IEnumerable<object[]>
        {
            public IEnumerator<object[]> GetEnumerator()
            {
                return Bagfiles.AllTestBags
                    .Select(b => new object[1] {b})
                    .ToList()
                    .GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}