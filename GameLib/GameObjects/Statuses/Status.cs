using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.GameLib.GameObjects.Statuses
{
	internal abstract class Status
	{
		protected string name;
		protected int duration;
		protected Hero source;
		protected Skill sourceSkill;

		public string Name => name; 
		public int Duration => duration;
		public Hero Source => source;
		public Skill SourceSkill => sourceSkill;

		public Status(string name, int duration, Hero source, Skill sourceSkill)
		{
			this.name = name;
			this.duration = duration;
			this.source = source;
			this.sourceSkill = sourceSkill;
		}

		public abstract bool End(bool showText);
		public abstract  bool Update();
	}
}
