using Extensions;
using NUnit.Framework;
using UnityEngine;

namespace Editor.Tests
{
    public class ExtensionMethodsTest
    {
        [Test]
        public void ToVector2Test()
        {
            var v3 = new Vector3(1,0,1);
            var v2 = new Vector2(1,1);
            Assert.AreEqual(v3.ToVector2(), v2);
        }
        
        [Test]
        public void ToVector3Test()
        {
            var v3 = new Vector3(1,0,1);
            var v2 = new Vector2(1,1);
            Assert.AreEqual(v2.ToVector3(), v3);
        }

        [Test]
        public void MapTest()
        {
            var result = 4f.Map(0f, 5f, 0f, 1f);
            Assert.AreEqual(result, 0.8f);
            result = 4f.Map(5f, 0f, 0f, 1f);
            Assert.AreEqual(result, 0.2f);
            result = 4f.Map(0f, 5f, 1f, 0f);
            Assert.AreEqual(result, 0.2f);
            result = 4f.Map(5f, 0f, 1f, 0f);
            Assert.AreEqual(result, 0.8f);
        }
    }
}