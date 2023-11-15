using Unity.Collections;

using Unity.Entities;

public struct detected_units_list_component : IComponentData
{
    public FixedList64Bytes<Entity> results;
}
