using UnityEngine;
using System.Globalization;

public class Culture {
	public static CultureInfo GetCurrentCulture(){
		//Application.systemLanguage 
		return new CultureInfo("es-es");
	}
}
