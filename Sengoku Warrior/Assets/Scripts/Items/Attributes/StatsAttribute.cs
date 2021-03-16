
namespace SengokuWarrior
{
    public class StatsAttribute : ItemAttribute 
    {
        public override int Priority()
        {
            return 11;
        }
        public float[] stats = new float[5];
       public bool[] enabled = new bool[5];

    }
}
