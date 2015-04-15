using System;
using FakeItEasy;
using NUnit.Framework;

namespace HrKit
{
	public class Reviver<T> where T : class
	{
		private T obj;
		private readonly Func<T> revive;
		private readonly Func<T, bool> isDead;
		private int reviveLimit;

		public Reviver(Func<T> revive, Func<T, bool> isDead, int reviveLimit)
		{
			this.revive = revive;
			this.isDead = isDead;
			this.reviveLimit = reviveLimit;
		}

		public T TryGetAlive()
		{
			if (obj != null && !isDead(obj)) return obj;
			if (reviveLimit < 0) return null;
			reviveLimit--;
			return obj = revive();
		}
	}

	[TestFixture]
	public class Reviver_should
	{
		[Test]
		public void return_alive_infinitely()
		{
			var revive = A.Fake<Func<string>>();
			A.CallTo(() => revive()).ReturnsNextFromSequence("a", "b", "c");
		
			var reviver = new Reviver<object>(revive, s => false, 0);
			for(int i=0; i<100; i++)
				Assert.AreEqual("a", reviver.TryGetAlive());

			A.CallTo(() => revive()).MustHaveHappened(Repeated.Exactly.Once);
		}
	
		[Test]
		public void not_revive_after_reviveLimit()
		{
			var revive = A.Fake<Func<string>>();
			A.CallTo(() => revive()).ReturnsNextFromSequence("a", "b", "c");

			var reviver = new Reviver<object>(revive, s => true, 1);

			Assert.AreEqual("a", reviver.TryGetAlive());
			Assert.AreEqual("b", reviver.TryGetAlive());
			Assert.IsNull(reviver.TryGetAlive());

			A.CallTo(() => revive()).MustHaveHappened(Repeated.Exactly.Twice);
		}
	}
}