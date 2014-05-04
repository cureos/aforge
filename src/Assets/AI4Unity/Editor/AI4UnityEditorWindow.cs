using UnityEngine;
using UnityEditor;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Reflection;
using Microsoft.CSharp;

public class AI4UnityEditorWindow : EditorWindow {
	#region public class methods
	[MenuItem ("Window/AI4Unity/Build DLLs")]
	public static void ShowWindow () {
		AI4UnityEditorWindow w = EditorWindow.GetWindow<AI4UnityEditorWindow>("AI4Unity", true);

		if (PlayerPrefs.HasKey (AI4UnityEditorWindow.PlayerPrefs_BasePath)) {
			w.BuildPath = PlayerPrefs.GetString (AI4UnityEditorWindow.PlayerPrefs_BasePath);
		} else {
			w.BuildPath = Application.dataPath;
		}
	}
	#endregion

	#region protected class properties
	protected static readonly string BasePath = 
	Application.dataPath	+ Path.DirectorySeparatorChar + 
	"AForge.NET"			+ Path.DirectorySeparatorChar + 
	"Runtime"				+ Path.DirectorySeparatorChar;

	protected static readonly string AForgeCorePath = 
	AI4UnityEditorWindow.BasePath + "Core";

	protected static readonly string AForgeCoreDll = 
	"AForge.Core.dll";

	protected static readonly string AForgeFuzzyPath = 
	AI4UnityEditorWindow.BasePath + "Fuzzy";

	protected static readonly string AForgeFuzzyDll = 
	"AForge.Fuzzy.dll";

	protected static readonly string AForgeGeneticPath = 
	AI4UnityEditorWindow.BasePath + "Genetic";

	protected static readonly string AForgeGeneticDll = 
	"AForge.Genetic.dll";

	protected static readonly string AForgeMachineLearningPath = 
	AI4UnityEditorWindow.BasePath + "MachineLearning";

	protected static readonly string AForgeMachineLearningDll = 
	"AForge.MachineLearning.dll";

	protected static readonly string AForgeMathPath = 
	AI4UnityEditorWindow.BasePath + "Math";

	protected static readonly string AForgeMathDll = 
	"AForge.Math.dll";

	protected static readonly string AForgeNeuroPath = 
	AI4UnityEditorWindow.BasePath + "Neuro";

	protected static readonly string AForgeNeuroDll = 
	"AForge.Neuro.dll";

	protected static readonly string PlayerPrefs_BasePath = 
	"AI4Unity_BasePath";
	#endregion

	#region public instance properties
	public string BuildPath{
		get{
			return this._buildPath;
		}
		set{
			this._buildPath = value;
		}
	}

	public bool BuildAForgeCore{
		get{
			return this._buildAForgeCore;
		}
		set{
			this._buildAForgeCore = value;
		}
	}

	public bool BuildAForgeFuzzy{
		get{
			return this._buildAForgeFuzzy;
		}
		set{
			this._buildAForgeFuzzy = value;
		}
	}

	public bool BuildAForgeGenetic{
		get{
			return this._buildAForgeGenetic;
		}
		set{
			this._buildAForgeGenetic = value;
		}
	}

	public bool BuildAForgeMachineLearning{
		get{
			return this._buildAForgeMachineLearning;
		}
		set{
			this._buildAForgeMachineLearning = value;
		}
	}

	public bool BuildAForgeMath{
		get{
			return this._buildAForgeMath;
		}
		set{
			this._buildAForgeMath = value;
		}
	}

	public bool BuildAForgeNeuro{
		get{
			return this._buildAForgeNeuro;
		}
		set{
			this._buildAForgeNeuro = value;
		}
	}
	#endregion

	#region public instance properties
	protected string _buildPath;
	protected bool _buildAForgeCore = true;
	protected bool _buildAForgeFuzzy = true;
	protected bool _buildAForgeGenetic = true;
	protected bool _buildAForgeMachineLearning = true;
	protected bool _buildAForgeMath = true;
	protected bool _buildAForgeNeuro = true;
	protected bool _compiling = false;
	#endregion

	#region protected instance methods
	protected virtual CompilerResults CompileDll(
		string sourceFolder,
		string dllFilename,
		IEnumerable<string> referencedAssemblies
	){
		if (dllFilename != null) {
			//dllFilename = dllFilename.Replace('/', Path.PathSeparator);
			dllFilename = new FileInfo(dllFilename).FullName;
		}

		CSharpCodeProvider provider = new CSharpCodeProvider();
		CompilerParameters parameters = new CompilerParameters();
		parameters.IncludeDebugInformation = false;
		parameters.GenerateExecutable = false;
		parameters.GenerateInMemory = false;
		parameters.OutputAssembly = dllFilename;

		if (referencedAssemblies != null){
			foreach (string assembly in referencedAssemblies){
				if (!string.IsNullOrEmpty(assembly)){
					parameters.ReferencedAssemblies.Add(assembly);
				}
			}
		}

		return provider.CompileAssemblyFromFile(
			parameters, 
			this.GetSourceFiles(sourceFolder)
		);
	}

	protected string[] GetSourceFiles(string sourceFolder){
		DirectoryInfo dir = new DirectoryInfo(sourceFolder);
		List<string> files = new List<string> ();

		if (dir.Exists) {
			foreach (FileInfo file in dir.GetFiles("*.cs", SearchOption.AllDirectories)){
				files.Add(file.FullName);
			}
		}

		return files.ToArray ();
	}
	#endregion

	#region EditorWindow methods
	public virtual void OnGUI(){
		GUILayout.Space (30);

		if (this._compiling) {
			GUILayout.Label ("Compiling...");
		}else{
			GUILayout.BeginHorizontal ();
			GUILayout.Label ("Build Path:");
			this.BuildPath = GUILayout.TextField (this.BuildPath);

			if (GUILayout.Button ("+")) {
				this.BuildPath = EditorUtility.SaveFolderPanel(
					"Select the folder where the DLLs will be created",
					string.IsNullOrEmpty(this.BuildPath)? Application.dataPath : this.BuildPath,
					string.Empty
				);
			}
			GUILayout.EndHorizontal ();
			GUILayout.Space (10);

			GUILayout.BeginHorizontal ();
			GUILayout.BeginVertical ();
			this.BuildAForgeCore = GUILayout.Toggle (
				this.BuildAForgeCore,
				"AForge.NET Core"
			);

			this.BuildAForgeMath = GUILayout.Toggle (
				this.BuildAForgeMath,
				"AForge.NET Math"
			);

			this.BuildAForgeFuzzy = GUILayout.Toggle (
				this.BuildAForgeFuzzy,
				"AForge.NET Fuzzy"
			);
			GUILayout.EndVertical ();

			GUILayout.BeginVertical ();
			this.BuildAForgeGenetic = GUILayout.Toggle (
				this.BuildAForgeGenetic,
				"AForge.NET Genetic"
			);

			this.BuildAForgeNeuro = GUILayout.Toggle (
				this.BuildAForgeNeuro,
				"AForge.NET Neuro"
			);

			this.BuildAForgeMachineLearning = GUILayout.Toggle (
				this.BuildAForgeMachineLearning,
				"AForge.NET Machine Learning"
			);
			GUILayout.EndVertical ();
			GUILayout.EndHorizontal ();
			GUILayout.Space (10);

			if (GUILayout.Button ("Build DLLs")) {
				if (string.IsNullOrEmpty(this.BuildPath)){
					EditorUtility.DisplayDialog(
						"Build path not selected",
						"You must select the folder where the DLLs will be created.",
						"OK"
					);
				}else{
					StringBuilder warnings = new StringBuilder();
					StringBuilder errors = new StringBuilder();
					this._compiling = true;

					string aForgeCoreDll = new FileInfo( 
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AForgeCoreDll
					).FullName;

					string aForgeMathDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AForgeMathDll
					).FullName;

					string aForgeFuzzyDll = new FileInfo( 
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AForgeFuzzyDll
					).FullName;

					string aForgeGeneticDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AForgeGeneticDll
					).FullName;

					string aForgeNeuroDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AForgeNeuroDll
					).FullName;

					PlayerPrefs.SetString (
						AI4UnityEditorWindow.PlayerPrefs_BasePath,
						this.BuildPath
					);

					if (this.BuildAForgeCore){
						CompilerResults results = this.CompileDll(
							AI4UnityEditorWindow.AForgeCorePath,
							aForgeCoreDll,
							null
						);

						foreach (CompilerError error in results.Errors){
							if (error.IsWarning){
								warnings.AppendLine(error.ToString());
							}else{
								errors.AppendLine(error.ToString());
							}
						}
					}

					Debug.Log("Generating DLLs..."); 
					if (this.BuildAForgeMath){
						CompilerResults results = this.CompileDll(
							AI4UnityEditorWindow.AForgeMathPath
							,
							aForgeMathDll
							,
							new string[]{
								"System.dll",
								aForgeCoreDll
							}
						);

						foreach (CompilerError error in results.Errors){
							if (error.IsWarning){
								warnings.AppendLine(error.ToString());
							}else{
								errors.AppendLine(error.ToString());
							}
						}
					}

					if (this.BuildAForgeFuzzy){
						CompilerResults results = this.CompileDll(
							AI4UnityEditorWindow.AForgeFuzzyPath
							,
							aForgeFuzzyDll
							,
							new string[]{
								"System.dll",
								aForgeCoreDll,
								aForgeMathDll
							}
						);

						foreach (CompilerError error in results.Errors){
							if (error.IsWarning){
								warnings.AppendLine(error.ToString());
							}else{
								errors.AppendLine(error.ToString());
							}
						}
					}

					if (this.BuildAForgeGenetic){
						CompilerResults results = this.CompileDll(
							AI4UnityEditorWindow.AForgeGeneticPath
							,
							aForgeGeneticDll
							,
							new string[]{
								"System.dll",
								"mscorlib.dll",
								aForgeCoreDll,
								aForgeMathDll
							}
						);

						foreach (CompilerError error in results.Errors){
							if (error.IsWarning){
								warnings.AppendLine(error.ToString());
							}else{
								errors.AppendLine(error.ToString());
							}
						}
					}

					if (this.BuildAForgeNeuro){
						CompilerResults results = this.CompileDll(
							AI4UnityEditorWindow.AForgeNeuroPath
							,
							aForgeNeuroDll
							,
							new string[]{
								"System.dll",
								"mscorlib.dll",
								aForgeCoreDll,
								aForgeMathDll,
								aForgeGeneticDll
							}
						);

						foreach (CompilerError error in results.Errors){
							if (error.IsWarning){
								warnings.AppendLine(error.ToString());
							}else{
								errors.AppendLine(error.ToString());
							}
						}
					}

					if (this.BuildAForgeMachineLearning){
						CompilerResults results = this.CompileDll(
							AI4UnityEditorWindow.AForgeMachineLearningPath
							,
							this.BuildPath + 
							Path.DirectorySeparatorChar + 
							AI4UnityEditorWindow.AForgeMachineLearningDll
							,
							new string[]{
								"System.dll",
								aForgeCoreDll,
								aForgeMathDll
							}
						);

						foreach (CompilerError error in results.Errors){
							if (error.IsWarning){
								warnings.AppendLine(error.ToString());
							}else{
								errors.AppendLine(error.ToString());
							}
						}
					}

					if (warnings.Length > 0){
						Debug.LogWarning(warnings.ToString());
					}

					if (errors.Length > 0){
						Debug.LogError(errors.ToString());
					}else{
						Debug.Log("The DLLs were generated successfully"); 
					}

					this._compiling = false;
				}
			}
		}
	}
	#endregion
}
