using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Shriek.Utils;

namespace Shriek.Test
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void ObjectToMapTest()
        {
            var obj = new obj { name = "ysj", age = 1, gender = genter.男, Obj = new obj { name = "objname", age = 2, gender = genter.女 } };

            var dict = obj.ToMap();

            var obj2 = dict.ToObject<obj>();

            var obj3 = dict.ToObject(typeof(obj));

            Assert.IsNotNull(obj2);
            Assert.IsNotNull(obj3);
            Assert.IsNotNull(obj2.Obj);

            Assert.AreEqual(obj2.name, "ysj");
            Assert.AreEqual(obj.Obj.name, "objname");
        }
    }

    public class obj
    {
        public string name { get; set; }

        public int age { get; set; }

        public genter gender { get; set; }

        public obj Obj { get; set; }
    }

    public enum genter
    {
        男, 女
    }
}