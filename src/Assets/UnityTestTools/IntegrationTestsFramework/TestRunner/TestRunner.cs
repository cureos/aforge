//#define IMITATE_BATCH_MODE //uncomment if you want to imitate batch mode behaviour in non-batch mode mode run
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityTest.IntegrationTestRunner;

namespace UnityTest
{
	[Serializable]
	public class TestRunner : MonoBehaviour
	{
		static public string integrationTestsConfigFileName = "integrationtestsconfig.txt";
		static public string batchRunFileMarker = "batchrun.txt";
		static private TestResultRenderer resultRenderer = new TestResultRenderer ();
		private ITestComponent currentTest;
		public List<TestResult> testToRun = new List<TestResult>();
		
		public bool isInitializedByRunner
		{
			get
			{
#if UNITY_EDITOR && !IMITATE_BATCH_MODE
				if (!UnityEditorInternal.InternalEditorUtility.inBatchMode) return true;
#endif
				return false;
			}
		}

		private double startTime;
		private bool readyToRun;

		private AssertionComponent[] assertionsToCheck = null;
		private string testMessages;
		private string stacktrace;
		private TestState testState = TestState.Running;

		public TestRunnerCallbackList TestRunnerCallback = new TestRunnerCallbackList();
		private IntegrationTestsProvider testsProvider;

		private const string startedMessage = "IntegrationTest started";
		private const string finishedMessage = "IntegrationTest finished";
		private const string timeoutMessage = "IntegrationTest timeout";
		private const string failedMessage = "IntegrationTest failed";
		private const string failedExceptionMessage = "IntegrationTest failed with exception";
		private const string ignoredMessage = "IntegrationTest ignored";
		private const string interruptedMessage = "IntegrationTest Run interrupted";

		private const string defaultResulFilePostfix = "TestResults.xml";

		public void Awake ()
		{
#if UNITY_EDITOR && !IMITATE_BATCH_MODE
			if (!UnityEditorInternal.InternalEditorUtility.inBatchMode) return;
#endif
			DisableAllTests ();
		}

		

		public void Start()
		{
#if UNITY_EDITOR && !IMITATE_BATCH_MODE
			if (!UnityEditorInternal.InternalEditorUtility.inBatchMode) return;
#endif
			var tests = TestRunner.FindAllTestsOnScene ();
			InitRunner(tests);
		}

		public void InitRunner(List<ITestComponent> tests)
		{
			Application.RegisterLogCallback(LogHandler);
			var testComponents = ParseListForGroups (tests).ToList ();
			testToRun = testComponents.Select (component => new TestResult (component.gameObject)).ToList ();
			testsProvider = new IntegrationTestsProvider (testToRun.Select (result => result.TestComponent as ITestComponent));
			readyToRun = true;
		}

		private static IEnumerable<ITestComponent> ParseListForGroups ( IEnumerable<ITestComponent> tests )
		{
			var results = new HashSet<ITestComponent> ();
			foreach (var testResult in tests)
			{
				if (testResult.IsTestGroup ())
				{
					var childrenTestResult = testResult.gameObject.GetComponentsInChildren (typeof (TestComponent), true)
						.Where (t=>t!=testResult)
						.Cast<ITestComponent> ()
						.ToArray ();
					foreach (var result in childrenTestResult)
					{
						if(!result.IsTestGroup())
							results.Add (result);
					}
					continue;
				}
				results.Add (testResult);
			}
			return results;
		}

		public void Update ()
		{
			if (readyToRun  && Time.frameCount > 1)
			{
				readyToRun = false;
				StartCoroutine ("StateMachine");
			}
		}

		public void OnDestroy()
		{
			if (currentTest != null)
			{
				var testResult = testToRun.Single (result => result.isRunning);
				testResult.messages += "Test run interrupted (crash?)";
				LogMessage(interruptedMessage);
				FinishTest(TestResult.ResultType.Failed);
			}
			if (currentTest != null || (testsProvider != null && testsProvider.AnyTestsLeft ()))
			{
				var remainingTests = testsProvider.GetRemainingTests ();
				TestRunnerCallback.TestRunInterrupted(testToRun.Where (t=>remainingTests.Contains (t.TestComponent)).ToList ());
			}
			Application.RegisterLogCallback(null);
		}

		private void LogHandler (string condition, string stacktrace, LogType type)
		{
			testMessages += condition + "\n";
			if (type == LogType.Exception)
			{
				var exceptionType = condition.Substring (0, condition.IndexOf(':'));
				if (currentTest.IsExceptionExpected (exceptionType))
				{
					testMessages += exceptionType + " was expected\n";
					if (currentTest.ShouldSucceedOnException())
					{
						testState = TestState.Success;
					}
				}
				else
				{
					testState = TestState.Exception;
					this.stacktrace = stacktrace;
				}
			}
			else if (type == LogType.Log)
			{
				if (condition.StartsWith (IntegrationTest.passMessage))
				{
					testState = TestState.Success;
				}
				if (condition.StartsWith(IntegrationTest.failMessage))
				{
					testState = TestState.Failure;
				}
			}
		}

		public IEnumerator StateMachine ()
		{
			TestRunnerCallback.RunStarted(Application.platform.ToString(), testToRun);
			while (true)
			{
				if (!testsProvider.AnyTestsLeft() && currentTest == null)
				{
					FinishTestRun ();
					yield break;
				}
				if (currentTest == null)
				{
					StartNewTest ();
				}
				if (currentTest != null)
				{
					if (testState == TestState.Running)
					{
						if (assertionsToCheck != null && assertionsToCheck.All (a => a.checksPerformed > 0))
						{
							IntegrationTest.Pass (currentTest.gameObject);
							testState = TestState.Success;
						}
						if (currentTest != null && Time.time > startTime + currentTest.GetTimeout())
						{
							testState = TestState.Timeout;
						}
					}

					switch (testState)
					{
						case TestState.Success:
							LogMessage (finishedMessage);
							FinishTest (TestResult.ResultType.Success);
							break;
						case TestState.Failure:
							LogMessage (failedMessage);
							FinishTest (TestResult.ResultType.Failed);
							break;
						case TestState.Exception:
							LogMessage (failedExceptionMessage);
							FinishTest (TestResult.ResultType.FailedException);
							break;
						case TestState.Timeout:
							LogMessage(timeoutMessage);
							FinishTest(TestResult.ResultType.Timeout);
							break;
						case TestState.Ignored:
							LogMessage (ignoredMessage);
							FinishTest(TestResult.ResultType.Ignored);
							break;
					}
				}
				yield return null;
			}
		}

		private void LogMessage(string message)
		{
			if (currentTest != null)
				Debug.Log (message + " (" + currentTest.Name + ")", currentTest.gameObject);
			else
				Debug.Log (message);
		}

		private void FinishTestRun ()
		{
			if (IsBatchRun ())
				SaveResults ();
			PrintResultToLog ();
			TestRunnerCallback.RunFinished (testToRun);
			LoadNextLevelOrQuit ();
		}

		private void PrintResultToLog ()
		{
			var resultString = "";
			resultString += "Passed: " + testToRun.Count (t => t.IsSuccess);
			if (testToRun.Any (result => result.IsFailure))
			{
				resultString += " Failed: " + testToRun.Count (t => t.IsFailure);
				Debug.Log ("Failed tests: " + string.Join (", ", testToRun.Where (t => t.IsFailure).Select (result => result.Name).ToArray ()));
			}
			if (testToRun.Any (result => result.IsIgnored))
			{
				resultString += " Ignored: " + testToRun.Count (t => t.IsIgnored);
				Debug.Log ("Ignored tests: " + string.Join (", ",
															testToRun.Where (t => t.IsIgnored).Select (result => result.Name).ToArray ()));
			}
			Debug.Log (resultString);
		}

		private void LoadNextLevelOrQuit ()
		{
			if (isInitializedByRunner) return;
			
			if (Application.loadedLevel < Application.levelCount - 1)
				Application.LoadLevel (Application.loadedLevel + 1);
			else
			{
#if UNITY_EDITOR && !IMITATE_BATCH_MODE
				UnityEditor.EditorApplication.Exit (0);
#else
				resultRenderer.ShowResults ();
				if(IsBatchRun())
					Application.Quit ();
#endif
			}
		}

		public void OnGUI ()
		{
			resultRenderer.Draw ();
		}

		private void SaveResults ()
		{
			if (!IsFileSavingSupported ()) return;
			var resultDestiantion = GetResultDestiantion ();
			var resultFileName = Application.loadedLevelName;
			if (resultFileName != "")
				resultFileName += "-";
			resultFileName += defaultResulFilePostfix;

			var resultWriter = new XmlResultWriter (Application.loadedLevelName, testToRun.ToArray ());

#if !UNITY_METRO 
			Uri uri;
			if ( Uri.TryCreate (resultDestiantion, UriKind.Absolute, out uri) && uri.Scheme == Uri.UriSchemeFile)
			{
				resultWriter.WriteToFile (resultDestiantion, resultFileName);
			}
			else
			{
				Debug.LogError ("Provided path is invalid");
			}
#endif
		}

		private bool IsFileSavingSupported ()
		{
#if UNITY_EDITOR || UNITY_STANDALONE
			return true;
#else
			return false;
#endif
		}

		private string GetResultDestiantion ()
		{
			var nameWithoutExtension = integrationTestsConfigFileName.Substring (0, integrationTestsConfigFileName.LastIndexOf ('.'));
			var resultpathFile = Resources.Load (nameWithoutExtension) as TextAsset;
			var resultDestiantion = Application.dataPath;
			if (resultpathFile != null)
				resultDestiantion = resultpathFile.text;
#if UNITY_EDITOR
			var resultsFileDirectory = "-resultsFileDirectory=";
			if (UnityEditorInternal.InternalEditorUtility.inBatchMode && Environment.GetCommandLineArgs ().Any (s => s.StartsWith (resultsFileDirectory)))
				resultDestiantion = Environment.GetCommandLineArgs ().First (s => s.StartsWith (resultsFileDirectory)).Substring (resultsFileDirectory.Length);
#endif
			return resultDestiantion;
		}

		private bool IsBatchRun ()
		{
#if UNITY_EDITOR && !IMITATE_BATCH_MODE
			if (UnityEditorInternal.InternalEditorUtility.inBatchMode) return true;
#endif
			var nameWithoutExtension = batchRunFileMarker.Substring (0, batchRunFileMarker.LastIndexOf ('.'));
			var resultpathFile = Resources.Load (nameWithoutExtension) as TextAsset;
			return resultpathFile != null;
		}

		private void StartNewTest ()
		{
			this.testMessages = "";
			this.stacktrace = "";
			testState = TestState.Running;
			assertionsToCheck = null;

			startTime = Time.time;
			currentTest = testsProvider.GetNextTest ();
			var testResult = testToRun.Single (result => result.TestComponent == currentTest);
			testResult.isRunning = true;
			
			if (currentTest.ShouldSucceedOnAssertions ())
			{
				var assertionList = currentTest.gameObject.GetComponentsInChildren<AssertionComponent> ().Where (a => a.enabled);
				if(assertionList.Any())
					assertionsToCheck = assertionList.ToArray();
			}

			if (currentTest.IsExludedOnThisPlatform ())
			{
				testState = TestState.Ignored;
				Debug.Log(currentTest.gameObject.name + " is excluded on this platform");
			}

			//do not run ignored tests only when it's batch mode
			//test runner in the editor will not pass ignored tests to run, unless is expected to
			if (!isInitializedByRunner && currentTest.IsIgnored())
				testState = TestState.Ignored;
			LogMessage(startedMessage);
			TestRunnerCallback.TestStarted (testResult);
		}

		private void FinishTest(TestResult.ResultType result)
		{
			testsProvider.FinishTest (currentTest);
			var testResult = testToRun.Single (t => t.isRunning);
			testResult.resultType = result;
			testResult.isRunning = false;
			testResult.duration = Time.time - startTime;
			testResult.messages = testMessages;
			testResult.stacktrace = stacktrace;
			TestRunnerCallback.TestFinished (testResult);
			currentTest = null;
			if (!testResult.IsSuccess 
				&& testResult.Executed
				&& !testResult.IsIgnored) resultRenderer.AddResults (Application.loadedLevelName, testResult);
		}

		#region Test Runner Helpers
		
		public static TestRunner GetTestRunner ()
		{
			TestRunner testRunnerComponent = null;
			var testRunnerComponents = Resources.FindObjectsOfTypeAll(typeof(TestRunner));

			if (testRunnerComponents.Count () > 1)
				foreach (var t in testRunnerComponents) DestroyImmediate((t as TestRunner).gameObject);
			else if (!testRunnerComponents.Any())
				testRunnerComponent = Create().GetComponent<TestRunner>();
			else
				testRunnerComponent = testRunnerComponents.Single() as TestRunner;

			return testRunnerComponent;
		}

		private static GameObject Create()
		{
			var runner = new GameObject ("TestRunner");
			var component = runner.AddComponent<TestRunner> ();
			component.hideFlags = HideFlags.NotEditable;
			Debug.Log ("Created Test Runner");
			return runner;
		}

		public static List<ITestComponent> FindAllTestsOnScene ()
		{
			return Resources.FindObjectsOfTypeAll (typeof (TestComponent)).Cast<ITestComponent> ().ToList ();
		}

		public static void DisableAllTests ()
		{
			foreach (var t in TestRunner.FindAllTestsOnScene ())
			{
				t.EnableTest (false);
			}
		}

		#endregion

		enum TestState
		{
			Running,
			Success,
			Failure,
			Exception,
			Timeout,
			Ignored
		}
	}
}
