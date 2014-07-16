using UnityEngine;
using System.Collections;

public interface ICatchingMiceWorldObjectTrap
{
    float Health { get; set; }
    int Ammo { get; set; }
    float Cost { get; set; }
    float Damage { get; set; }
	float Interval { get; set; }
	int TileRange { get; set; }
	CatchingMiceTrap TrapObject { get; }

    void OnHit(ICatchingMiceCharacter character);
    void DestroySelf();
    void PlayerInteraction();
}
