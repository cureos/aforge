using System;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
	public interface ITestComponent : IComparable<ITestComponent>
	{
		void EnableTest (bool enable);
		bool IsTestGroup ();
		GameObject gameObject { get; }
		string Name { get; }
		ITestComponent GetTestGroup ();
		bool IsExceptionExpected (string exceptionType);
		bool ShouldSucceedOnException ();
		double GetTimeout ();
		bool IsIgnored ();
		bool ShouldSucceedOnAssertions ();
		bool IsExludedOnThisPlatform ();
	}

	public class TestComponent : MonoBehaviour, ITestComponent
	{
		public static ITestComponent NullTestComponent = new NullTestComponentImpl ();

		public float timeout = 5;
		public bool ignored = false;
		public bool succeedAfterAllAssertionsAreExecuted = false;
		public bool expectException = false;
		public string expectedExceptionList = "";
		public bool succeedWhenExceptionIsThrown = false;
		public IncludedPlatforms includedPlatforms = (IncludedPlatforms) ~0L;

		public bool IsExludedOnThisPlatform ()
		{
			try
			{
				var ipv = (IncludedPlatforms) Enum.Parse (typeof (IncludedPlatforms),
														Application.platform.ToString ());
				return (includedPlatforms & ipv) == 0;
			}
			catch
			{
				Debug.LogWarning ("Current platform is not supported");
				return true;
			}
		}

		static bool IsAssignableFrom(Type a, Type b)
		{
#if !UNITY_METRO
			return a.IsAssignableFrom(b);
#else
			return false;
#endif
		}

		public bool IsExceptionExpected (string exception)
		{
			if (!expectException) return false;
			exception = exception.Trim ();
			foreach (var expectedException in expectedExceptionList.Split (',').Select (e => e.Trim ()))
			{
				if (exception == expectedException) return true;
				var exceptionType = Type.GetType (exception) ?? GetTypeByName (exception);
				var expectedExceptionType = Type.GetType (expectedException) ?? GetTypeByName (expectedException);
				if (exceptionType != null && expectedExceptionType != null && IsAssignableFrom(expectedExceptionType,exceptionType))
					return true;
			}
			return false;
		}

		public bool ShouldSucceedOnException ()
		{
			return succeedWhenExceptionIsThrown;
		}

		public double GetTimeout ()
		{
			return timeout;
		}

		public bool IsIgnored ()
		{
			return ignored;
		}

		public bool ShouldSucceedOnAssertions ()
		{
			return succeedAfterAllAssertionsAreExecuted;
		}

		private static Type GetTypeByName (string className)
		{
#if !UNITY_METRO
			return AppDomain.CurrentDomain.GetAssemblies ().SelectMany (a => a.GetTypes ()).FirstOrDefault (type => type.Name == className);
#else
			return null;
#endif
		}

		public void OnValidate ()
		{
			if (timeout < 0.01f) timeout = 0.01f;
		}

		/// <summary>
		/// List of platform where the integration tests can be run. 
		/// Feel free to experiment with platforms that are commented out by uncommenting them.
		/// </summary>
		[Flags]
		public enum IncludedPlatforms
		{
			WindowsEditor = 1 << 0,
			OSXEditor = 1 << 1,
			WindowsPlayer = 1 << 2,
			OSXPlayer = 1 << 3,
			LinuxPlayer = 1 << 4,
			//MetroPlayerX86	= 1 << 5,
			//MetroPlayerX64	= 1 << 6,	
			//MetroPlayerARM	= 1 << 7,
			WindowsWebPlayer = 1 << 8,
			//OSXWebPlayer		= 1 << 9,
			Android = 1 << 10,
			//IPhonePlayer		= 1 << 11,
			//TizenPlayer		= 1 << 12,
			//WP8Player			= 1 << 13,
			//BB10Player		= 1 << 14,	
			//NaCl				= 1 << 15,
		}

		#region ITestComponent implementation

		public void EnableTest (bool enable)
		{
			gameObject.SetActive (enable);
		}

		public int CompareTo (ITestComponent obj)
		{
			if (obj == NullTestComponent)
				return 1;
			var result = gameObject.name.CompareTo (obj.gameObject.name);
			if (result == 0)
				result = gameObject.GetInstanceID ().CompareTo (obj.gameObject.GetInstanceID ());
			return result;
		}

		public bool IsTestGroup ()
		{
			for (int i = 0; i < gameObject.transform.childCount; i++)
			{
				var childTC = gameObject.transform.GetChild (i).GetComponent (typeof (TestComponent));
				if (childTC != null)
					return true;
			}
			return false;
		}

		public string Name { get { return gameObject == null ? "" : gameObject.name; } }

		public ITestComponent GetTestGroup ()
		{
			var parent = gameObject.transform.parent;
			if (parent == null)
				return NullTestComponent;
			return parent.GetComponent<TestComponent> ();
		}

		public override bool Equals ( object o )
		{
			if (o is TestComponent)
				return this == (o as TestComponent);
			return false;
		}

		public override int GetHashCode ()
		{
			return base.GetHashCode ();
		}

		public static bool operator == ( TestComponent a, TestComponent b )
		{
			if (ReferenceEquals (a, b))
			{
				return true;
			}
			if (((object)a == null) || ((object)b == null))
			{
				return false;
			}
			return a.gameObject == b.gameObject;
		}

		public static bool operator != ( TestComponent a, TestComponent b )
		{
			return !(a == b);
		}

		#endregion

		private sealed class NullTestComponentImpl : ITestComponent
		{
			public int CompareTo (ITestComponent other)
			{
				if (other == this) return 0;
				return -1;
			}

			public void EnableTest (bool enable)
			{
			}

			public ITestComponent GetParentTestComponent ()
			{
				throw new NotImplementedException ();
			}

			public bool IsTestGroup ()
			{
				throw new NotImplementedException ();
			}

			public GameObject gameObject { get; private set; }
			public string Name { get { return ""; } }

			public ITestComponent GetTestGroup ()
			{
				return null;
			}

			public bool IsExceptionExpected (string exceptionType)
			{
				throw new NotImplementedException ();
			}

			public bool ShouldSucceedOnException ()
			{
				throw new NotImplementedException ();
			}

			public double GetTimeout ()
			{
				throw new NotImplementedException ();
			}

			public bool IsIgnored ()
			{
				throw new NotImplementedException ();
			}

			public bool ShouldSucceedOnAssertions ()
			{
				throw new NotImplementedException ();
			}

			public bool IsExludedOnThisPlatform ()
			{
				throw new NotImplementedException ();
			}
		}
	}
}
