using System.Collections;
using NUnit.Framework;
using UnityEngine.TestTools;

namespace Carotaa.Code.Test
{
    public class ShareEventTest
    {
        // A Test behaves as an ordinary method
        [Test]
        public void ShareEventTest2SimplePasses()
        {
            var e = ShareEvent.BuildEvent("Test");
            e.Raise();

            e.Subscribe(() => { });

            e.Raise();
        }
    }
}