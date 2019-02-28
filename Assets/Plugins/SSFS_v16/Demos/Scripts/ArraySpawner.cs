using System.Collections;
using UnityEngine;

public class ArraySpawner : MonoBehaviour
{
	public GameObject prefab;
	public int xCount;
	public int yCount;
	public int zCount;
	public Vector3 spacing = Vector3.one;
	public bool centered = true;
	public bool spawnAtOnce = false;
	public bool hideInHierarchy = true;

	private IEnumerator Start()
	{
		yield return new WaitWhile( () => { return prefab == null; } );
		StartCoroutine( SpawnObjects() );
	}

	private IEnumerator SpawnObjects()
	{
		int xx = 0;
		int yy = 0;
		int zz = 0;
		while ( xx < xCount )
		{
			yy = 0;
			while ( yy < yCount )
			{
				zz = 0;
				while ( zz < zCount )
				{
					if ( !spawnAtOnce ) yield return new WaitForEndOfFrame();
					SpawnIndex( ( float )xx / xCount , ( float )yy / yCount , ( float )zz / zCount );
					zz++;
				}
				yy++;
			}
			xx++;
		}
	}

	private void SpawnIndex( float xphase , float yphase , float zphase )
	{
		Vector3 p = transform.position;
		Vector3 phase = new Vector3( xphase , yphase , zphase );
		Vector3 span = new Vector3( spacing.x * xCount , spacing.y * yCount , spacing.z * zCount );
		float xpos = p.x + ( centered ? ( phase.x - 0.5f ) : phase.x ) * span.x;
		float ypos = p.y + ( centered ? ( phase.y - 0.5f ) : phase.y ) * span.y;
		float zpos = p.z + ( centered ? ( phase.z - 0.5f ) : phase.z ) * span.z;
		GameObject o = Instantiate( prefab , new Vector3( xpos , ypos , zpos ) , Quaternion.identity );
		o.transform.SetParent( transform );
		if ( hideInHierarchy ) o.hideFlags = HideFlags.HideInHierarchy;
	}
}