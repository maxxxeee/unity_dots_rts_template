using Unity.Entities;
using Unity.Mathematics;


// small system only offering a helper function user by the look towards destination system

public partial class HelperFunctions_system : SystemBase
{
    protected override void OnUpdate()
    {
        
    }

    public static quaternion lookTowardsDestinationSystem(float3 TargetPosition, float3 SourcePosition)
    {

       

        float3 targetDirection = math.normalize(TargetPosition - SourcePosition);
        

        quaternion temp = quaternion.LookRotation(targetDirection, new float3(0, 1, 0));
        
        return temp;
    }
}
