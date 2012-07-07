namespace SampleClassLibrary
{
	public class UsefulType<A, E, D>
	{
		private A aArray;
		private E aEnum;
		private D aDelegate;

		public UsefulType(A aArray, E aEnum, D aDelegate)
		{
			this.aArray = aArray;
			this.aEnum = aEnum;
			this.aDelegate = aDelegate;
		}
	}

	public class NoConstraint<T>
	{
	}
}
