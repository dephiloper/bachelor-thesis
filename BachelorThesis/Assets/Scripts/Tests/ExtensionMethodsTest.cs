using UnityEngine;
using UnityEngine.Assertions;

namespace Tests
{
    public static class ExtensionMethodsTest
    {
        [RuntimeInitializeOnLoadMethod]
        private static void ToVector2Test()
        {
            var v3 = new Vector3(1,0,1);
            var v2 = new Vector2(1,1);
            Assert.AreEqual(v2, v3.ToVector2());
        }

        [RuntimeInitializeOnLoadMethod]
        private static void ToVector3Test()
        {
            var v3 = new Vector3(1,0,1);
            var v2 = new Vector2(1,1);
            Assert.AreEqual(v2.ToVector3(), v3);
        }

        [RuntimeInitializeOnLoadMethod]
        private static void MapTest()
        {
            var result = 4f.Map(0f, 5f, 0f, 1f);
            Assert.AreApproximatelyEqual(result, 0.8f);
            result = 4f.Map(5f, 0f, 0f, 1f);
            Assert.AreApproximatelyEqual(result, 0.2f);
            result = 4f.Map(0f, 5f, 1f, 0f);
            Assert.AreApproximatelyEqual(result, 0.2f);
            result = 4f.Map(5f, 0f, 1f, 0f);
            Assert.AreApproximatelyEqual(result, 0.8f);
        }
    }
}