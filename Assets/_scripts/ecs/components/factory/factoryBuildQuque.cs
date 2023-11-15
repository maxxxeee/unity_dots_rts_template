using Unity.Collections;
using Unity.Entities;

public struct factoryBuildQuque : IComponentData
{
    public FixedList4096Bytes<int> buildQuque;
}
