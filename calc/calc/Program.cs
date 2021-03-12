using System;
using System.Collections.Generic;

namespace calc
{
	class Calculator
	{
		public static List<string> digits = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", ",", "." };
		public static List<string> operands = new List<string>() {"^", "/", "*", "+", "-"};
		public static List<string> functions = new List<string>() {  "sin", "cos",  "abs"};
		public static List<string> brackets = new List<string>() { "(", ")" };
		public static List<string> constants = new List<string>() { "pi" };
		public static Dictionary<string, string> constantsValues = new Dictionary<string, string>{
			{"pi","3,14159265359"}
		};
		public  double CalculateExpression(string expression)
		{
			expression = expression.Replace(" ", "");   expression = expression.Replace(",", ".");
			List<string> symbols = new List<string>();

			string lastSymbol = "";
			string lastFunction = "";

			for (int i = 0; i < expression.Length; i++) {
				if (brackets.Contains(expression[i].ToString())){
					if (lastSymbol != ""){
						symbols.Add(lastSymbol);
						lastSymbol = "";
					}
					symbols.Add(expression[i].ToString());
				}
				else if (digits.Contains(expression[i].ToString())|| (expression[i] == ',' && lastSymbol.IndexOf(",") == -1) ){  //  || (expression[i] == ',' && lastSymbol.IndexOf(",") == -1)
					// if ( expression[i] == ',' && lastSymbol.IndexOf(",") == -1)  {lastSymbol +=".";} else
					lastSymbol += expression[i];
				}
				else if(operands.Contains(expression[i].ToString())) {
					if (lastSymbol != ""){
						symbols.Add(lastSymbol);
						lastSymbol = "";
					}

					if (symbols.Count >= 1 && operands.Contains(symbols[symbols.Count - 1]) || symbols.Count == 0) {
						string number = "";

						switch (expression[i].ToString()) {
							case "-": number += "-"; break;
							case "+": number += "+"; break;
						}

						i++;
						while (i < expression.Length && digits.Contains(expression[i].ToString())){
							number += expression[i];
							i++;
						}

						symbols.Add(number);
						i--;
					}
					else symbols.Add(expression[i].ToString());
				}else{
					lastFunction += expression[i].ToString().ToLower();

					if (constants.Contains(lastFunction)) {
						symbols.Add(constantsValues[lastFunction]);
						lastFunction = "";
					}
					else if (functions.Contains(lastFunction))
					{
						int functionStart = i + 1;
						int functionEnd = 0;
						int bracketsSum = 1;

						for (int j = functionStart + 1; j < expression.Length; j++)
						{
							if (expression[j].ToString() == "(") bracketsSum++;
							if (expression[j].ToString() == ")") bracketsSum--;
							if (bracketsSum == 0)
							{
								functionEnd = j;
								i = functionEnd;
								break;
							}
						}

						char[] buffer = new char[functionEnd - functionStart - 1];
						expression.CopyTo(functionStart + 1, buffer, 0, functionEnd - functionStart - 1);
						string functionParametrs = new string(buffer);


						if (lastFunction == "sin") symbols.Add(Math.Sin(CalculateExpression(functionParametrs)).ToString());
						if (lastFunction == "cos") symbols.Add(Math.Cos(CalculateExpression(functionParametrs)).ToString());
						if (lastFunction == "abs") symbols.Add(Math.Abs(CalculateExpression(functionParametrs)).ToString());

						lastFunction = "";
					}
				}
			}

			if (lastSymbol != ""){
				symbols.Add(lastSymbol);
				lastSymbol = "";
			}

			while (symbols.Contains("(")) {
				int bracketsStart = 0;
				int bracketsEnd = 0;
				int bracketsSum = 0;

				for (int i = 0; i < symbols.Count; i++) {
					if (symbols[i] == "(") {
						bracketsStart = i;
						bracketsSum = 1;
						break;
					}
				}

				for (int i = bracketsStart + 1; i < symbols.Count; i++) {
					if (symbols[i] == "(") bracketsSum++;
					if (symbols[i] == ")") bracketsSum--;
					if (bracketsSum == 0) {
						bracketsEnd = i;
						break;
					}
				}

				string bracketsExpression = "";
				for (int i = bracketsStart + 1; i < bracketsEnd; i++) bracketsExpression += symbols[i];

				symbols[bracketsStart] = CalculateExpression(bracketsExpression).ToString();
				symbols.RemoveRange(bracketsStart + 1, bracketsEnd - bracketsStart);
			}

			foreach(var j in operands){
				var flagO = true;
				while (flagO){
					flagO = false;
					for (int i = 0; i < symbols.Count; i++){
						if (symbols[i] == j){
							symbols[i - 1] = symbols[i - 1] + " " + symbols[i + 1] + " " + j;
							symbols.RemoveRange(i, 2);

							flagO = true;
							break;
						}
					}
				}
			}

			List<string> result = new List<string>();

			string[] temp = symbols[0].Split(' ');

			for (int i = 0; i < temp.Length; i++) {
				if (operands.Contains(temp[i])) {
					if (temp[i] == "^") {
						result[result.Count - 2] = Math.Pow(double.Parse(result[result.Count - 2]), double.Parse(result[result.Count - 1])).ToString();
						result.RemoveRange(result.Count - 1, 1);
					}
					if (temp[i] == "+") {
						result[result.Count - 2] = (double.Parse(result[result.Count - 2]) + double.Parse(result[result.Count - 1])).ToString();
						result.RemoveRange(result.Count - 1, 1);
					}
					if (temp[i] == "-") {
						result[result.Count - 2] = (double.Parse(result[result.Count - 2]) - double.Parse(result[result.Count - 1])).ToString();
						result.RemoveRange(result.Count - 1, 1);
					}
					if (temp[i] == "*") {
						result[result.Count - 2] = (double.Parse(result[result.Count - 2]) * double.Parse(result[result.Count - 1])).ToString();
						result.RemoveRange(result.Count - 1, 1);
					}
					if (temp[i] == "/") {
						result[result.Count - 2] = (double.Parse(result[result.Count - 2]) / double.Parse(result[result.Count - 1])).ToString();
						result.RemoveRange(result.Count - 1, 1);
					}
				}
				else result.Add(temp[i]);
			}

			return double.Parse(result[0]);
		}

		public  List<string> GetParametrs(string functionParametrs){
			int bracketsSum = 0;
			int functionEnd = 0;
			for (int j = 0; j < functionParametrs.Length; j++){
				if (functionParametrs[j].ToString() == "(") bracketsSum++;
				if (functionParametrs[j].ToString() == ")") bracketsSum--;
				if (functionParametrs[j].ToString() == ";" && bracketsSum == 0){
					functionEnd = j;
					break;
				}
			}
			var buffer = new char[functionEnd];
			functionParametrs.CopyTo(0, buffer, 0, functionEnd);
			string firstParametr = new string(buffer);

			buffer = new char[functionParametrs.Length - functionEnd - 1];
			functionParametrs.CopyTo(functionEnd + 1, buffer, 0, functionParametrs.Length - functionEnd - 1);
			string secondParametr = new string(buffer);

			return ( new List<string>() { firstParametr, secondParametr } );
		}

	} 






	class MainClass
	{
		public static void Main (string[] args)
		{
			var calc= new Calculator();
			while (true)
			{
				Console.WriteLine("Введите выражение:");
				string temp = Console.ReadLine();
				Console.WriteLine("Результат:"+calc.CalculateExpression(temp).ToString());
			}     
		}
	}
}
