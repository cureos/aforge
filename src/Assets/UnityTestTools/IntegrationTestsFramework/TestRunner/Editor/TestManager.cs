using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
	[Serializable]
	public class TestManager
	{
		[SerializeField]
		private static bool reloadTestList = true;
		private static DateTime nextIvalidateTime = DateTime.Now;
	
		[SerializeField]
		private List<TestResult> testList = new List<TestResult>();

		public IList<TestResult> GetAllTestsResults ()
		{
			TryToReload ();
			return testList.ToList();
		}

		public IList<TestResult> GetChildrenTestsResults ( TestComponent tc )
		{
			TryToReload ();
			if(tc==null)
				return testList.Where (t => t.GameObject != null && t.GameObject.transform.parent == null).ToList ();
			else
				return testList.Where (t => t.GameObject != null && t.GameObject.transform.parent == tc.gameObject.transform).ToList ();
		}

		private void TryToReload ()
		{
			if (reloadTestList && nextIvalidateTime <= DateTime.Now)
			{
				var foundTestList = TestRunner.FindAllTestsOnScene ();
				var newTestList = new List<TestResult> ();
				foreach (var test in foundTestList)
				{
					var result = testList.Find (t => t.GameObject == test.gameObject);
					if (result != null)
					{
						newTestList.Add (result);
					}
					else
						newTestList.Add (new TestResult (test.gameObject));
				}
				testList = newTestList;

				SortTestList ();
				reloadTestList = false;
				nextIvalidateTime = DateTime.Now.AddSeconds (1);
			}
		}

		public TestResult AddTest ()
		{
			var go = new GameObject ();
			go.name = "New Test";
			go.AddComponent<TestComponent>();
			go.transform.hideFlags |= HideFlags.HideInInspector;

			var testResult = new TestResult (go);
			testList.Add(testResult);
			SortTestList ();	

			return testResult;
		}

		private void SortTestList ()
		{
			testList.Sort();
		}

		public void ClearTestList ()
		{
			testList.Clear ();
			InvalidateTestList ();
		}

		public static void InvalidateTestList ()
		{
			nextIvalidateTime = DateTime.Now;
			reloadTestList = true;
		}

		public TestResult GetResultFor ( GameObject gameObject )
		{
			var results = GetAllTestsResults ().Where (result => result.GameObject == gameObject);
			if (results.Count () == 1)
				return results.Single ();
			else
			{
#if !(UNITY_4_0 || UNITY_4_0_1 || UNITY_4_1 || UNITY_4_2)
				Debug.LogWarning ( "Result not found for " + gameObject);
#endif
				InvalidateTestList ();
				return null;
			}
		}

		public void UpdateResults (List<TestResult> tests)
		{
			foreach (var testResult in tests)
			{
				var idx = testList.FindIndex (result => result.id == testResult.id);
				testList[idx] = testResult;
			}
		}

		#region Static methods

		public static GameObject FindTopGameObject (GameObject go)
		{
			while (go.transform.parent != null)
				go = go.transform.parent.gameObject;
			return go;
		}

		public bool AnyTestsOnScene ()
		{
			return GetAllTestsResults ().Any ();
		}

		public void SelectInHierarchy (TestResult test)
		{
			foreach (var t in GetAllTestsResults ())
			{
				if (t.GameObject == null)
				{
					InvalidateTestList ();
					continue;
				}
				t.TestComponent.EnableTest (test == t);
				if (t.GameObject.GetComponentsInChildren<TestComponent> (true).Any (c => c == test.TestComponent))
				{
					t.TestComponent.EnableTest (true);
				}
			}
		}

		#endregion

		public IEnumerable<TestResult> GetTestsToSelect (List<TestResult> selectedTests, TestResult testToSelect)
		{
			TestResult start = null;
			TestResult end = null;

			for (int i = testList.Count-1; i >=0 ; i--)
			{
				var testResult = testList[i];
				if (start==null)
				{
					if (testResult == testToSelect)
						start = testToSelect;
					else if (selectedTests.Contains (testResult))
						start = testResult;
				}else if(testResult == testToSelect)
				{
					end = testToSelect;
					break;
				}
				if(start!=null)
				{
					if (testResult == testToSelect)
						end = testToSelect;
					else if (selectedTests.Contains(testResult))
						end = testResult;

				}
			}
			var startIdx = testList.IndexOf (start);
			var endIdx = testList.IndexOf (end);
			return testList.GetRange(endIdx, startIdx-endIdx+1);
		}
	}
}
