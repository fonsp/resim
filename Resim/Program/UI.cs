using System;
using System.Globalization;
using OpenTK;

namespace Resim.Program
{
	/* ShowCombo laat zien de hoeveelheid damage zien
	 * Op dit moment is dat altijd '50'
	 */
	public partial class Game
	{
		private void ShowCombo(double combo, Vector2 position)
		{
			ShowCombo(combo, position, 0, false);
		}
		/* Er is een array van 100 textFields, en deze worden een voor een gebruikt
		 * zodat er niet telkens nieuwe object gemaakt moeten worden
		 */
		private void ShowCombo(double combo, Vector2 position, int round, bool exclamationMark)
		{
			combo = Math.Round(combo, round);
			string comboString = combo.ToString(CultureInfo.InvariantCulture);
			ShowCombo(comboString, position, exclamationMark);
		}

		private void ShowCombo(string combo, Vector2 position, bool exclamationMark)
		{

			comboTextFields[comboFieldCounter].age = 0;
			if(exclamationMark)
			{
				comboTextFields[comboFieldCounter].text = combo + "!";
			}
			else
			{
				comboTextFields[comboFieldCounter].text = combo;
			}
			comboTextFields[comboFieldCounter].position = position;
			Vector2 vel = new Vector2((float)random.NextDouble() - .5f, -(float)random.NextDouble());

			vel *= 100;
			comboTextFields[comboFieldCounter].velocity = vel;
			comboFieldCounter++;
			comboFieldCounter %= comboTextFields.Length;
		}
	}
}