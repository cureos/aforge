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

		if (PlayerPrefs.HasKey (AI4UnityEditorWindow.PlayerPrefs_BuildBasePath)) {
			w.BuildPath = PlayerPrefs.GetString (AI4UnityEditorWindow.PlayerPrefs_BuildBasePath);

			DirectoryInfo directory = new DirectoryInfo(w.BuildPath);
			if (!directory.Exists){
				w.BuildPath = new DirectoryInfo(Application.dataPath).Parent.FullName;
			}
		} else {
			w.BuildPath = new DirectoryInfo(Application.dataPath).Parent.FullName;
		}
	}
	#endregion

	#region protected class properties

	protected static readonly string AccordBasePath = 
	Application.dataPath	+ Path.DirectorySeparatorChar + 
	"Accord.NET"			+ Path.DirectorySeparatorChar + 
	"Runtime"				+ Path.DirectorySeparatorChar;

	protected static readonly string AccordCorePath = 
	AI4UnityEditorWindow.AccordBasePath + "Accord.Core";

	protected static readonly string AccordCoreDll = 
	"Accord.Core.dll";

	protected static readonly string AccordMachineLearningPath = 
	AI4UnityEditorWindow.AccordBasePath + "Accord.MachineLearning";

	protected static readonly string AccordMachineLearningDll = 
	"Accord.MachineLearning.dll";

	protected static readonly string AccordMathPath = 
	AI4UnityEditorWindow.AccordBasePath + "Accord.Math";

	protected static readonly string AccordMathDll = 
	"Accord.Math.dll";

	protected static readonly string AccordNeuroPath = 
	AI4UnityEditorWindow.AccordBasePath + "Accord.Neuro";

	protected static readonly string AccordNeuroDll = 
	"Accord.Neuro.dll";

	protected static readonly string AccordStatisticsPath = 
	AI4UnityEditorWindow.AccordBasePath + "Accord.Statistics";

	protected static readonly string AccordStatisticsDll = 
	"Accord.Statistics.dll";

	protected static readonly string AForgeBasePath = 
	Application.dataPath	+ Path.DirectorySeparatorChar + 
	"AForge.NET"			+ Path.DirectorySeparatorChar + 
	"Runtime"				+ Path.DirectorySeparatorChar;

	protected static readonly string AForgeCorePath = 
	AI4UnityEditorWindow.AForgeBasePath + "Core";

	protected static readonly string AForgeCoreDll = 
	"AForge.Core.dll";

	protected static readonly string AForgeFuzzyPath = 
	AI4UnityEditorWindow.AForgeBasePath + "Fuzzy";

	protected static readonly string AForgeFuzzyDll = 
	"AForge.Fuzzy.dll";

	protected static readonly string AForgeGeneticPath = 
	AI4UnityEditorWindow.AForgeBasePath + "Genetic";

	protected static readonly string AForgeGeneticDll = 
	"AForge.Genetic.dll";

	protected static readonly string AForgeMachineLearningPath = 
	AI4UnityEditorWindow.AForgeBasePath + "MachineLearning";

	protected static readonly string AForgeMachineLearningDll = 
	"AForge.MachineLearning.dll";

	protected static readonly string AForgeMathPath = 
	AI4UnityEditorWindow.AForgeBasePath + "Math";

	protected static readonly string AForgeMathDll = 
	"AForge.Math.dll";

	protected static readonly string AForgeNeuroPath = 
	AI4UnityEditorWindow.AForgeBasePath + "Neuro";

	protected static readonly string AForgeNeuroDll = 
	"AForge.Neuro.dll";

	protected static readonly string MonoDll = 
	Application.dataPath	+ Path.DirectorySeparatorChar + 
	"Accord.NET"			+ Path.DirectorySeparatorChar +
	"Dependencies"			+ Path.DirectorySeparatorChar + 
	"Mono4Unity"			+ Path.DirectorySeparatorChar +
	"Mono4Unity.dll";

	protected static readonly string PlayerPrefs_BuildBasePath = 
	"BuildBasePath";
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

	public bool BuildAccordCore{
		get{
			return this._buildAccordCore;
		}
		set{
			this._buildAccordCore = value;
		}
	}

	public bool BuildAccordMachineLearning{
		get{
			return this._buildAccordMachineLearning;
		}
		set{
			this._buildAccordMachineLearning = value;
		}
	}
	
	public bool BuildAccordMath{
		get{
			return this._buildAccordMath;
		}
		set{
			this._buildAccordMath = value;
		}
	}
	
	public bool BuildAccordNeuro{
		get{
			return this._buildAccordNeuro;
		}
		set{
			this._buildAccordNeuro = value;
		}
	}

	public bool BuildAccordStatistics{
		get{
			return this._buildAccordStatistics;
		}
		set{
			this._buildAccordStatistics = value;
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
	protected bool _buildAccordCore = true;
	protected bool _buildAccordMachineLearning = true;
	protected bool _buildAccordMath = true;
	protected bool _buildAccordNeuro = true;
	protected bool _buildAccordStatistics = true;
	protected bool _buildAForgeCore = true;
	protected bool _buildAForgeFuzzy = true;
	protected bool _buildAForgeGenetic = true;
	protected bool _buildAForgeMachineLearning = true;
	protected bool _buildAForgeMath = true;
	protected bool _buildAForgeNeuro = true;
	protected bool _compiling = false;
	#endregion

	#region protected instance methods
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
			this.BuildPath = GUILayout.TextField (this.BuildPath != null ? this.BuildPath : string.Empty);

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
			GUILayout.BeginVertical ();

			this.BuildAccordCore = GUILayout.Toggle (
				this.BuildAccordCore,
				"Accord.NET Core"
			);
			
			this.BuildAccordMath = GUILayout.Toggle (
				this.BuildAccordMath,
				"Accord.NET Math"
			);

			this.BuildAccordStatistics = GUILayout.Toggle (
				this.BuildAccordStatistics,
				"Accord.NET Statistics"
			);
			
			this.BuildAccordNeuro = GUILayout.Toggle (
				this.BuildAccordNeuro,
				"Accord.NET Neuro"
			);
			
			this.BuildAccordMachineLearning = GUILayout.Toggle (
				this.BuildAccordMachineLearning,
				"Accord.NET Machine Learning"
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

					string aForgeMachineLearningDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AForgeMachineLearningDll
					).FullName;

					string accordCoreDll = new FileInfo( 
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AccordCoreDll
					).FullName;
					
					string accordMathDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AccordMathDll
					).FullName;
					
					string accordStatisticsDll = new FileInfo( 
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AccordStatisticsDll
					).FullName;

					string accordNeuroDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AccordNeuroDll
					).FullName;

					string accordMachineLearningDll = new FileInfo(
						this.BuildPath + 
						Path.DirectorySeparatorChar + 
						AI4UnityEditorWindow.AccordMachineLearningDll
					).FullName;

					PlayerPrefs.SetString (
						AI4UnityEditorWindow.PlayerPrefs_BuildBasePath,
						this.BuildPath
					);

					Debug.Log("Generating DLLs..."); 
					if (this.BuildAForgeCore){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AForgeCorePath),
							new string[0],
							new string[0],
							new FileInfo(aForgeCoreDll).FullName
						);

						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAForgeMath){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AForgeMathPath),
							new string[]{
								"System.dll",
								aForgeCoreDll
							},
							new string[0],
							new FileInfo(aForgeMathDll).FullName
						);
						
						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAForgeFuzzy){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AForgeFuzzyPath),
							new string[]{
								"System.dll",
								aForgeCoreDll,
								aForgeMathDll
							},
							new string[0],
							new FileInfo(aForgeFuzzyDll).FullName
						);
						
						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAForgeGenetic){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AForgeGeneticPath),
							new string[]{
								"System.dll",
								"mscorlib.dll",
								aForgeCoreDll,
								aForgeMathDll
							},
							new string[0],
							new FileInfo(aForgeGeneticDll).FullName
						);

						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAForgeNeuro){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AForgeNeuroPath),
							new string[]{
								"System.dll",
								"mscorlib.dll",
								aForgeCoreDll,
								aForgeMathDll,
								aForgeGeneticDll
							},
							new string[0],
							new FileInfo(aForgeNeuroDll).FullName
						);

						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAForgeMachineLearning){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AForgeMachineLearningPath),
							new string[]{
								"System.dll",
								aForgeCoreDll,
								aForgeMathDll
							},
							new string[0],
							new FileInfo(aForgeMachineLearningDll).FullName
						);

						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAccordCore){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AccordCorePath),
							new string[]{
								"System.dll",
								AI4UnityEditorWindow.MonoDll,
							},
							new string[0],
							new FileInfo(accordCoreDll).FullName
						);

						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAccordMath){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AccordMathPath),
							new string[]{
								"System.dll",
								accordCoreDll,
								aForgeCoreDll,
								aForgeMathDll,
								AI4UnityEditorWindow.MonoDll,
							},
							new string[0],
							new FileInfo(accordMathDll).FullName
						);
						
						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAccordStatistics){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AccordStatisticsPath),
							new string[]{
								"System.dll",
								accordCoreDll,
								accordMathDll,
								aForgeCoreDll,
								aForgeMathDll,
								AI4UnityEditorWindow.MonoDll,
							},
							new string[0],
							new FileInfo(accordStatisticsDll).FullName
						);
						
						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAccordNeuro){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AccordNeuroPath),
							new string[]{
								"System.dll",
								accordCoreDll,
								accordMathDll,
								accordStatisticsDll,
								aForgeCoreDll,
								aForgeMathDll,
								aForgeNeuroDll,
								AI4UnityEditorWindow.MonoDll,
							},
							new string[0],
							new FileInfo(accordNeuroDll).FullName
						);
						
						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
							}
						}
					}

					if (this.BuildAccordMachineLearning){
						string[] results = EditorUtility.CompileCSharp(
							this.GetSourceFiles(AI4UnityEditorWindow.AccordMachineLearningPath),
							new string[]{
								"System.dll",
								accordCoreDll,
								accordMathDll,
								accordStatisticsDll,
								aForgeCoreDll,
								aForgeMathDll,
								aForgeMachineLearningDll,
								AI4UnityEditorWindow.MonoDll,
							},
							new string[0],
							new FileInfo(accordMachineLearningDll).FullName
						);
						
						foreach (string result in results){
							if (result.Contains("warning")){
								warnings.AppendLine(result);
							}else if (result.Contains("error")){
								errors.AppendLine(result);
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
