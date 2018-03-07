using UnityEngine;

public class RingTargeter : MonoBehaviour
{
	//------------------------- PUBLIC -------------------------//

		
	private void Update()
	{
		if (RaceGameMode.S.nextRing != null)
		{
			transform.position = RaceGameMode.S.nextRing.transform.position;
			transform.rotation = RaceGameMode.S.nextRing.transform.rotation;
		}
		else
			Destroy(gameObject);
	}
}