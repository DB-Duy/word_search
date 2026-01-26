namespace Shared.Service.Ump
{
    public static class UmpFlag
    {
#if USING_UMP
        public static bool IsUmpReady { get; set; } = false;
#endif
    }
}