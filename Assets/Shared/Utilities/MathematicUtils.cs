namespace Shared.Utilities
{
    public static class MathematicUtils
    {
        public static float Remap(this float input, float from1, float to1, float from2, float to2)
        {
            return from2 + (input - from1) * (to2 - from2) / (to1 - from1);
        }
    }
}