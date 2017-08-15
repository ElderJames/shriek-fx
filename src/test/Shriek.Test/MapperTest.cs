using Shriek.Utils;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shriek.Test
{
    [TestClass]
    public class MapperTest
    {
        [TestMethod]
        public void ObjectToMapTest()
        {
            var obj = new obj { name = "ysj", age = 1, gender = genter.男 };

            var dict = obj.ToMap();

            var obj2 = dict.ToObject<obj>();

            var obj3 = dict.ToObject(typeof(obj));

            Assert.IsNotNull(obj2);
            Assert.IsNotNull(obj3);
            //Assert.Equals(obj, obj2);
        }
    }

    public class obj
    {
        public string name { get; set; }

        public int age { get; set; }

        public genter gender { get; set; }
    }

    public enum genter
    {
        男, 女
    }
}