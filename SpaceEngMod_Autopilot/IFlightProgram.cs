namespace SpaceEngMod
{
    public interface IFlightProgram
    {
        string Name { get; }
        void Initialize(IFlightContext ctx);
        void Update(IFlightContext ctx, ref bool programTerminated);
    }
}
 