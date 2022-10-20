using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class WaterChecker : MonoBehaviour
{
	public bool isUnderWaterSensor;
	public PlayerDriver player;
	public BuoyancyPhysics waterForces;

    private void Start()
    {
        if (player == null)
        {
			Debug.LogWarning("Your missing a player reference on the water checker checker script");
        }
    }

    void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Water" && player != null && !isUnderWaterSensor)
		{
			waterForces.DetectDiveImpact(player.physicsProperties.physicalSpeed.y);
		}

		if (col.tag == "Water" && player != null && isUnderWaterSensor)
		{
			player.physicsProperties.isUnderWater = true;
		}
	}

	void OnTriggerStay(Collider col)
	{
		if (col.tag == "Water" && player != null && !isUnderWaterSensor)
		{
			player.physicsProperties.isSwimming = true;
			player.physicsProperties.ApplyGravity = false;
			player.BuoyancyForce.enablePhysics = true;
			waterForces.currentWaterHeight = col.bounds.max;
			UIManager.Instance.displaySwimmingActions = true;
		}

		if (col.tag == "Water" && player != null && isUnderWaterSensor && !waterForces.waterSurfaced)
		{
			player.physicsProperties.isUnderWater = true;
		}
	}

	void OnTriggerExit(Collider col)
	{
		if (col.tag == "Water" && player != null && !isUnderWaterSensor)
		{
			waterForces.SwitchState(false);
			player.physicsProperties.isSwimming = false;
			player.physicsProperties.ApplyGravity = true;
			player.BuoyancyForce.enablePhysics = false;

			waterForces.currentWaterHeight = col.transform.position;
			UIManager.Instance.clearAllDisplayActions = true;
			waterForces.EndDive();
		}

		if (col.tag == "Water" && player != null && isUnderWaterSensor)
		{
			player.physicsProperties.isUnderWater = false;
		}
	}
}
