using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlterbladeBot.Game.GameObjects
{
    enum SkillEffectResult
    {
        Success,
        Blocked,
        Return
    }

    internal class SkillEffects
    {
        public delegate SkillEffectResult SkillEffect(Hero source, Hero target, Skill skill);

        public static SkillEffectResult Damage(Hero source, Hero target, Skill skill)
        {

            return SkillEffectResult.Success;
        }
    }
}
