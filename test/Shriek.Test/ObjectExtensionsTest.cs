using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Shriek.Test
{
	[TestClass]
	public class ObjectExtensionsTest
	{
		[TestMethod]
		public void GetEnumKeyPairValuesTest()
		{
			var kp = typeof(testEnum).GetEnumKeyValueTuple();
			var dic = typeof(testEnum).GetEnumKeyValue();

			Assert.IsNotNull(kp);
			Assert.IsNotNull(dic);
		}

		public enum testEnum
		{
			test1 = 10,
			test2 = 20,
			test3 = 30
		}
	}
}